using DotNet.Testcontainers.Builders;
using Monorepo.WebApi.Shared.Persistence.Contexts;
using Monorepo.WebApi.Tests.Integration.Shared;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace Monorepo.WebApi.Tests.Integration.Setups.DatabaseService;

public sealed class DatabaseServiceSetup
{
    private readonly MsSqlContainer _databaseContainer;
    public string ConnectionString => _databaseContainer.GetConnectionString() + ";MultipleActiveResultSets=True;";

    public DatabaseServiceSetup()
    {
        const string msSqlPassword = "iMpMLmqLlBOKl3n0";
        const int msSqlPort = 2435;

        _databaseContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
            .WithName("juridico-inicial-db-integration-tests")
            .WithPortBinding(msSqlPort, 1433)
            .WithPassword(msSqlPassword)
            .WithEnvironment("TZ", "America/Sao_Paulo")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilInternalTcpPortIsAvailable(1433)
                .UntilMessageIsLogged("SQL Server is now ready for client connections."))
            .Build();
    }

    public async Task IniciaServidorDbAsync()
    {
        await _databaseContainer.StartAsync();
        await Task.Delay(3000);
        Environment.SetEnvironmentVariable("SicredMatoneConnection", ConnectionString);
        Console.WriteLine($"SicredMatoneConnection: {_databaseContainer.GetConnectionString()}");
        await CriarBancoDados();
    }

    public void ConfigureContext(IServiceCollection services)
    {
        var descriptorDbContext = services.SingleOrDefault(d => d.ServiceType == typeof(AppDbContext));

        if (descriptorDbContext is not null)
        {
            services.Remove(descriptorDbContext);
        }

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(ConnectionString);
            options.UseLazyLoadingProxies();
        });
    }

    public static async Task ExecuteMigrationsAsync(AppDbContext dbContext) => await dbContext.Database.MigrateAsync();

    public async Task PararServidorDbAsync() => await _databaseContainer.StopAsync();

    public async Task DisposeAsync() => await _databaseContainer.DisposeAsync();

    private async Task CriarBancoDados()
    {
        var sql = await MetodosUteis.ObterScript("create_tables.sql");

        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }
}
