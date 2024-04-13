using MediatR;
using RecAll.Core.List.Domain.AggregateModels.ItemAggregate;

namespace RecAll.Core.List.Domain.AggregateModels.ItemAggregate;

public class ItemDeletedDomainEvent : INotification
{
    public Item Item { get; set; }

    public ItemDeletedDomainEvent(Item item)
    {
        Item = item;
    }
}