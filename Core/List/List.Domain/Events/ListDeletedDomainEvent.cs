using MediatR;

namespace RecAll.Core.List.Domain.Events;

/// <summary>
/// 领域事件不等于操作，领域事件是对操作的一个记录，是对操作的一个描述
/// 感觉像是一个回调函数，也就是说是“操作”带来一个“事件”但是“操作“不等于“事件” 
/// </summary> <summary>
/// 
/// </summary>
public class ListDeletedDomainEvent : INotification
{
    public AggregateModels.ListAggregate.List List { get; }

    public ListDeletedDomainEvent(AggregateModels.ListAggregate.List list)
    {
        List = list;
    }
}
