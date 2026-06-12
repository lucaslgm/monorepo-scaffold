using Respawn;

namespace Monorepo.WebApi.Tests.Integration.Setups;

public static class RespawnServiceSetup
{
    private static readonly RespawnerOptions Options = new() { TablesToIgnore = ["__EFMigrationsHistory"] };

    public static Respawner Inicializa()
    {
        using var conexao = new Microsoft.Data.SqlClient.SqlConnection(
            TesteIntegracaoSetup.DataBaseServidor.ConnectionString
        );

        conexao.Open();

        var respawner = Respawner.CreateAsync(
            conexao,
            Options
        ).GetAwaiter().GetResult();

        return respawner;
    }
}
