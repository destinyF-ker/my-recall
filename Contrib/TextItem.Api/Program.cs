using RecAll.Contrib.TextItem.Api;

var builder = WebApplication.CreateBuilder(args);
builder.AddCustomConfiguration();
builder.AddCustomDatabase();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.AddCustomSwagger();

builder.AddCustomApplicationServices();
builder.Services.AddControllers();

// Console.WriteLine(builder.Configuration["ConnectionStrings:TextItemContext"]);

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

app.ApplyDatabaseMigration();

app.Run();