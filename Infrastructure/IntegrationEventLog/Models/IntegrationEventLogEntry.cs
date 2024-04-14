using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using RecAll.Infrastructure.EventBus.Events;

namespace RecAll.Infrastructure.IntegrationEventLog.Models;

/// <summary>
/// 将事件转换成 Json 之后再存储到数据库之中
/// 因为一个 Json 之中只包含类的字段，所以需要额外存储事件的类型
/// </summary> <summary>
/// 
/// </summary>
public class IntegrationEventLogEntry
{
    public Guid EventId { get; private set; }

    // 事件类型的全名
    public string EventTypeName { get; private set; }

    // NotMapped 表示不会映射到数据库之中
    [NotMapped] public string EventTypeShortName => EventTypeName.Split('.')?.Last();

    // 集成事件主体
    [NotMapped] public IntegrationEvent IntegrationEvent { get; private set; }

    public EventState State { get; set; }

    public int TimesSent { get; set; }

    public DateTime CreatedTime { get; private set; }

    public string ContentJson { get; private set; }

    // 事务，因为将 Item 保存到数据库之中和将 Item 发送到消息队列之中是一个事务，必须同时成功
    public string TransactionId { get; private set; }

    public IntegrationEventLogEntry DeserializeIntegrationEvent(Type type)
    {
        IntegrationEvent = JsonSerializer.Deserialize(ContentJson, type,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) as
            IntegrationEvent;
        return this;
    }

    private IntegrationEventLogEntry() { }

    public IntegrationEventLogEntry(IntegrationEvent integrationEvent,
        Guid transactionId)
    {
        EventId = integrationEvent.Id;
        CreatedTime = integrationEvent.CreatedTime;
        EventTypeName = integrationEvent.GetType().FullName;
        ContentJson = JsonSerializer.Serialize(integrationEvent,
            integrationEvent.GetType(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = false
            });
        State = EventState.NotPublished;
        TimesSent = 0;
        TransactionId = transactionId.ToString();
    }
}
