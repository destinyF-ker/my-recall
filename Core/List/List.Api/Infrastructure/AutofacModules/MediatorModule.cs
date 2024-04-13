using System.Reflection;
using Autofac;
using FluentValidation;
using MediatR;
using RecAll.Core.List.Api.Application.Behaviors;
using RecAll.Core.List.Api.Application.Commands;
using RecAll.Core.List.Api.Application.Validators;
using Module = Autofac.Module;

namespace RecAll.Core.List.Api;

/// <summary>
/// 用于中介者模式注册的类，这个模式是一个行为设计模式，用于减少对象之间的直接通信，将所有的请求都通过中介者进行处理</br>
/// </summary>
public class MediatorModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

        // 相当于把项目之中所有的 RequestHandler 一口气全部都注册了 
        builder.RegisterAssemblyTypes(typeof(CreateListCommand).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(IRequestHandler<,>));

        // 注册所有的 validator，用于验证请求，但是实际上验证请求的功能必须在拦截器之中调用，所以这里还要把拦截器也一起注册进去
        builder.RegisterAssemblyTypes(
                    typeof(CreateListCommandValidator).GetTypeInfo().Assembly
                )
                .Where(p => p.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces();

        builder.RegisterGeneric(typeof(LoggingBehavior<,>))
            .As(typeof(IPipelineBehavior<,>)); // 拦截器模式，这里之用于记录日志，所有进入到 Mediator 系统的请求都会被记录下来s

        builder.RegisterGeneric(typeof(ValidatorBehavior<,>))
            .As(typeof(IPipelineBehavior<,>));

        builder.RegisterGeneric(typeof(TransactionBehavior<,>))
            .As(typeof(IPipelineBehavior<,>));
    }
}
