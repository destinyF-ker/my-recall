using MediatR;

namespace RecAll.Core.List.Api.Application.Behaviors;

/// <summary>
/// 一个全局的日志类型，有两个泛型参数，一个是请求，一个是响应，和中介者模式有关</br>
/// 这个类实现了 IPipelineBehavior 接口，说明了这个类是一个管道行为</br> 
/// 该 TRequest 必须是 IRequest<TResponse> 的实现</br>
/// 和 CreateListCommand 一样，这里也是一个简单的 DTO，用于传递创建列表的请求。</br> 
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam> <summary>
/// 
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class
    LoggingBehavior<TRequest, TResponse>(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger
    ) : IPipelineBehavior<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>>
        _logger = logger;

    /// <summary>
    /// 有是一个 Handle 方法，该方法之中实现了一个拦截器模式，用于记录日志</br>
    /// 说是拦截器模式，实际上就在请求处理之前和请求处理之后记录日志</br>
    /// 所有请求行为都会直接放行，不会对请求进行任何处理</br> 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // 开始处理
        _logger.LogInformation(
            "----- Handling command {CommandName} ({@Command})",
            request.GetType().Name, request);

        // 继续处理，一般来说，出现 next() 方法，说明这就是一个拦截器管道，注意我这里使用拦截器管道这个词
        //  next 的意思就是下一步
        var response = await next();

        // 处理完成
        _logger.LogInformation(
            "----- Command {CommandName} handled - response: {@Response}",
            request.GetType().Name, response);
        return response;
    }
}
