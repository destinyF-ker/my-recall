using MediatR;

namespace RecAll.Core.List.Domain.Events;

/// <summary>
/// 领域事件不等于操作，领域事件是对操作的一个记录，是对操作的一个描述。
/// </summary> <summary>
/// 
/// </summary>
public class ListCreatedDomainEvent : INotification
{
    public AggregateModels.ListAggregate.List List { get; set; }

    public ListCreatedDomainEvent(AggregateModels.ListAggregate.List list)
    {
        List = list;
    }
}
