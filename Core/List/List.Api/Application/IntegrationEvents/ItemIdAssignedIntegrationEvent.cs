using RecAll.Infrastructure.EventBus.Events;

namespace RecAll.Core.List.Api.Application.IntegrationEvents;

public record class ItemIdAssignedIntegrationEvent(
    int TypeId,
    string ContribId,
    int ItemId) : IntegrationEvent;