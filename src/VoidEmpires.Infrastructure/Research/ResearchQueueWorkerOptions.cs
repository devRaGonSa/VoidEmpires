namespace VoidEmpires.Infrastructure.Research;

public sealed class ResearchQueueWorkerOptions
{
    public const string SectionName = "VoidEmpires:ResearchQueueWorker";

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
