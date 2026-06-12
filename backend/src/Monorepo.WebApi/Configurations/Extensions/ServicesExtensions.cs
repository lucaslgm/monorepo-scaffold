using System.IO.Compression;
using System.Reflection;
using System.Text.RegularExpressions;
using Asp.Versioning;
using Monorepo.WebApi.Configurations.Filters;
using Monorepo.WebApi.Configurations.Handlers;
using Monorepo.WebApi.Shared.Persistence.Contexts;
using Monorepo.WebApi.Shared.Services.Implementations;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Monorepo.WebApi.Configurations.Extensions;

internal static class ServicesExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDbContext(IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(SecretConfiguration.GetSecretConfigurationValue("SicredMatoneConnection", configuration));
                options.UseLazyLoadingProxies();
            });

            return services;
        }

        public IServiceCollection AddOpenTelemetry(IConfiguration configuration)
        {
            const string activitySourceName = "Juridico.Inicial.WebApi.Instrumentation";

            var serviceName = Assembly.GetExecutingAssembly().GetName().Name ?? "Juridico.Inicial.WebApi";

            var telemetrySection = configuration.GetSection("TelemetrySettings");

            var logMinimumLevel = telemetrySection.GetSection("LogMinimumLevel").Get<string>() ?? "Information";

            var writeToConsole = telemetrySection.GetSection("WriteToConsole").Get<bool>();

            // var tracing = telemetrySection.GetSection("Tracing").Get<SignalSettings>() ?? new SignalSettings();
            // var logging = telemetrySection.GetSection("Logging").Get<SignalSettings>() ?? new SignalSettings();
            // var metrics = telemetrySection.GetSection("Metrics").Get<SignalSettings>() ?? new SignalSettings();

            // services.UseBemOpenTelemetry(
            //     opts =>
            //     {
            //         MapSettings(opts.Logging, logging);
            //         MapSettings(opts.Tracing, tracing);
            //         MapSettings(opts.Metrics, metrics);
            //
            //         opts.LogMinimumLevel = logMinimumLevel;
            //         opts.WriteToConsole = writeToConsole;
            //     },
            //     builder =>
            //     {
            //         builder.WithTracing(tracer =>
            //             {
            //                 tracer.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName));
            //
            //                 if (tracing.IsEnabled && !string.IsNullOrEmpty(tracing.Endpoint))
            //                 {
            //                     tracer.AddOtlpExporter(o =>
            //                     {
            //                         o.Endpoint = new Uri(tracing.Endpoint);
            //                         o.Protocol = tracing.Protocol;
            //                     });
            //                 }
            //
            //                 tracer
            //                     .AddSource(activitySourceName)
            //                     .AddAspNetCoreInstrumentation()
            //                     .AddHttpClientInstrumentation();
            //             }
            //         );
            //         builder.WithLogging(logger =>
            //         {
            //             logger.AddOpenTelemetry(options =>
            //             {
            //                 options.IncludeFormattedMessage = true;
            //                 options.IncludeScopes = true;
            //                 options.ParseStateValues = true;
            //
            //                 options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName));
            //
            //                 options.AddOtlpExporter(otlpOptions =>
            //                 {
            //                     otlpOptions.Endpoint = new Uri(logging.Endpoint);
            //                     otlpOptions.Protocol = logging.Protocol;
            //                 });
            //             });
            //             logger.AddConsole();
            //         });
            //         builder.WithMetrics(meterBuilder =>
            //         {
            //             meterBuilder
            //                 .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
            //                 .AddMeter("Juridico.Inicial.Infrastructure.Telemetry")
            //                 .AddAspNetCoreInstrumentation()
            //                 .AddRuntimeInstrumentation()
            //                 .AddHttpClientInstrumentation()
            //                 .AddPrometheusExporter();
            //
            //             if (metrics.IsEnabled && !string.IsNullOrEmpty(metrics.Endpoint))
            //             {
            //                 meterBuilder.AddOtlpExporter(o =>
            //                 {
            //                     o.Endpoint = new Uri(metrics.Endpoint);
            //                     o.Protocol = metrics.Protocol;
            //                 });
            //             }
            //         });
            //     }
            // );

            return services;
        }

        public IServiceCollection AddVersioning()
        {
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });
            return services;
        }

        public IServiceCollection AddOpenApiDoc()
        {
            const string serviceName = "Juridico Inicial - Web API";
            const string serviceDescription = "API responsável pela consulta de contratos e obtenção de documentos jurídicos.";
            const string schemeName = "Bearer";

            services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(x => x.ToString());
                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description =
                            "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = schemeName,
                        BearerFormat = "JWT"
                    }
                );

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(schemeName, document)] = []
                });

                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = serviceName,
                        Description = serviceDescription,
                        Version = "v1"
                    }
                );

                options.DocInclusionPredicate((_, apiDesc) =>
                {
                    if (apiDesc.RelativePath != null)
                    {
                        apiDesc.RelativePath =
                            apiDesc.RelativePath.Replace("Endpoint", "", StringComparison.OrdinalIgnoreCase);
                    }

                    return true;
                });
                options.TagActionsBy(api =>
                {
                    if (api.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor)
                    {
                        return [api.GroupName ?? "Default"];
                    }

                    var methodTags = controllerActionDescriptor.MethodInfo.GetCustomAttributes(true)
                        .OfType<ITagsMetadata>()
                        .SelectMany(attr => attr.Tags)
                        .ToList();

                    if (methodTags.Count != 0)
                    {
                        return [.. methodTags];
                    }

                    var classTags = controllerActionDescriptor.ControllerTypeInfo
                        .GetCustomAttributes(true)
                        .OfType<ITagsMetadata>()
                        .SelectMany(x => x.Tags)
                        .ToList();

                    if (classTags.Count != 0)
                    {
                        return [.. classTags];
                    }

                    var controllerName = api.ActionDescriptor.RouteValues["controller"] ?? "Default";

                    var controllerNameFormated = ServicesExtensionsHelper.FormatcontrollerName(controllerName);
                    return [controllerNameFormated];
                });
                options.OrderActionsBy(apiDesc => $"{apiDesc.GroupName ?? apiDesc.RelativePath}");
                options.CustomSchemaIds(type => type.Name);
                options.DocInclusionPredicate((_, _) => true);
                options.DocumentFilter<TagOrderingDocumentFilter>();
                options.EnableAnnotations();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name?.Replace(".HttpService", "")}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath, true);
            });
            return services;
        }

        public IServiceCollection AddCustomCors(IConfiguration configuration)
        {
            services.AddCors(o =>
                o.AddPolicy(
                    "default",
                    builder =>
                    {
                        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

                        if (origins is { Length: > 0 })
                        {
                            builder.WithOrigins(origins);
                        }
                        else
                        {
                            builder.AllowAnyOrigin();
                        }

                        builder.AllowAnyMethod().AllowAnyHeader();
                    }
                )
            );
            return services;
        }

        public IServiceCollection AddHealth()
        {
            var hcBuilder = services.AddHealthChecks();
            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy(), ["ready"]);
            return services;
        }

        public IServiceCollection AddCaching()
        {
            services.AddSingleton<IMemoryCache>(_ => new MemoryCache(new MemoryCacheOptions()));
            return services;
        }

        public IServiceCollection AddCustomMvc()
        {
            var assembly = typeof(Program).Assembly;
            services
                .AddExceptionHandler<GlobalExceptionHandler>()
                .AddProblemDetails()
                .AddControllers(o =>
                {
                    o.Filters.Add<ValidationFilter>();
                    o.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
                })
                .AddApplicationPart(assembly);

            return services;
        }

        public IServiceCollection AddSecurity(IConfiguration configuration)
        {
            /*
             * Adiciona e configura os serviços de autenticação e autorização da aplicação.
             * Atualmente, utiliza a configuração personalizada do BAuth, mas pode ser expandido para incluir outros provedores de autenticação, como JWT ou Microsoft Identity.
             * Este método centraliza a configuração de segurança, facilitando a manutenção e futuras alterações.
             */

            // services
            //     .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //     .AddMicrosoftIdentityWebApi(configuration);

            return services;
        }

        public IServiceCollection AddResponsesCompression()
        {
            services
                .AddResponseCompression(options =>
                {
                    options.EnableForHttps = true;
                    options.Providers.Add<BrotliCompressionProvider>();
                    options.Providers.Add<GzipCompressionProvider>();

                    // Tipos de MIME que você quer comprimir (além dos padrões)
                    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                        ["application/json", "text/plain", "image/svg+xml"]);
                })
                .Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Fastest; })
                .Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.SmallestSize; });

            return services;
        }

        public IServiceCollection AddHttpClients(IConfiguration configuration)
        {
            /*
             * Adiciona e configura clientes HTTP para a aplicação.
             * Utilizado para registrar instâncias nomeadas de HttpClient
             */

            var anexosApiUrl = SecretConfiguration.GetSecretConfigurationValue("HttpClients:AnexosApi", configuration);

            services.AddHttpClient("anexos-api", client =>
            {
                client.BaseAddress = new Uri(anexosApiUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            return services;
        }
    }

    // private static void MapSettings(dynamic target, SignalSettings source)
    // {
    //     target.IsEnabled = source.IsEnabled;
    //     target.Protocol = source.Protocol;
    //     target.Endpoint = source.Endpoint;
    // }
}

public sealed class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        return value == null
            ? null
            : value.ToString()?.Replace("Endpoint", "", StringComparison.OrdinalIgnoreCase)!;
    }
}

public static partial class ServicesExtensionsHelper
{
    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex ControllerNameRegex();

    public static string FormatcontrollerName(string controllerName) =>
        ControllerNameRegex().Replace(controllerName, "$1 $2");
}
