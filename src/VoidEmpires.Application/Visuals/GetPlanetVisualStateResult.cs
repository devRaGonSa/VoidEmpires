namespace VoidEmpires.Application.Visuals;

public sealed record GetPlanetVisualStateResult(
    bool Succeeded,
    PlanetVisualStateDto? VisualState,
    IReadOnlyList<string> Errors)
{
    public static GetPlanetVisualStateResult Success(PlanetVisualStateDto visualState) =>
        new(true, visualState, []);

    public static GetPlanetVisualStateResult Failure(params string[] errors) =>
        new(false, null, errors);
}
