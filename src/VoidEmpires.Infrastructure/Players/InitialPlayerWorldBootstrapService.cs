using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Players;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Players;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Players;

public sealed class InitialPlayerWorldBootstrapService : IInitialPlayerWorldBootstrapService
{
    private readonly VoidEmpiresDbContext _dbContext;
    private readonly IHomePlanetAllocator _homePlanetAllocator;

    public InitialPlayerWorldBootstrapService(VoidEmpiresDbContext dbContext)
        : this(dbContext, new HomePlanetAllocator(dbContext))
    {
    }

    public InitialPlayerWorldBootstrapService(VoidEmpiresDbContext dbContext, IHomePlanetAllocator homePlanetAllocator)
    {
        _dbContext = dbContext;
        _homePlanetAllocator = homePlanetAllocator;
    }

    public async Task<InitialPlayerWorldBootstrapResult> CreateAsync(
        InitialPlayerWorldBootstrapRequest request,
        CancellationToken cancellationToken = default)
    {
        var errors = Validate(request);
        if (errors.Count > 0) return InitialPlayerWorldBootstrapResult.Failure(errors.ToArray());

        var userId = request.UserId.Trim();
        var displayName = request.DisplayName.Trim();
        var civilizationName = request.CivilizationName.Trim();
        var homePlanetName = string.IsNullOrWhiteSpace(request.HomePlanetName)
            ? $"{civilizationName} Prime"
            : request.HomePlanetName.Trim();
        var normalizedDisplayName = PlayerProfile.NormalizeLookupKey(displayName);
        var normalizedCivilizationName = PlayerProfile.NormalizeLookupKey(civilizationName);

        if (await _dbContext.PlayerProfiles.AnyAsync(x => x.UserId == userId, cancellationToken)) return InitialPlayerWorldBootstrapResult.Failure("Player profile already exists for this user.");
        if (await _dbContext.PlayerProfiles.AnyAsync(x => x.NormalizedDisplayName == normalizedDisplayName, cancellationToken)) return InitialPlayerWorldBootstrapResult.Failure("Display name is already in use.");
        if (await _dbContext.Civilizations.AnyAsync(x => x.NormalizedName == normalizedCivilizationName, cancellationToken)) return InitialPlayerWorldBootstrapResult.Failure("Civilization name is already in use.");

        var homePlanet = await _homePlanetAllocator.AllocateAsync(homePlanetName, cancellationToken);
        var profile = PlayerProfile.Create(userId, displayName);
        var civilization = Civilization.Create(profile.Id, civilizationName, CivilizationArchetype.Balanced, homePlanet.Id);
        var stockpile = StartingHomeWorldBaseline.CreateResourceStockpile(homePlanet.Id);

        profile.AddCivilization(civilization);
        _dbContext.PlayerProfiles.Add(profile);
        _dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(homePlanet.Id, civilization.Id));
        _dbContext.PlanetResourceStockpiles.Add(stockpile);
        _dbContext.PlanetProductionProfiles.Add(StartingHomeWorldBaseline.CreateProductionProfile(homePlanet.Id));
        await _dbContext.SaveChangesAsync(cancellationToken);

        return InitialPlayerWorldBootstrapResult.Success(
            userId,
            profile.Id,
            civilization.Id,
            homePlanet.Id,
            homePlanet.Name,
            StartingHomeWorldBaseline.CreateResourceSnapshot());
    }

    private static List<string> Validate(InitialPlayerWorldBootstrapRequest request)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(request.UserId)) errors.Add("User id is required.");
        if (string.IsNullOrWhiteSpace(request.DisplayName)) errors.Add("Display name is required.");
        if (string.IsNullOrWhiteSpace(request.CivilizationName)) errors.Add("Civilization name is required.");
        return errors;
    }
}
