using System.Data;

namespace Monorepo.Domain.Interfaces;

public interface IDbConnectionFactoryProvider
{
    IDbConnection GetConnection(string name);
}
