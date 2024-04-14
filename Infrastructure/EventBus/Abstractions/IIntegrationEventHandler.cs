using RecAll.Infrastructure.EventBus.Events;

namespace RecAll.Infrastructure.EventBus.Abstractions;

/// <summary>
/// 处理收到的快递
/// </summary>
/// <typeparam name="TIntegrationEvent"></typeparam> <summary>
/// 
/// </summary>
/// <typeparam name="TIntegrationEvent"></typeparam>
public interface
    IIntegrationEventHandler<TIntegrationEvent> : IIntegrationEventHandler
    where TIntegrationEvent : IntegrationEvent
{
    Task Handle(TIntegrationEvent @event);
}

public interface IIntegrationEventHandler { }