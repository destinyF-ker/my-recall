using Dapr.Extensions.Configuration;
using Dapr.Client;
using RecAll.Contrib.TextItem.Api.Data;
using Microsoft.EntityFrameworkCore;
using Polly;
namespace RecAll.Contrib.TextItem.Api;

public static class ProgramExtensions
{
    public static void AddCustomConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddDaprSecretStore("recall-secretstore", new DaprClientBuilder().Build());
    }

    // public static void AddCustomDatabase(this WebApplicationBuilder builder)
    // {
    //     Console.WriteLine(builder.Configuration["ConnectionString:TextItemContext"]);
    //     builder.Services.AddDbContext<TextItemContext>(
    //         p => p.UseSqlServer(builder.Configuration["ConnectionString:TextItemContext"])
    //     );
    // }
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
}
