using System.Diagnostics.CodeAnalysis;

namespace Monorepo.WebApi.Shared.Extensions;

[ExcludeFromCodeCoverage]
public static class DateTimeExtensions
{
    /// <summary>
    /// Formata data para string. Padrão brasileiro: dd/MM/yyyy
    /// </summary>
    public static string ToDisplayDate(this DateTime? data, string formato = "dd/MM/yyyy")
        => data?.ToString(formato) ?? string.Empty;

    /// <summary>
    /// Formata data para string (sobrecarga para DateTime não-nulo)
    /// </summary>
    public static string ToDisplayDate(this DateTime data, string formato = "dd/MM/yyyy")
        => data.ToString(formato);
}
