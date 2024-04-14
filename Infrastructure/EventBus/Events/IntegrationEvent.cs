using System.Text.Json.Serialization;

namespace RecAll.Infrastructure.EventBus.Events;

/// <summary>
/// 最后以 Json 的形式保存，该类的作用是代表一个集成事件（信封）
/// </summary> <summary>
/// 
/// </summary>
public record class IntegrationEvent
{
    [JsonInclude] public Guid Id { get; private init; }

    [JsonInclude] public DateTime CreatedTime { get; private init; }

    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreatedTime = DateTime.Now;
    }

    [JsonConstructor]
    public IntegrationEvent(Guid id, DateTime createdTime)
    {
        Id = id;
        CreatedTime = createdTime;
    }
}
