using CSharpFunctionalExtensions;
using System.Reflection;
using Monorepo.WebApi.Configurations.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

Result.Configuration.ErrorMessagesSeparator = "";

builder.Host.ConfigureAutoFac();

builder.Services
    .AddCustomMvc()
    .AddOpenTelemetry(configuration)
    .AddDbContext(configuration)
    .AddEndpointsApiExplorer()
    .AddVersioning()
    .AddOpenApiDoc()
    .AddCustomCors(configuration)
    .AddHealth()
    .AddOptions()
    .AddCaching()
    .AddSecurity(configuration)
    .AddHttpContextAccessor()
    .AddResponsesCompression()
    .AddDistributedMemoryCache()
    .AddHttpClients(configuration);

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Iniciando a aplicação {AppName}", Assembly.GetExecutingAssembly().GetName().Name);

var ignorarBAuth = builder.Configuration.GetValue<bool>("IgnorarBAuth");

app.UseResponseCompression();
app.UseLogControleMiddleware();
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseSwaggerConfigs();
app.UseRouting();
app.UseCors("default");

app.UseHealthChecks();
app.MapControllers();

try
{
    app.Run();
    return 0;
}
catch (Exception ex)
{
    logger.LogCritical(ex, "A aplicação terminou inesperadamente: {Message}", ex.Message);
    return 1;
}
