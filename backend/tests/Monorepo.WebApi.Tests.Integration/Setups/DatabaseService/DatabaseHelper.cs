using Microsoft.Data.SqlClient;

namespace Monorepo.WebApi.Tests.Integration.Setups.DatabaseService;

public static class DatabaseHelper
{
    public static async Task ExecuteCommand(string sql)
    {
        try
        {
            await using var connection = new SqlConnection(TesteIntegracaoSetup.DataBaseServidor.ConnectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
