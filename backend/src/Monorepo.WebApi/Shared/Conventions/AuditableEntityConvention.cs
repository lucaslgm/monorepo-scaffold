using Monorepo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Monorepo.WebApi.Shared.Conventions;

public sealed class AuditableEntityConvention : IEntityTypeAddedConvention
{
    public void ProcessEntityTypeAdded(IConventionEntityTypeBuilder entityTypeBuilder,
        IConventionContext<IConventionEntityTypeBuilder> context)
    {
        if (!typeof(IAuditableEntity).IsAssignableFrom(entityTypeBuilder.Metadata.ClrType))
        {
            return;
        }

        entityTypeBuilder.Property(typeof(string), "Login")?
            .HasColumnName("LOGIN")?
            .IsRequired(true)?
            .HasMaxLength(100);

        entityTypeBuilder.Property(typeof(string), "Maquina")?
            .HasColumnName("MAQUINA")?
            .IsRequired(true)?
            .HasMaxLength(100);

        entityTypeBuilder.Property(typeof(string), "UsuarioAtualizacao")?
            .HasColumnName("USUARIO_ATUALIZACAO")?
            .IsRequired(true)?
            .HasMaxLength(10);

        entityTypeBuilder.Property(typeof(DateTime?), "DataAtualizacao")?
            .HasColumnName("DATA_ATUALIZACAO");
    }
}
