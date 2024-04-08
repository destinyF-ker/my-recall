using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecAll.Core.List.Api.Application.Commands;
using RecAll.Core.List.Api.Infrastructure.Services;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ListController(IIdentityService identityService,
    ILogger<ListController> logger, IMediator mediator)
{
    // 写日志的时候需要身份信息
    private readonly IIdentityService _identityService = identityService;
    private readonly ILogger<ListController> _logger = logger;
    private readonly IMediator _mediator = mediator;

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
        return (await _mediator.Send(command)).ToServiceResultViewModel();
    }
    // 关于为什么这里_mediator.Send(command)方法会放回一个自己定义的 ServiceResult 对象
    // 因为 CreateListCommand 类实现了 IRequest<ServiceResult> 接口，说明了CreateListCommand 返回值的类型

}
