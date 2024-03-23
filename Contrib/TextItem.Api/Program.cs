using RecAll.Infrastructure.Infrastructure.Api;
using HealthChecks.UI.Client;
using RecAll.Contrib.TextItem.Api;

var builder = WebApplication.CreateBuilder(args);
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