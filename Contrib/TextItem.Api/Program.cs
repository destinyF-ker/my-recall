using RecAll.Infrastructure.Infrastructure.Api;
using HealthChecks.UI.Client;
using RecAll.Contrib.TextItem.Api;

// Initializes a new instance of the WebApplicationBuilder class with preconfigured defaults.
var builder = WebApplication.CreateBuilder(args); // WebApplicationBuilder

/*
关于WebApplicationBuilder类需要配置的相关属性：
具体可见https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.builder.webapplicationbuilder?view=aspnetcore-8.0

1. Configuration：A collection of configuration providers for the application to compose. This is useful for adding new configuration sources and providers.

2. Environment：Provides information about the web hosting environment an application is running.

3. Host：An IHostBuilder for configuring host specific properties, but not building. To build after configuration, call Build().

4. Services：A collection of services for the application to compose. This is useful for adding user provided or framework provided services.

5. Logging：A collection of logging providers for the application to compose. This is useful for adding new logging providers.

6. Metrics：Allows enabling metrics and directing their output.

7. WebHost：An IWebHostBuilder for configuring server specific properties, but not building. To build after configuration, call Build().

关于Service和Configuration：
在 ASP.NET Core 中，Services 和 Configuration 是两个不同的概念，它们分别对应了依赖注入（Dependency Injection）和配置（Configuration）这两个重要的应用程序设计模式。

Services 是一个服务容器(IServiceCollections)，它包含了应用程序中所有注册的服务。服务是一种可以被其他组件使用的对象，例如数据库上下文、HTTP 客户端、日志记录器等。在 ASP.NET Core 中，你可以通过依赖注入来获取这些服务。例如，你可以在控制器的构造函数中添加一个参数，ASP.NET Core 就会自动为你提供这个服务。
具体参见微软官方文档https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection?view=dotnet-plat-ext-8.0

Configuration 是一个配置容器，它包含了应用程序的配置数据。这个配置数据可以来自多个源，如环境变量、命令行参数、JSON 文件、XML 文件等。你可以通过索引器（如 Configuration["Key"]）来访问这些配置数据。

总的来说，Services 和 Configuration 都是用于存储和管理应用程序的重要信息，但是它们的用途和工作方式是不同的。Services 是用于存储和管理服务的，而 Configuration 是用于存储和管理配置数据的。
*/

builder.AddCustomConfiguration();
builder.AddCustomDatabase();
builder.AddCustomSerilog();
builder.AddInvalidModelStateResponseFactory();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.AddCustomSwagger();

builder.AddCustomHealthChecks();

builder.AddCustomApplicationServices();

// Console.WriteLine(builder.Configuration["ConnectionStrings:TextItemContext"]);
builder.Services.AddDaprClient();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCustomSwagger();
    app.MapGet("/", () => Results.LocalRedirect("~/swagger"));
}

app.UseHttpsRedirection();

app.MapControllers();
app.MapCustomHealthChecks(responseWriter: UIResponseWriter.WriteHealthCheckUIResponse);

app.ApplyDatabaseMigration();

app.Run();