using MediatR;
using Microsoft.EntityFrameworkCore;
using RecAll.Infrastructure.Ddd.Domain.SeedWork;

namespace RecAll.Infrastructure.Ddd.Infrastructure;

/// <summary>
/// 触发领域事件
/// </summary>
public static class MediatorExtension
{
    // 广播/触发领域事件 
    public static async Task DispatchDomainEventsAsync(this IMediator mediator,
        DbContext context)
    {
        var domainEntities = context.ChangeTracker.Entries<Entity>().Where(p =>
            p.Entity.DomainEvents != null && p.Entity.DomainEvents.Any());
        var domainEvents = domainEntities.SelectMany(p => p.Entity.DomainEvents)
            .ToList();

        domainEntities.ToList().ForEach(domainEntity =>
            domainEntity.Entity.ClearDomainEvents());
        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }
}
