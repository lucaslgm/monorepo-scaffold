namespace Monorepo.WebApi.Shared.Persistence.Accessors;

public interface IDbContextAccessor<T> : IDisposable where T : Microsoft.EntityFrameworkCore.DbContext
{
    void Register(T context);
    T Get();
    void Clear();
}
