using Server.Client.Services;
using Server.Core.gRPC.Client;
using Server.Core.Services;

var builder = WebApplication.CreateBuilder(args);
BuildConfig(builder.Configuration);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddSingleton<ISqliteService, SqliteService>();
builder.Services.AddSingleton<INormalizeDataService, NormalizeDataService>();
builder.Services.AddSingleton<IDomainClientDataService, DomainClientDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<PredictConsumptionService>();
app.MapGrpcService<ClientService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

static void BuildConfig(IConfigurationBuilder builder)
{
    builder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables();
}
