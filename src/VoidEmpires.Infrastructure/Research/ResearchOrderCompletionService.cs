using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Research;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Research;

public sealed class ResearchOrderCompletionService(VoidEmpiresDbContext dbContext) : IResearchOrderCompletionService
{
    public async Task<CompleteResearchOrdersResult> CompleteDueOrdersAsync(
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        if (nowUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Completion date must be UTC.", nameof(nowUtc));
        }

        var dueOrders = await dbContext.ResearchOrders
            .Where(x =>
                (x.Status == ResearchQueueItemStatus.Pending || x.Status == ResearchQueueItemStatus.Active) &&
                x.EndsAtUtc <= nowUtc)
            .OrderBy(x => x.EndsAtUtc)
            .ThenBy(x => x.Sequence)
            .ToListAsync(cancellationToken);

        var completedOrderIds = new List<Guid>();

        foreach (var order in dueOrders)
        {
            var project = await dbContext.ResearchProjects
                .SingleOrDefaultAsync(
                    x => x.CivilizationId == order.CivilizationId && x.ResearchType == order.ResearchType,
                    cancellationToken);

            if (project is null)
            {
                project = ResearchProject.Create(order.CivilizationId, order.ResearchType);
                dbContext.ResearchProjects.Add(project);
            }

            while (project.Level < order.TargetLevel)
            {
                project.Upgrade();
            }

            order.MarkCompleted();
            completedOrderIds.Add(order.Id);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new CompleteResearchOrdersResult(completedOrderIds.Count, completedOrderIds);
    }
}
