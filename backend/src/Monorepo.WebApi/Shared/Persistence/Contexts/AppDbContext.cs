using Monorepo.Domain.Commons.Entities.Audit;
using Monorepo.WebApi.Shared.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using AuditableEntityConvention = Monorepo.WebApi.Shared.Conventions.AuditableEntityConvention;

namespace Monorepo.WebApi.Shared.Persistence.Contexts;

public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    IHttpContextAccessor httpContextAccessor = null!) : DbContext(options)
{
    public DbSet<EntityExample> Entity { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditProperties();

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);

        modelBuilder.MapearOnDeleteRestrictRelacionamentos();
        modelBuilder.SetarColunasETabelasMaiusculaSnakeCase();
        modelBuilder.MapearPropriedadesEsquecidas();
        modelBuilder.MapearPropriedadesParaDateTime();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Add(_ => new AuditableEntityConvention());
        base.ConfigureConventions(configurationBuilder);
    }

    private void SetAuditProperties()
    {
        var user = httpContextAccessor.HttpContext?.User;
        var login = user?.Identity?.IsAuthenticated == true ? user.Identity.Name : Environment.UserName;
        var maquina = Environment.MachineName;
        var dataAtual = ObterHoraBrasilia();

        var entries = ChangeTracker.Entries<AuditableEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            entry.Entity.Login = login ?? "Sistema";
            entry.Entity.Maquina = maquina;
            entry.Entity.DataAtualizacao = dataAtual;

            var usuarioResumido = login ?? "Sistema";
            entry.Entity.UsuarioAtualizacao = usuarioResumido.Length > 10
                ? usuarioResumido[..10]
                : usuarioResumido;
        }
    }

    private static DateTime ObterHoraBrasilia()
    {
        const string zoneId = "E. South America Standard Time";
        var brasiliaZone = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brasiliaZone);
    }
}

public class EntityExample
{
}
