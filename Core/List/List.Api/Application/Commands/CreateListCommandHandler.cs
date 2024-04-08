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

    public async Task<ServiceResult> Handle(CreateListCommand command,
        CancellationToken cancellationToken)
    {
        _listRepository.Add(new Domain.AggregateModels.ListAggregate.List(
            command.Name, command.TypeId,
            _identityService.GetUserIdentityGuid()));

        // 保存更改，cancellationToken 是一个取消令牌，用于通知任务取消，可能是用户取消，也可能是系统取消
        return await _listRepository.UnitOfWork.SaveEntitiesAsync(
            cancellationToken)
            ? ServiceResult.CreateSucceededResult()
            : ServiceResult.CreateFailedResult();
    }
}
