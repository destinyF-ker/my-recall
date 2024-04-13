using FluentValidation;
using MediatR;
using RecAll.Core.List.Domain.Exceptions;

namespace RecAll.Core.List.Api.Application.Behaviors;

/// <summary>
/// 
/// 这是一个全局的验证器，用于验证请求的合法性</br>
/// 虽然可以在 Command 之中这样写来验证请求的合法性：
/// <code>
///  public class CreateTextItemCommand
/// {
///     [Required] public string Content { get; set; }
/// }
/// </code> 
///  
/// 但是这样会导致代码重复，所以这里使用了一个全局的验证器</br>
/// 
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam> <summary>
/// 
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class
    ValidatorBehavior<TRequest, TResponse>
    (
        ILogger<ValidatorBehavior<TRequest, TResponse>> logger,
        IValidator<TRequest>[] validators   // 这个参数之中包含了许多不同的 validator，分别对应不同的 Request
    ) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger = logger;

    // TRequest 是 IRequest<TResponse> 的实现，说明这个类是一个管道行为（中介者模式）
    // 验证都是通过往管道行为之中添加验证器来实现的
    private readonly IValidator<TRequest>[] _validators = validators;

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // 首先把请求的类型名字记录下来
        var typeName = request.GetType().Name;
        _logger.LogInformation("----- Validating command {CommandType}", typeName);

        // 开始检查，首先把所有 validator 都执行一遍，然后把错误信息都记录下来，将错误不为空的情况保留下来
        var failures =
            (await Task.WhenAll
                (
                    _validators
                    .Select(p => p.ValidateAsync(request, cancellationToken))
                )
            )
            .SelectMany(p => p.Errors).Where(p => p != null).ToList();

        // 如果有错误，那么就记录下来，然后抛出异常，退出执行
        if (failures.Any())
        {
            _logger.LogWarning(
                "Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}",
                typeName, request, failures);

            // 在之类抛出的异常会被一个全局异常捕获器捕获到（位于 Liat.Api/Infrustructure/Filters/HttpGlobalExceptionFilter 之中）
            throw new ListDomainException(
                $"Command Validation Errors for type {typeof(TRequest).Name}",
                new ValidationException("Validation exception", failures));
        }

        return await next();
        // ----> 注意这个 next，next 之前的部分是拦截器需要拦截的部分，next 之后是执行完之后，后续的部分
    }
}
