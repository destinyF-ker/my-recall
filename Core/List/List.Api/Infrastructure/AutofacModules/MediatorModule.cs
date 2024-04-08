using System.Reflection;
using Autofac;
using MediatR;
using RecAll.Core.List.Api.Application.Behaviors;
using RecAll.Core.List.Api.Application.Commands;
using Module = Autofac.Module;

namespace RecAll.Core.List.Api;

public class MediatorModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

        // 相当于把项目之中所有的 RequestHandler 一口气全部都注册了 
        builder.RegisterAssemblyTypes(typeof(CreateListCommand).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(IRequestHandler<,>));

        builder.RegisterGeneric(typeof(LoggingBehavior<,>))
            .As(typeof(IPipelineBehavior<,>)); // 拦截器模式，这里之用于记录日志
    }
}

