using Dapr.Client;
using Microsoft.Extensions.Logging;
using RecAll.Infrastructure.EventBus.Abstractions;
using RecAll.Infrastructure.EventBus.Events;

namespace RecAll.Infrastructure.EventBus;

public class DaprEventBus : IEventBus
{
    // 配置 dapr 事件总线服务的名称
    public const string PubSubName = "recall-pubsub";

    private readonly DaprClient _dapr;
    private readonly ILogger _logger;

    public DaprEventBus(DaprClient dapr, ILogger<DaprEventBus> logger)
    {
        _dapr = dapr;
        _logger = logger;
    }

    /// <summary>
    /// 作为发件人，发布事件
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    public async Task PublishAsync(IntegrationEvent @event)
    {
        // 获取事件的类型名，就像是收快递的时候你的手机号码（代表了关心什么事件“topic”）
        var topicName = @event.GetType().Name;
        // ItemDeletedIntegrationEvent : IntegrationEvent
        // "ItemDeletedIntegrationEvent"，得到类型名的字符串

        _logger.LogInformation(
            "Publishing event {@Event} to {PubsubName}.{TopicName}", @event,
            PubSubName, topicName);

        // We need to make sure that we pass the concrete type to PublishEventAsync,
        // which can be accomplished by casting the event to dynamic. This ensures
        // that all event fields are properly serialized.
        await _dapr.PublishEventAsync(PubSubName, topicName, (object)@event);
    }
}