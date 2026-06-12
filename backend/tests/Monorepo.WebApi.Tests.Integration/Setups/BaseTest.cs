using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Monorepo.WebApi.Shared.Persistence.Contexts;
using Respawn;

namespace Monorepo.WebApi.Tests.Integration.Setups;

[Collection("Teste Integracao Collection")]
public abstract class BaseTest : IClassFixture<TesteIntegracaoSetup>, IDisposable
{
    protected readonly TesteIntegracaoSetup Setup;
    protected readonly AppDbContext DbContext;

    private readonly Respawner _checkpoint = RespawnServiceSetup.Inicializa();
    private readonly IServiceScope _scope;

    protected BaseTest(TesteIntegracaoSetup factory)
    {
        Setup = factory;
        _scope = factory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    protected async Task ExcluirDadosBancoDeDados()
    {
        await using var conexao = new SqlConnection(TesteIntegracaoSetup.DataBaseServidor.ConnectionString);
        await conexao.OpenAsync();
        await _checkpoint.ResetAsync(conexao);
    }

    public void Dispose()
    {
        _scope.Dispose();
        GC.SuppressFinalize(this);
    }
}
