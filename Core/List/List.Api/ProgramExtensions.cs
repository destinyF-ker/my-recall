﻿using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dapr.Client;
using Dapr.Extensions.Configuration;
using RecAll.Infrastructure.Infrastructure.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Polly;
using RecAll.Core.List.Api.Infrastructure.AutofacModules;
// using RecAll.Core.List.Api.Infrastructure.Filters;
using RecAll.Core.List.Infrastructure;
using Serilog;
using TheSalLab.GeneralReturnValues;
using RecAll.Core.List.Api.Infrastructure.Filter;
using RecAll.Infrastructure.IntegrationEventLog;


namespace RecAll.Core.List.Api;

public static class ProgramExtensions
{
    public static readonly string AppName = typeof(ProgramExtensions).Namespace;

    /// <summary>
    /// 解决跨域请求问题
    /// </summary>
    /// <param name="builder"></param> <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    public static void AddCustomCors(this WebApplicationBuilder builder) =>
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.SetIsOriginAllowed(host => true)
                    .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
        });

    /// <summary>
    /// 注册自定义的服务提供者工厂（ApplicationModule 和 MediatorModule）
    /// </summary>
    /// <param name="builder"></param> <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    public static void AddCustomServiceProviderFactory(
        this WebApplicationBuilder builder)
    {
        builder.Host.UseServiceProviderFactory(
            new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
        {
            containerBuilder.RegisterModule(new MediatorModule());
            containerBuilder.RegisterModule(new ApplicationModule());
        });
    }

    /// <summary>
    /// 加入 Dapr 的配置（密码服务）
    /// </summary>
    /// <param name="builder"></param> <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    public static void AddCustomConfiguration(
        this WebApplicationBuilder builder)
    {
        builder.Configuration.AddDaprSecretStore("recall-secretstore",
            new DaprClientBuilder().Build());
    }

    /// <summary>
    /// 日志配置
    /// </summary>
    /// <param name="builder"></param> <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    public static void AddCustomSerilog(this WebApplicationBuilder builder)
    {
        var seqServerUrl = builder.Configuration["Serilog:SeqServerUrl"];

        Log.Logger = new LoggerConfiguration().ReadFrom
            .Configuration(builder.Configuration).WriteTo.Console().WriteTo
            .Seq(seqServerUrl).Enrich.WithProperty("ApplicationName", AppName)
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    public static void AddCustomSwagger(this WebApplicationBuilder builder) =>
        builder.Services.AddSwaggerGen();

    public static void
        AddCustomHealthChecks(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy()).AddDapr()
            .AddSqlServer(
                builder.Configuration["ConnectionStrings:ListContext"]!,
                name: "ListDb-check", tags: new[] { "ListDb" })
            .AddUrlGroup(
                new Uri(builder.Configuration["TextListHealthCheck"]),
                "TextListHealthCheck", tags: new[] { "TextList" });
    }

    public static void AddCustomDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ListContext>(options =>
        {
            options.UseSqlServer(
                builder.Configuration["ConnectionStrings:ListContext"],
                sqlServerOptionsAction =>
                {
                    sqlServerOptionsAction.MigrationsAssembly(
                        typeof(ProgramExtensions).GetTypeInfo().Assembly
                            .GetName().Name);
                    // sqlServerOptionsAction.MigrationsAssembly("RecAll.Core.List.Api");
                    Console.WriteLine(
                        $"---------ConnectionStrings: {builder.Configuration["ConnectionStrings:ListContext"]}");
                    sqlServerOptionsAction.EnableRetryOnFailure(15,
                        TimeSpan.FromSeconds(30), null);
                });
        });

        builder.Services.AddDbContext<IntegrationEventLogContext>(options =>
        {
            options.UseSqlServer(
                builder.Configuration["ConnectionStrings:ListContext"],
                sqlServerOptionsAction =>
                {
                    sqlServerOptionsAction.MigrationsAssembly(
                        typeof(ProgramExtensions).GetTypeInfo().Assembly
                            .GetName().Name);
                    sqlServerOptionsAction.EnableRetryOnFailure(15,
                        TimeSpan.FromSeconds(30), null);
                });
        });
    }

    public static void AddInvalidModelStateResponseFactory(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions().PostConfigure<ApiBehaviorOptions>(
            options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                    new OkObjectResult(ServiceResult
                        .CreateInvalidParameterResult(
                            new ValidationProblemDetails(context.ModelState)
                                .Errors.Select(p =>
                                    $"{p.Key}: {string.Join(" / ", p.Value)}"))
                        .ToServiceResultViewModel());
            });
    }

    public static void
        AddCustomControllers(this WebApplicationBuilder builder) =>
        builder.Services
            .AddControllers(options =>
                options.Filters.Add(typeof(HttpGlobalExceptionFilter)))
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.IncludeFields = true);

    public static void UseCustomSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    public static void UserCustomCors(this WebApplication app)
    {
        app.UseCors("CorsPolicy");
    }

    public static void ApplyDatabaseMigration(this WebApplication app)
    {
        // Apply database migration automatically. Note that this approach is not
        // recommended for production scenarios. Consider generating SQL scripts from
        // migrations instead.
        using var scope = app.Services.CreateScope();

        var retryPolicy = CreateRetryPolicy();
        var listContext =
            scope.ServiceProvider.GetRequiredService<ListContext>();
        var listContextSeedLogger = scope.ServiceProvider
            .GetRequiredService<ILogger<ListContextSeed>>();

        retryPolicy.Execute(() =>
        {
            listContext.Database.Migrate();
            new ListContextSeed().SeedAsync(listContext, listContextSeedLogger)
                .Wait();
        });

        var integrationEventLogContext = scope.ServiceProvider
            .GetRequiredService<IntegrationEventLogContext>();
        retryPolicy.Execute(() =>
        {
            integrationEventLogContext.Database.Migrate();
        });
    }

    private static Policy CreateRetryPolicy()
    {
        return Policy.Handle<Exception>().WaitAndRetryForever(
            sleepDurationProvider: _ => TimeSpan.FromSeconds(5),
            onRetry: (exception, retry, _) =>
            {
                Console.WriteLine(
                    "Exception {0} with message {1} detected during database migration (retry attempt {2})",
                    exception.GetType().Name, exception.Message, retry);
            });
    }
}
