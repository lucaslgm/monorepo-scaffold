using System.Text.Json;
using Monorepo.WebApi.Configurations.Middlewares;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi;

namespace Monorepo.WebApi.Configurations.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public void UseHealthChecks()
        {
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var result = new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            description = entry.Value.Description,
                            data = entry.Value.Data
                        })
                    };
                    await JsonSerializer.SerializeAsync(context.Response.Body, result);
                }
            });
        }

        public void UseSwaggerConfigs()
        {
            app.UseSwagger(options =>
            {
                var swaggerApiPath = Environment.GetEnvironmentVariable("PROJECT_NAME");
                if (!string.IsNullOrEmpty(swaggerApiPath))
                {
                    Console.WriteLine($"Running on Kubernetes -> API /{swaggerApiPath}");
                    options.PreSerializeFilters.Add((swagger, _) => { swagger.Servers = [new OpenApiServer { Url = $"/{swaggerApiPath}" }]; });
                }
            });
            app.UseSwaggerUI(ui => { ui.SwaggerEndpoint("v1/swagger.json", "API"); });
        }

        public void UseLogControleMiddleware() => app.UseMiddleware<LogControleMiddleware>();
    }
}
