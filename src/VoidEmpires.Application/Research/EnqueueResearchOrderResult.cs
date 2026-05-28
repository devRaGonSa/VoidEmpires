namespace VoidEmpires.Application.Research;

public sealed record EnqueueResearchOrderResult(
    bool Succeeded,
    Guid? OrderId,
    DateTime? StartsAtUtc,
    DateTime? EndsAtUtc,
    IReadOnlyList<string> Errors)
{
    public static EnqueueResearchOrderResult Success(
        Guid orderId,
        DateTime startsAtUtc,
        DateTime endsAtUtc)
        => new(true, orderId, startsAtUtc, endsAtUtc, []);

    public static EnqueueResearchOrderResult Failure(params string[] errors)
        => new(false, null, null, null, errors);
}
