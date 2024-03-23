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

public static class ProgramExtensions
{
    public static readonly string AppName = typeof(ProgramExtensions).Namespace;

    public static void AddCustomConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddDaprSecretStore(
            "recall-secretstore",
            new DaprClientBuilder().Build());
    }

    public static void AddCustomSwagger(this WebApplicationBuilder builder) =>
        builder.Services.AddSwaggerGen();

    // Copy can't pass the compiler check
    public static void
        AddCustomHealthChecks(this WebApplicationBuilder builder) =>
            builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy()) // 检查自己
            .AddDapr()  // 检查Dapr
            .AddSqlServer(builder.Configuration["ConnectionStrings:TextItemContext"]!,
                name: "TextListDb-check", tags: new[] { "TextListDb" }); // 检查SQL Server

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

    public static void UseCustomSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }


    // public static void AddCustomDatabase(this WebApplicationBuilder builder)
    // {
    //     Console.WriteLine(builder.Configuration["ConnectionString:TextItemContext"]);
    //     builder.Services.AddDbContext<TextItemContext>(
    //         p => p.UseSqlServer(builder.Configuration["ConnectionString:TextItemContext"])
    //     );
    // }
    public static void AddCustomApplicationServices(this WebApplicationBuilder bulider)
    {
        bulider.Services.AddScoped<IIdentityService, MockIndentityService>();
    }

    public static void AddCustomDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<TextItemContext>(p =>
            p.UseSqlServer(
                builder.Configuration["ConnectionStrings:TextItemContext"]));
    }

    public static void ApplyDatabaseMigration(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var retryPolicy = CreateRetryPolicy();
        var context =
            scope.ServiceProvider.GetRequiredService<TextItemContext>();

        retryPolicy.Execute(context.Database.Migrate);
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

    // 模型验证, 把不符合要求的模型也转化为一个默认但是符合要求的形式
    public static void AddInvalidModelStateResponseFactory(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions().Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
                new OkObjectResult(ServiceResult
                    .CreateInvalidParameterResult(
                        new ValidationProblemDetails(context.ModelState).Errors
                            .Select(p =>
                                $"{p.Key}: {string.Join(" / ", p.Value)}"))
                    .ToServiceResultViewModel());
        });
    }
}
