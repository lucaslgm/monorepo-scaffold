using System.Data;
using System.Diagnostics.CodeAnalysis;
using Autofac.Features.Indexed;
using Monorepo.Domain.Interfaces;

namespace Monorepo.WebApi.Shared.Persistence.Connections;

[ExcludeFromCodeCoverage]
public class DbConnectionFactoryProvider(IIndex<string, IDbConnectionFactory> factories) : IDbConnectionFactoryProvider
{
    public IDbConnection GetConnection(string name) => factories[name].CreateConnection();
}
