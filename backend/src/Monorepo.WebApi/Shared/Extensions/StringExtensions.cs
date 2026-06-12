using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Monorepo.WebApi.Shared.Extensions;

[ExcludeFromCodeCoverage]
public static class StringExtensions
{
    extension(string? value)
    {
        /// <summary>
        /// Aplica a máscara de CPF (000.000.000-00).
        /// </summary>
        public string ToCpfMask()
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var cleanCpf = value.Trim();
            if (cleanCpf.Length == 11 && long.TryParse(cleanCpf, out _))
            {
                return $"{cleanCpf[..3]}.{cleanCpf.Substring(3, 3)}.{cleanCpf.Substring(6, 3)}-{cleanCpf.Substring(9, 2)}";
            }

            return cleanCpf;
        }

        /// <summary>
        /// Remove espaços em branco. Retorna string vazia se for nulo.
        /// </summary>
        public string ToTrimmed()
            => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

        /// <summary>
        /// Tenta converter a string para DateTime? testando múltiplos formatos comuns.
        /// </summary>
        public DateTime? ToDateTimeSafe()
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var formatos = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "MM/dd/yyyy", "yyyy/MM/dd" };

            foreach (var formato in formatos)
            {
                if (DateTime.TryParseExact(value, formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out var data))
                {
                    return data;
                }
            }

            return DateTime.TryParse(value, out var dataGenerica) ? dataGenerica : null;
        }

        /// <summary>
        /// Converte um intervalo de strings (Início e Fim) para o formato de banco (MM/dd/yyyy).
        /// Adiciona 1 dia à data final para cobrir integralmente o período no SQL.
        /// </summary>
        public (string Start, string End) ToSqlDateRange(string? dataFinalStr)
        {
            var startDb = string.Empty;
            var endDb = string.Empty;

            // 'value' é a data inicial que chamou o método
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dStart))
                {
                    startDb = dStart.ToString("MM/dd/yyyy");
                }
            }

            if (string.IsNullOrWhiteSpace(dataFinalStr))
            {
                return (startDb, endDb);
            }

            if (DateTime.TryParseExact(dataFinalStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dEnd))
            {
                endDb = dEnd.AddDays(1).ToString("MM/dd/yyyy");
            }

            return (startDb, endDb);
        }
    }
}
