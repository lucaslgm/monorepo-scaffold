using Autofac;
using Autofac.Extensions.DependencyInjection;
using Monorepo.WebApi.Shared.Persistence.Contexts;
using Monorepo.WebApi.Tests.Integration.Setups.DatabaseService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Monorepo.WebApi.Shared.Services.Implementations;
using Monorepo.WebApi.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;

namespace Monorepo.WebApi.Tests.Integration.Setups;

public class TesteIntegracaoSetup : WebApplicationFactory<Program>, IAsyncLifetime
{
    private static readonly ILeitorDeArquivos LeitorDeArquivos = new LeitorDeArquivos();
    public static DatabaseServiceSetup DataBaseServidor { get; private set; } = new();
    private static MockacoServiceSetup MockacoServiceSetup { get; set; } = new();

    public HttpClient HttpClient;

    public async Task InitializeAsync()
    {
        await DataBaseServidor.IniciaServidorDbAsync();

        await MockacoServiceSetup.InitializeAsync();

        Server.PreserveExecutionContext = true;

        HttpClient = CreateClient();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureContainer<ContainerBuilder>(container =>
        {
            // var arquivoAnexoServiceMock = new Mock<IArquivoAnexoService>();
            //
            // arquivoAnexoServiceMock
            //     .Setup(service => service.BuscarArquivoOuUrlSasAsync(
            //         It.IsAny<string>(),
            //         It.IsAny<CancellationToken>()))
            //     .ReturnsAsync(ArquivoOuUrlSas.CriarComUrlSas("https://storage.test.local/anexo.pdf?<sas-token>"));
            //
            // container.RegisterInstance(arquivoAnexoServiceMock.Object)
            //     .As<IArquivoAnexoService>()
            //     .SingleInstance();

            // container.RegisterType<ServicoAuthApiMock>()
            //     .As<IServicoAuthApi>()
            // .InstancePerLifetimeScope();
        });

        builder.ConfigureHostConfiguration(config =>
        {
            config.SetBasePath(Directory.GetCurrentDirectory());
            config.AddJsonFile("appsettings.Test.json");
            config.AddEnvironmentVariables();
        });

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddAutofac();

            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(DataBaseServidor.ConnectionString));

            services.AddSingleton<IFilterProvider, SkipExternalFiltersProvider>();
        });
    }

    public new Task DisposeAsync() => Task.CompletedTask;
}

public class SkipExternalFiltersProvider : IFilterProvider
{
    public int Order => -1000; // Executa antes de todos

    public void OnProvidersExecuting(FilterProviderContext context)
    {
        // Remove o filtro específico da lista de execução para este request
        var externalFilters = context.Results
            .Where(f => f.Filter!.GetType().Namespace != null &&
                        f.Filter!.GetType().Namespace!.StartsWith("B.Auth"))
            .ToList();

        foreach (var filter in externalFilters)
        {
            context.Results.Remove(filter);
        }
    }

    public void OnProvidersExecuted(FilterProviderContext context)
    {
    }
}
