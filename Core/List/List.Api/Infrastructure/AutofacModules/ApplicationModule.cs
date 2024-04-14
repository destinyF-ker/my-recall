using System.Data.Common;
using Autofac;
using RecAll.Core.List.Api.Application.IntegrationEvents;
using RecAll.Core.List.Api.Application.Queries;
using RecAll.Core.List.Api.Infrastructure.Services;
using RecAll.Core.List.Domain.AggregateModels.ItemAggregate;
using RecAll.Core.List.Domain.AggregateModels.ListAggregate;
using RecAll.Core.List.Domain.AggregateModels.SetAggregate;
using RecAll.Core.List.Infrastructure;
using RecAll.Core.List.Infrastructure.Repositories;
using RecAll.Infrastructure.EventBus;
using RecAll.Infrastructure.EventBus.Abstractions;
using RecAll.Infrastructure.IntegrationEventLog.Services;

namespace RecAll.Core.List.Api.Infrastructure.AutofacModules;

/// <summary>
/// Service 等 接口和实现类的注册
/// </summary>
public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // 注册 A 类为 B 接口的实现
        builder.RegisterType<ListRepository>().As<IListRepository>()
            .InstancePerLifetimeScope();
        builder.RegisterType<SetRepository>().As<ISetRepository>()
           .InstancePerLifetimeScope();
        builder.RegisterType<ItemRepository>().As<IItemRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<MockIdentityService>().As<IIdentityService>()
            .InstancePerLifetimeScope();
        builder.RegisterType<ContribUrlService>().As<IContribUrlService>()
           .InstancePerLifetimeScope();

        builder.RegisterType<ListQueryService>().As<IListQueryService>()
            .InstancePerLifetimeScope();
        builder.RegisterType<SetQueryService>().As<ISetQueryService>()
            .InstancePerLifetimeScope();

        builder.Register<Func<DbConnection, IIntegrationEventLogService>>(_ =>
            connection => new IntegrationEventLogService(connection));
        builder.RegisterType<ListIntegrationEventService>()
            .As<IListIntegrationEventService>().InstancePerLifetimeScope();

        builder.RegisterType<DaprEventBus>().As<IEventBus>()
            .InstancePerLifetimeScope();
    }
}
