using MediatR;
using RecAll.Core.List.Api.Infrastructure.Services;
using RecAll.Core.List.Domain.AggregateModels.ListAggregate;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Application.Commands;

/// <summary>
/// 实现了 IRequestHandler 接口，说明了该 Handler 应该处理什么程序以及它的返回值是什么 
/// </summary>
public class CreateListCommandHandler(IIdentityService identityService,
    IListRepository listRepository) : IRequestHandler<CreateListCommand, ServiceResult>
{
    private readonly IIdentityService _identityService = identityService;

    // 需要专门处理 List 的 Repository
    private readonly IListRepository _listRepository = listRepository;

    /// <summary>
    /// 在 Controller 中调用 Mediator.Send() 方法时，会根据在 IRequestHandler 之中注册的内容调用这个方法</br>
    /// 在这个方法之中实际上就是根据 CreateListCommand 对象之中带有的信息（从前端返回）
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ServiceResult> Handle(CreateListCommand command,
        CancellationToken cancellationToken)
    {
        // 交给 Repository 处理
        _listRepository.Add(new Domain.AggregateModels.ListAggregate.List(
            command.Name, command.TypeId,
            _identityService.GetUserIdentityGuid()));

        // 保存更改，cancellationToken 是一个取消令牌，用于通知任务取消，可能是用户取消，也可能是系统取消
        // SaveEntitiesAsync() 方法的作用是将所有的更改保存到数据库之中
        // 和调用 DbContext.SaveChanges() 方法一样，但是这里是异步的
        return await _listRepository.UnitOfWork.SaveEntitiesAsync(
            cancellationToken)
            ? ServiceResult.CreateSucceededResult()
            : ServiceResult.CreateFailedResult();
    }
}
