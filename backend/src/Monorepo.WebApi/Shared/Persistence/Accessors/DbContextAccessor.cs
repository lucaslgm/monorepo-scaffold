using AppDbContext = Monorepo.WebApi.Shared.Persistence.Contexts.AppDbContext;

namespace Monorepo.WebApi.Shared.Persistence.Accessors;

public class DbContextAccessor : IDbContextAccessor<AppDbContext>
{
    private AppDbContext? _contexto;

    private bool _disposed;

    public AppDbContext Get() => _contexto ?? throw new InvalidOperationException("Contexto deve ser registrado!");

    public void Register(AppDbContext context)
    {
        _disposed = false;
        _contexto = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Clear() => Dispose(true);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _contexto?.Dispose();
        }

        _contexto = null!;
        _disposed = true;
    }
}
