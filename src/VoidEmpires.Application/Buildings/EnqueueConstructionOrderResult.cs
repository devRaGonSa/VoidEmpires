namespace VoidEmpires.Application.Buildings;

public sealed record EnqueueConstructionOrderResult(
    bool Succeeded,
    Guid? OrderId,
    DateTime? StartsAtUtc,
    DateTime? EndsAtUtc,
    IReadOnlyList<string> Errors)
{
    public static EnqueueConstructionOrderResult Success(
        Guid orderId,
        DateTime startsAtUtc,
        DateTime endsAtUtc)
        => new(true, orderId, startsAtUtc, endsAtUtc, []);

    public static EnqueueConstructionOrderResult Failure(params string[] errors)
        => new(false, null, null, null, errors);
}
