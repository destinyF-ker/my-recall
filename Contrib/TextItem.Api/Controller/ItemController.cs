using Microsoft.AspNetCore.Mvc;
using RecAll.Contrib.TextItem.Api.Data;
using RecAll.Contrib.TextItem.Api.Service;
using RecAll.Contrib.TextItem.Api.Models;
using Microsoft.EntityFrameworkCore;
using TheSalLab.GeneralReturnValues;
using RecAll.Contrib.TextItem.Api.ViewModels;
using System.Collections;
using RecAll.Contrib.TextItem.Api.Commands;
using RecAll.Contrib.TextItem.Api.Services;

namespace RecAll.Contrib.TextItem.Api.Controller;

[ApiController]
[Route("[controller]")]
public class ItemController
{
    private readonly IIdentityService _identityService;
    private readonly TextItemContext _textItemContext;
    private readonly ILogger<ItemController> _logger;

    public ItemController(IIdentityService identityService,
                            TextItemContext textItemContext,
                            ILogger<ItemController> logger)
    {
        _identityService = identityService;
        _textItemContext = textItemContext;
        _logger = logger;
    }

    [Route("create")]
    [HttpPost]
    public async Task<ActionResult<ServiceResultViewModel<string>>> CreateAsync(
           [FromBody] CreateTextItemCommand command)
    {
        _logger.LogInformation(
            "----- Handling command {CommandName} ({@Command})",
            command.GetType().Name, command
        );

        var textItem = new Models.TextItem
        {
            Content = command.Content,
            UserIdentityGuid = _identityService.GetUserIdentityGuid(),
            IsDeleted = false,
        };

        var textItemEntity = _textItemContext.Add(textItem);
        await _textItemContext.SaveChangesAsync();

        _logger.LogInformation(
            "----- Command {CommandName} handled",
            command.GetType().Name
        );

        // return textItemEntity.Entity.Id.ToString();
        return ServiceResult<string>
            .CreateSucceededResult(textItemEntity.Entity.Id.ToString())
            .ToServiceResultViewModel();
    }

    [Route("update")]
    [HttpPost]
    public async Task<ServiceResultViewModel> UpdateAsync(
        [FromBody] UpdateTextItemCommand command
    )
    {
        _logger.LogInformation(
            "----- Handling command {CommandName} ({@Command})",
            command.GetType().Name, command
        );

        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var textItem = await _textItemContext.TextItems.FirstOrDefaultAsync(p =>
            p.Id == command.Id &&
            p.UserIdentityGuid == userIdentityGuid &&
            !p.IsDeleted);

        if (textItem is null)
        {
            _logger.LogWarning(
                $"用户{userIdentityGuid}尝试查看已删除，不存在或不属于自己的TextItem {command.Id}"
            );

            // return new BadRequestResult();
            return ServiceResult
                .CreateFailedResult($"Unknow TextItem Id: {command.Id}")
                .ToServiceResultViewModel();
        }

        textItem.Content = command.Content;
        await _textItemContext.SaveChangesAsync();

        _logger.LogInformation(
            "----- Command {CommandName} handled",
            command.GetType().Name
        );

        // return new OkResult();
        return ServiceResult
            .CreateSucceededResult()
            .ToServiceResultViewModel();
    }

    [Route("get/{id}")]
    [HttpGet]
    public async Task<ActionResult<ServiceResultViewModel<TextItemViewModel>>> GetAsync(int id)
    {
        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var textItem = await _textItemContext.TextItems.FirstOrDefaultAsync(p =>
            p.Id == id &&
            p.UserIdentityGuid == userIdentityGuid &&
            !p.IsDeleted
        );

        if (textItem is null)
        {
            _logger.LogWarning(
                $"用户{userIdentityGuid}尝试查看已经删除，不存在或不属于自己的TextItem {id}"
            );

            // return ServiceResult<TextItemViewModel>
            //     .CreateFailedResult($"Unknown TextItem id: {id}")
            //     .ToServiceResultViewModel();
        }

        // return textItem is null ? new BadRequestResult() : textItem;
        return textItem is null
            ? ServiceResult<TextItemViewModel>
                .CreateFailedResult($"Unkown TextItem id: {id}")
                .ToServiceResultViewModel()
            : ServiceResult<TextItemViewModel>
                .CreateSucceededResult(new TextItemViewModel
                {
                    Id = textItem.Id,
                    ItemId = textItem.ItemId,
                    Context = textItem.Content,
                }).ToServiceResultViewModel();
    }

    [Route("getByItemId/{itemId}")]
    [HttpGet]
    public async Task<ActionResult<ServiceResultViewModel<TextItemViewModel>>> GetByItemId(int itemId)
    {
        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var textItem = await _textItemContext.TextItems.FirstOrDefaultAsync(p =>
            p.ItemId == itemId &&
            p.UserIdentityGuid == userIdentityGuid &&
            !p.IsDeleted
        );

        if (textItem is null)
        {
            _logger.LogWarning(
                $"用户{userIdentityGuid}尝试查看已经删除，不存在或不属于字节的TextItem, ItemId: {itemId}"
            );
        }

        // return textItem is null ? new BadRequestResult() : textItem;
        return textItem is null
            ? ServiceResult<TextItemViewModel>
                .CreateFailedResult($"Unkown TextItem's ItemId: {itemId}")
                .ToServiceResultViewModel()
            : ServiceResult<TextItemViewModel>
                .CreateSucceededResult(new TextItemViewModel
                {
                    Id = textItem.Id,
                    ItemId = textItem.ItemId,
                    Context = textItem.Content
                }).ToServiceResultViewModel();
    }

    [Route("getItems")]
    [HttpPost]
    public async Task<ActionResult
                    <ServiceResultViewModel
                    <IEnumerable
                    <TextItemViewModel>>>> GetItemsAsync(
        GetItemsCommand command
    )
    {
        var itemIds = command.Ids.ToList();
        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var textItems = await _textItemContext.TextItems.Where(p =>
            p.ItemId.HasValue &&
            itemIds.Contains(p.ItemId.Value) &&
            p.UserIdentityGuid == userIdentityGuid &&
            !p.IsDeleted
        ).ToListAsync();

        if (textItems.Count != itemIds.Count)
        {
            var missingIds = string.Join(",",
                itemIds.Except(textItems.Select(p => p.ItemId.Value))
                    .Select(p => p.ToString()));

            _logger.LogWarning($"用户{userIdentityGuid}尝试查看已经删除，不存在或不属于字节的TextItems: {missingIds}");

            // return new BadRequestResult();

            return ServiceResult<IEnumerable<TextItemViewModel>>
                .CreateFailedResult($"Unknow Item id: {missingIds}")
                .ToServiceResultViewModel();
        }

        textItems.Sort((x, y) =>
            itemIds.IndexOf(x.ItemId.Value) - itemIds.IndexOf(y.ItemId.Value));

        // return textItems;

        return ServiceResult<IEnumerable<TextItemViewModel>>
            .CreateSucceededResult(textItems.Select(p => new TextItemViewModel
            {
                Id = p.Id,
                ItemId = p.ItemId,
                Context = p.Content
            })).ToServiceResultViewModel();
    }
}
