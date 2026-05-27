namespace VoidEmpires.Domain.Research;

public sealed class ResearchProject
{
    private ResearchProject() { }

    private ResearchProject(Guid civilizationId, ResearchType researchType)
    {
        if (civilizationId == Guid.Empty)
        {
            throw new ArgumentException("Civilization id is required.");
        }

        Id = Guid.NewGuid();
        CivilizationId = civilizationId;
        ResearchType = researchType;
        Level = 1;
    }

    public Guid Id { get; private set; }
    public Guid CivilizationId { get; private set; }
    public ResearchType ResearchType { get; private set; }
    public int Level { get; private set; }

    public static ResearchProject Create(Guid civilizationId, ResearchType researchType)
        => new(civilizationId, researchType);

    public void Upgrade()
    {
        Level++;
    }
}
