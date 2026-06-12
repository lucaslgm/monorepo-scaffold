using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Monorepo.WebApi.Configurations.Filters;

/// <summary>
/// Filtro para ordenar as tags no documento Swagger.
/// </summary>
public sealed class TagOrderingDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// Aplica o filtro ao documento Swagger, ordenando as tags.
    /// </summary>
    /// <param name="swaggerDoc">O documento Swagger.</param>
    /// <param name="context">O contexto do filtro do documento.</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags = new List<OpenApiTag>
            {
                // Adicione as tags conforme necessário
                 new() { Name = "Juridico Inicial - Processos", Description = "Operações relacionadas à gestão e acompanhamento de processos jurídicos." },

            }
            .OrderBy(tag => tag.Name)
            .ToHashSet();
    }
}
