using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Players;
using VoidEmpires.Domain.Players;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Players;

public sealed class StartingCivilizationService(VoidEmpiresDbContext dbContext) : IStartingCivilizationService
{
    public async Task<CreateStartingCivilizationResult> CreateAsync(
        CreateStartingCivilizationRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationErrors = Validate(request);
        if (validationErrors.Count > 0)
        {
            return CreateStartingCivilizationResult.Failure(validationErrors.ToArray());
        }

        var normalizedUserId = request.UserId.Trim();
        var exists = await dbContext.PlayerProfiles
            .AnyAsync(profile => profile.UserId == normalizedUserId, cancellationToken);

        if (exists)
        {
            return CreateStartingCivilizationResult.Failure("Player profile already exists for this user.");
        }

        var profile = PlayerProfile.Create(normalizedUserId, request.DisplayName);
        var civilization = Civilization.Create(profile.Id, request.CivilizationName, request.Archetype, request.HomePlanetId);
        profile.AddCivilization(civilization);

        dbContext.PlayerProfiles.Add(profile);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreateStartingCivilizationResult.Success(profile.Id, civilization.Id, civilization.HomePlanetId);
    }

    private static List<string> Validate(CreateStartingCivilizationRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            errors.Add("User id is required.");
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            errors.Add("Display name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.CivilizationName))
        {
            errors.Add("Civilization name is required.");
        }

        return errors;
    }
}
