using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecAll.Core.List.Api.Application.Commands;
using RecAll.Core.List.Api.Application.Queries;
using RecAll.Core.List.Api.Infrastructure.Services;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ListController(
    IIdentityService identityService,
    ILogger<ListController> logger,
    IMediator mediator,
    IListQueryService listQueryService
    )
{
    // 写日志的时候需要身份信息
    private readonly IIdentityService _identityService = identityService;
    private readonly ILogger<ListController> _logger = logger;
    private readonly IMediator _mediator = mediator;
    private readonly IListQueryService _listQueryService = listQueryService;

    /// <summary>
    /// 逻辑如下：
    /// 1. 服务器通过网络接收到一个 CreateListCommand 对象<br>
    /// 2. Contoller 记录日志 <br>
    /// 3. Controller 通过 Mediator 发送 CreateListCommand 对象给 CreateListCommandHandler 对象进行处理 <br>
    /// 4. CreateListCommandHandler 处理 CreateListCommand 对象(最终返回成功或失败)，返回一个 ServiceResult 对象 <br>
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [Route("create")]
    [HttpPost]
    public async Task<ActionResult<ServiceResultViewModel>> CreateAsync(
        [FromBody] CreateListCommand command)
    {
        _logger.LogInformation(
            "----- Sending command: {CommandName} - UserIdentityGuid: {userIdentityGuid} ({@Command})",
            command.GetType().Name, _identityService.GetUserIdentityGuid(),
            command);
        // 在这里实际上 Controller 仅仅是在写了一个日志之后将请求转发给了 Mediator，然后 Mediator 会将请求转发给对应的 Handler
        return (await _mediator.Send(command)).ToServiceResultViewModel();
    }
    // 关于为什么这里_mediator.Send(command)方法会放回一个自己定义的 ServiceResult 对象
    // 因为 CreateListCommand 类实现了 IRequest<ServiceResult> 接口，说明了CreateListCommand 返回值的类型

    [Route("update")]
    [HttpPost]
    public async Task<ActionResult<ServiceResultViewModel>> UpdateAsync(
       [FromBody] UpdateListCommand command)
    {
        _logger.LogInformation(
            "----- Sending command: {CommandName} - UserIdentityGuid: {userIdentityGuid} ({@Command})",
            command.GetType().Name, _identityService.GetUserIdentityGuid(),
            command);

        return (await _mediator.Send(command)).ToServiceResultViewModel();
    }

    [Route("delete")]
    [HttpPost]
    public async Task<ActionResult<ServiceResultViewModel>> DeleteAsync(
      [FromBody] DeleteListCommand command)
    {
        _logger.LogInformation(
            "----- Sending command: {CommandName} - UserIdentityGuid: {userIdentityGuid} ({@Command})",
            command.GetType().Name, _identityService.GetUserIdentityGuid(),
            command);

        return (await _mediator.Send(command)).ToServiceResultViewModel();
    }

    [Route("list")]
    [HttpGet]
    public async
       Task<ActionResult<
           ServiceResultViewModel<(IEnumerable<ListViewModel>, int)>>>
       ListAsync(int skip = 0, int take = 20) =>
       ServiceResult<(IEnumerable<ListViewModel>, int)>
           .CreateSucceededResult(await _listQueryService.ListAsync(skip, take,
               _identityService.GetUserIdentityGuid()))
           .ToServiceResultViewModel();


    [Route("listByTypeId")]
    [HttpGet]
    public async
        Task<ActionResult<
            ServiceResultViewModel<(IEnumerable<ListViewModel>, int)>>>
        ListByTypeId([Required] int typeId, int skip = 0, int take = 20) =>
        ServiceResult<(IEnumerable<ListViewModel>, int)>
            .CreateSucceededResult(await _listQueryService.ListAsync(typeId,
                skip, take, _identityService.GetUserIdentityGuid()))
            .ToServiceResultViewModel();

    [Route("get/{id}")]
    [HttpGet]
    public async Task<ActionResult<ServiceResultViewModel<ListViewModel>>>
        GetAsync([Required] int id)
    {
        var listViewModel = await _listQueryService.GetAsync(id,
            _identityService.GetUserIdentityGuid());

        if (listViewModel is null)
        {
            _logger.LogWarning(
                $"用户{_identityService.GetUserIdentityGuid()}尝试查看已删除、不存在或不属于自己的List {id}");
            return ServiceResult<ListViewModel>
                .CreateFailedResult(ErrorMessage
                    .NotFoundOrDeletedOrIdentityMismatch)
                .ToServiceResultViewModel();
        }

        return ServiceResult<ListViewModel>.CreateSucceededResult(listViewModel)
            .ToServiceResultViewModel();
    }
}
