using System.Runtime.CompilerServices;
using Monorepo.Domain.Commons.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Monorepo.WebApi.Shared.Persistence.Extensions;

public static class DbContextExtensions
{
    extension(ModelBuilder builder)
    {
        public void MapearOnDeleteRestrictRelacionamentos()
        {
            foreach (var foreignKey in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        public void SetarColunasETabelasMaiusculaSnakeCase()
        {
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                // Alterar nome das Tabelas
                entity.SetTableName(entity.GetTableName()!.ToScreamingSnakeCase());

                // Alterar nome das colunas
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.Name.ToScreamingSnakeCase());
                }
            }
        }

        public void MapearPropriedadesEsquecidas()
        {
            // Obtem todas as entidades
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                // Obtem as propriedades do tipo string
                var properties = entity.GetProperties().Where(p => p.ClrType == typeof(string));
                foreach (var property in properties)
                {
                    // Obtém o max length configurado para o tipo string para não setar as colunas como NVARCHAR
                    var maxLengthConfigured = property.GetMaxLength();

                    if (maxLengthConfigured.HasValue)
                    {
                        property.SetColumnType($"VARCHAR({maxLengthConfigured.Value})");
                    }
                    else if (string.IsNullOrEmpty(property.GetColumnType()) && !property.GetMaxLength().HasValue)
                    {
                        // Se o tipo da coluna for nulo e não tiver Max Length definido
                        property.SetColumnType("VARCHAR(100)");
                    }
                }
            }
        }

        public void MapearPropriedadesParaDateTime()
        {
            // Obtem todas as entidades
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                // Obtem as propriedades do tipo DateTime
                var properties = entity.GetProperties()
                    .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));

                foreach (var property in properties)
                {
                    // Define o tipo da coluna como DATETIME
                    property.SetColumnType("DATETIME");
                }
            }
        }
    }

    public static IQueryable<T> TagComOrigem<T>(this IQueryable<T> queryable,
        string tag = "",
        [CallerMemberName] string nomeMetodo = "",
        [CallerFilePath] string caminhoArquivo = "",
        [CallerLineNumber] int linha = 0)
    {
        var myTag = string.IsNullOrEmpty(tag)
            ? $"{nomeMetodo} - {caminhoArquivo}:{linha}"
            : $"{tag}{Environment.NewLine}{nomeMetodo} - {caminhoArquivo}:{linha}";

        return queryable.TagWith(myTag);
    }
}
