using Monorepo.Domain.Commons.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Monorepo.WebApi.Shared.Persistence.Mappings;

public class AuditableMapping : IEntityTypeConfiguration<AuditableEntity>
{
    public void Configure(EntityTypeBuilder<AuditableEntity> builder)
    {
        // Aqui você "traduz" o domínio para o banco legado
        builder.Property(e => e.UsuarioAtualizacao)
            .HasColumnName("USUARIO_ATUALIZACAO")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(e => e.DataAtualizacao)
            .HasColumnName("DATA_ATUALIZACAO")
            .HasColumnType("datetime");

        builder.Property(e => e.Maquina)
            .HasColumnName("MAQUINA")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Login)
            .HasColumnName("LOGIN")
            .HasMaxLength(100)
            .IsRequired();
    }
}
