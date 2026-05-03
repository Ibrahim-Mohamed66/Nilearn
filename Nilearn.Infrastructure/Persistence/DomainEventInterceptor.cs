using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Nilearn.Domain.Entities;

namespace Nilearn.Infrastructure.Persistence;

public class DomainEventInterceptor : SaveChangesInterceptor
{
    private readonly IPublisher _publisher;
    private readonly ILogger<DomainEventInterceptor> _logger;

    private const int MaxIterations = 10;

    public DomainEventInterceptor(
        IPublisher publisher,
        ILogger<DomainEventInterceptor> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public override InterceptionResult<int> SavingChanges(
    DbContextEventData eventData,
    InterceptionResult<int> result)
    {
        throw new InvalidOperationException(
            "Use SaveChangesAsync to ensure domain events are published.");
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            await PublishDomainEventsAsync(eventData.Context, cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task PublishDomainEventsAsync(
        DbContext context,
        CancellationToken cancellationToken)
    {
        var iteration = 0;

        while (iteration++ < MaxIterations)
        {
            var domainEntities = context.ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            if (domainEntities.Count == 0)
                break;

            var domainEvents = domainEntities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            _logger.LogInformation(
                "Publishing {Count} domain events (Iteration {Iteration})",
                domainEvents.Count,
                iteration);

            foreach (var entity in domainEntities)
            {
                entity.ClearDomainEvents();
            }

            foreach (var domainEvent in domainEvents)
            {
                try
                {
                    _logger.LogDebug(
                        "Publishing domain event: {EventType}",
                        domainEvent.GetType().Name);

                    await _publisher.Publish(domainEvent, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error publishing domain event: {EventType}",
                        domainEvent.GetType().Name);
                    throw;
                }
            }
        }

        if (iteration >= MaxIterations)
        {
            _logger.LogWarning(
                "Max domain event publishing iterations ({MaxIterations}) reached. Possible infinite loop detected.",
                MaxIterations);
        }
    }
}