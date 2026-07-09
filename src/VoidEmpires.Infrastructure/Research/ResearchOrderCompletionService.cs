using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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

        await using var transaction = await BeginCompletionTransactionAsync(cancellationToken);
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
            if (!await TryClaimOrderAsync(order, nowUtc, cancellationToken))
            {
                continue;
            }

            var project = await dbContext.ResearchProjects
                .Where(x => x.CivilizationId == order.CivilizationId && x.ResearchType == order.ResearchType)
                .OrderByDescending(x => x.Level)
                .FirstOrDefaultAsync(cancellationToken);

            if (project is null)
            {
                project = ResearchProject.Create(order.CivilizationId, order.ResearchType);
                dbContext.ResearchProjects.Add(project);
            }

            while (project.Level < order.TargetLevel)
            {
                project.Upgrade();
            }

            completedOrderIds.Add(order.Id);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (transaction is not null) await transaction.CommitAsync(cancellationToken);

        return new CompleteResearchOrdersResult(completedOrderIds.Count, completedOrderIds);
    }

    private async Task<IDbContextTransaction?> BeginCompletionTransactionAsync(CancellationToken cancellationToken) =>
        dbContext.Database.IsRelational()
            ? await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken)
            : null;

    private async Task<bool> TryClaimOrderAsync(
        ResearchOrder order,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        if (!dbContext.Database.IsRelational())
        {
            if (!order.IsOpen || order.EndsAtUtc > nowUtc) return false;
            order.MarkCompleted();
            return true;
        }

        var updated = await dbContext.ResearchOrders
            .Where(x =>
                x.Id == order.Id &&
                (x.Status == ResearchQueueItemStatus.Pending || x.Status == ResearchQueueItemStatus.Active) &&
                x.EndsAtUtc <= nowUtc)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(x => x.Status, ResearchQueueItemStatus.Completed),
                cancellationToken);

        return updated == 1;
    }
}
