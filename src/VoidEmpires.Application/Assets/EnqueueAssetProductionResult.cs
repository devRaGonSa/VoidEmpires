namespace VoidEmpires.Application.Assets;

public sealed record EnqueueAssetProductionResult(
    bool Succeeded,
    Guid? OrderId,
    DateTime? StartsAtUtc,
    DateTime? EndsAtUtc,
    IReadOnlyList<string> Errors)
{
    public static EnqueueAssetProductionResult Success(
        Guid orderId,
        DateTime startsAtUtc,
        DateTime endsAtUtc)
        => new(true, orderId, startsAtUtc, endsAtUtc, []);

    public static EnqueueAssetProductionResult Failure(params string[] errors)
        => new(false, null, null, null, errors);
}
