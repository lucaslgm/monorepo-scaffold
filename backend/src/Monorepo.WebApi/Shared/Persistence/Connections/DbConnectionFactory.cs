using System.Data;
using System.Diagnostics.CodeAnalysis;
using Monorepo.Domain.Interfaces;
using Microsoft.Data.SqlClient;

namespace Monorepo.WebApi.Shared.Persistence.Connections;

[ExcludeFromCodeCoverage]
public class DbConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection CreateConnection() => new SqlConnection(connectionString);
}
