using Dapr.Extensions.Configuration;
using Dapr.Client;
using RecAll.Contrib.TextItem.Api.Data;
using Microsoft.EntityFrameworkCore;
using Polly;
using Serilog;
using RecAll.Contrib.TextItem.Api.Services;
using Microsoft.AspNetCore.Mvc;
using TheSalLab.GeneralReturnValues;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RecAll.Infrastructure.Infrastructure.Api;
namespace RecAll.Contrib.TextItem.Api;

///  <summary>
///  ProgameExtensions 类是一个静态类，用于扩展 WebApplicationBuilder 类的功能，为其添加相关配置。
/// </summary>
public static class ProgramExtensions
{
    public static readonly string AppName = typeof(ProgramExtensions).Namespace;


    /// <summary>
    /// Adds custom configuration to the WebApplicationBuilder.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance.</param>
    public static void AddCustomConfiguration(this WebApplicationBuilder builder)
    {
        // Add Dapr secret store configuration
        builder.Configuration.AddDaprSecretStore(
            "recall-secretstore",
            new DaprClientBuilder().Build()); // 这个AddDaprSercretStore也是一个对WebApplicationBuilder的拓展方法
    }

    /// <summary>
    /// Adds custom Swagger configuration to the WebApplicationBuilder.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance.</param>
    public static void AddCustomSwagger(this WebApplicationBuilder builder) =>
        builder.Services.AddSwaggerGen(); // Services是一个集合类型，IServiceCollection接口的实例，实际上就是一个依赖注入容器

    /// <summary>
    /// Adds custom health checks to the WebApplicationBuilder.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance.</param>
    public static void AddCustomHealthChecks(this WebApplicationBuilder builder) =>
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy()) // 检查自己
            .AddDapr() // 检查Dapr，这个方法是自己定义的
            .AddSqlServer(builder.Configuration["ConnectionStrings:TextItemContext"]!,
                name: "TextListDb-check", tags: new[] { "TextListDb" }); // 检查SQL Server

    /// <summary>
    /// Adds custom Serilog configuration to the WebApplicationBuilder.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance.</param>
    public static void AddCustomSerilog(this WebApplicationBuilder builder)
    {
        // seq服务器地址
        var seqServerUrl = builder.Configuration["serilog:SeqServerUrl"];

        Log.Logger = new LoggerConfiguration().ReadFrom
            .Configuration(builder.Configuration)
            .WriteTo.Console()
            .WriteTo.Seq(seqServerUrl)
            .Enrich.WithProperty("ApplicationName", AppName)
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    /// <summary>
    /// Uses custom Swagger configuration in the WebApplication.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    public static void UseCustomSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    /// <summary>
    /// Adds custom application services to the WebApplicationBuilder.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance.</param>
    public static void AddCustomApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IIdentityService, MockIndentityService>();
    }

    /// <summary>
    /// Adds custom database configuration to the WebApplicationBuilder.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance.</param>
    public static void AddCustomDatabase(this WebApplicationBuilder builder)
    {
        // AddDbContext是一个扩展方法，它接收一个配置委托作为参数，这个委托用于配置DbContextOptionsBuilder(p)
        builder.Services.AddDbContext<TextItemContext>(p =>
            p.UseSqlServer(
                builder.Configuration["ConnectionStrings:TextItemContext"])
            );
    }

    /// <summary>
    /// Applies database migration to the WebApplication.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    public static void ApplyDatabaseMigration(this WebApplication app)
    {
        // 这里使用了using语句，这是C#的一种语法，用于自动管理资源的生命周期
        // 当变量离开其作用域时，它的Dispose方法会被自动调用
        using var scope = app.Services.CreateScope();

        var retryPolicy = CreateRetryPolicy();
        var context = scope.ServiceProvider.GetRequiredService<TextItemContext>();

        retryPolicy.Execute(context.Database.Migrate);
    }

    /// <summary>
    /// Creates a retry policy for database migration.
    /// </summary>
    /// <returns>The retry policy.</returns>
    private static Policy CreateRetryPolicy()
    {
        return Policy.Handle<Exception>()
                    .WaitAndRetryForever(
                        sleepDurationProvider: _ => TimeSpan.FromSeconds(5),
                        onRetry: (exception, retry, _) =>
                        {
                            Console.WriteLine(
                                "Exception {0} with message {1} detected during database migration (retry attempt {2})",
                               exception.GetType().Name, exception.Message, retry);
                        }
                    );
    }

    /// <summary>
    /// Adds an invalid model state response factory to the WebApplicationBuilder.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance.</param>
    public static void AddInvalidModelStateResponseFactory(this WebApplicationBuilder builders)
    {
        builder.Services.PostConfigure<ApiBehaviorOptions>(
                            options =>
                            {
                                options.InvalidModelStateResponseFactory = context =>
                                    new OkObjectResult(ServiceResult
                                        .CreateInvalidParameterResult(new ValidationProblemDetails(
                                            context.ModelState).Errors.Select(p => $"{p.Key}: {string.Join(" / ", p.Value)}"))
                                                .ToServiceResultViewModel()
                                    );
                            }
                        );
    }
}
