using VoidEmpires.Application.Development;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Development;

public sealed class DevelopmentSeedService(VoidEmpiresDbContext dbContext) : IDevelopmentSeedService
{
    public Task<ApplyDevelopmentSeedResult> ApplyAsync(
        ApplyDevelopmentSeedRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        _ = dbContext;
        var profile = request.Profile.Trim();

        return Task.FromResult(!string.Equals(profile, "minimal-validation", StringComparison.OrdinalIgnoreCase)
            ? ApplyDevelopmentSeedResult.Failure(profile, ["Unsupported development seed profile."])
            : ApplyDevelopmentSeedResult.Success(profile, [
                "Seed profile acknowledged.",
                "No dataset rows are applied in Phase 9Y; extend this profile in later seed tasks."
            ]));
    }
}
