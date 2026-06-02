namespace VoidEmpires.Application.Development;

public sealed record ApplyDevelopmentSeedRequest(string Profile);

public sealed record ApplyDevelopmentSeedResult(
    bool Succeeded,
    string Profile,
    IReadOnlyList<string> AppliedSteps,
    IReadOnlyList<string> Errors)
{
    public static ApplyDevelopmentSeedResult Success(string profile, IReadOnlyList<string> appliedSteps) =>
        new(true, profile, appliedSteps, []);

    public static ApplyDevelopmentSeedResult Failure(string profile, IReadOnlyList<string> errors) =>
        new(false, profile, [], errors);
}

public interface IDevelopmentSeedService
{
    Task<ApplyDevelopmentSeedResult> ApplyAsync(
        ApplyDevelopmentSeedRequest request,
        CancellationToken cancellationToken = default);
}
