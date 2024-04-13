using MediatR;
using Microsoft.EntityFrameworkCore;
using RecAll.Core.List.Infrastructure;
using Serilog.Context;

namespace RecAll.Core.List.Api.Application.Behaviors;

/// <summary>
/// 事务行为，用于处理事务</br>
/// 在之前的开发之中有可能会遇到不知道事务应该放在哪里的情况，但是使用拦截器模式就可以很好很方便地处理这个问题</br>
/// 在所有的数据库操作开始之前将其打开，然后在所有的数据库操作结束之后将其关闭</br>
/// 但是和文件操作一样，可能会出现打开了就忘记关闭的情况，所以这里使用了一个拦截器模式，用于处理这个问题</br>
/// 使用拦截器模式：在 Command 到达 Handler 之前将事务打开，然后在 Handler 执行完毕之后将事务关闭</br>
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam> <summary>
/// 
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class TransactionBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    // 关于 DbContext 这里有一个特点，关于我们在管道的这一部分开启了事务，那么在管道的下一个部分此事务还有用吗？
    // 答案是有用：对于一个 DbContext，在一个 HTTP 请求之中是单例的
    private readonly ListContext _listContext;

    // private readonly IListIntegrationEventService _listIntegrationEventService;

    public TransactionBehavior(ListContext listContext,
        // IListIntegrationEventService listIntegrationEventService,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _listContext = listContext;
        // _listIntegrationEventService = listIntegrationEventService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = default(TResponse);
        var typeName = request.GetType().Name;

        try
        {
            if (_listContext.HasActiveTransaction)
            {
                return await next();
            }

            // 打开事务
            var strategy = _listContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                Guid transactionId;
                await using var transaction =
                    await _listContext.BeginTransactionAsync();
                using (LogContext.PushProperty("TransactionContext",
                           transaction.TransactionId))
                {
                    _logger.LogInformation(
                        "----- Begin transaction {TransactionId} for {CommandName} ({@Command})",
                        transaction.TransactionId, typeName, request);
                    // 执行 Handler
                    response = await next();
                    _logger.LogInformation(
                        "----- Commit transaction {TransactionId} for {CommandName}",
                        transaction.TransactionId, typeName);

                    // 提交事务
                    await _listContext.CommitTransactionAsync(transaction);
                    // transactionId = transaction.TransactionId;
                }

                // await _listIntegrationEventService.PublishEventsAsync(
                //     transactionId);
            });

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "ERROR Handling transaction for {CommandName} ({@Command})",
                typeName, request);
            throw;
        }
    }
}
