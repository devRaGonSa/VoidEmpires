namespace VoidEmpires.Domain.Fleets;

public sealed class OrbitalTravelEstimate
{
    private readonly OrbitalTravelResourceCost[] resourceCosts;

    public OrbitalTravelEstimate(
        int abstractDistanceUnits,
        TimeSpan estimatedDuration,
        IEnumerable<OrbitalTravelResourceCost> resourceCosts)
    {
        if (abstractDistanceUnits <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(abstractDistanceUnits));
        }

        AbstractDistanceUnits = abstractDistanceUnits;
        EstimatedDuration = estimatedDuration;
        this.resourceCosts = resourceCosts.ToArray();
    }

    public int AbstractDistanceUnits { get; }

    public TimeSpan EstimatedDuration { get; }

    public IReadOnlyList<OrbitalTravelResourceCost> ResourceCosts => Array.AsReadOnly(resourceCosts);
}
