namespace VoidEmpires.Application.Research;

public interface IResearchUpgradeService
{
    Task<UpgradeResearchResult> UpgradeAsync(
        UpgradeResearchRequest request,
        CancellationToken cancellationToken = default);
}
