using RecAll.Infrastructure.EventBus.Events;

namespace RecAll.Infrastructure.EventBus.Abstractions;

/// <summary>
/// 快递站
/// 关于为什么这里只有发的功能而没有收的功能，是因为 dapr 自带了一个消息队列可以替你进行消息的接收
/// 当然也可以替你进行发送 但是这里自己实现发送的功能，提高抽象的层次
/// </summary> <summary>
/// 
/// </summary>
public interface IEventBus
{
    Task PublishAsync(IntegrationEvent @event);
}
