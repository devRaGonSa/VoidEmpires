namespace VoidEmpires.Infrastructure.Buildings;

public sealed class ConstructionQueueWorkerOptions
{
    public const string SectionName = "VoidEmpires:ConstructionQueueWorker";

    public bool Enabled { get; set; }

    public int IntervalSeconds { get; set; } = 30;

    public TimeSpan GetInterval()
    {
        if (IntervalSeconds <= 0)
        {
            return TimeSpan.FromSeconds(30);
        }

        return TimeSpan.FromSeconds(IntervalSeconds);
    }
}
