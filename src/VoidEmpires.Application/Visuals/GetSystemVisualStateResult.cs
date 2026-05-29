namespace VoidEmpires.Application.Visuals;

public sealed record GetSystemVisualStateResult(
    bool Succeeded,
    SystemVisualStateDto? VisualState,
    IReadOnlyList<string> Errors)
{
    public static GetSystemVisualStateResult Success(SystemVisualStateDto visualState) =>
        new(true, visualState, []);

    public static GetSystemVisualStateResult Failure(params string[] errors) =>
        new(false, null, errors);
}
