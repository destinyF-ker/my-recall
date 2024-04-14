using RecAll.Infrastructure.EventBus.Events;

namespace RecAll.Contrib.TextItem.Api;

public record class ItemIdAssignedIntegrationEvent(
    int ItemId,
    int TypeId,
    string ContribId) : IntegrationEvent;
