
using HealthChecks.UI.Client;
using Infrastructure.Api;
using RecAll.Contrib.TextItem.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomConfiguration();
builder.AddCustomSerilog();
builder.AddCustomSwagger();
builder.AddCustomHealthChecks();
builder.AddCustomApplicationServices();
builder.AddCustomDatabase();
builder.AddInvalidModelStateResponseFactory();

builder.Services.AddDaprClient();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
    app.UseCustomSwagger();
    app.MapGet("/", () => Results.LocalRedirect("~/swagger"));
}

app.MapControllers();
app.MapCustomHealthChecks(
    responseWriter: UIResponseWriter.WriteHealthCheckUIResponse);

app.ApplyDatabaseMigration();

app.Run();
