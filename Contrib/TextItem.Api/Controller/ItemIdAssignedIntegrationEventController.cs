using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecAll.Contrib.TextItem.Api.Data;
using RecAll.Core.List.Domain.AggregateModels;
using RecAll.Infrastructure.EventBus;

namespace RecAll.Contrib.TextItem.Api.Controllers;

/// <summary>
/// 不是用户调用的控制器，而是由 Dapr 事件总线调用的控制器。
/// </summary>
[ApiController]
[Route("[controller]")]
public class ItemIdAssignedIntegrationEventController
{
    private readonly TextItemContext _textItemContext;

    private readonly ILogger<ItemIdAssignedIntegrationEventController> _logger;

    public ItemIdAssignedIntegrationEventController(
        TextItemContext textItemContext,
        ILogger<ItemIdAssignedIntegrationEventController> logger)
    {
        _textItemContext = textItemContext;
        _logger = logger;
    }

    [Route("itemIdAssigned")]
    [HttpPost]
    [Topic(DaprEventBus.PubSubName, nameof(ItemIdAssignedIntegrationEvent))] // 告诉 AspNetCore 该方法是一个 Dapr 事件总线的订阅者
    public async Task HandleAsync(ItemIdAssignedIntegrationEvent @event)
    {
        if (@event.TypeId != ListType.Text.Id)
        {
            return;
        }

        _logger.LogInformation(
            "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
            @event.Id, ProgramExtensions.AppName, @event);

        var textItem = await _textItemContext.TextItems.FirstOrDefaultAsync(p =>
            p.Id == int.Parse(@event.ContribId));

        if (textItem is null)
        {
            _logger.LogWarning("Unknown TextItem id: {ItemId}", @event.ItemId);
            return;
        }

        textItem.ItemId = @event.ItemId;
        await _textItemContext.SaveChangesAsync();

        _logger.LogInformation(
            "----- Integration event handled: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
            @event.Id, ProgramExtensions.AppName, @event);
    }
}

