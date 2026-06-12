using System.Globalization;

namespace Monorepo.WebApi.Shared.Extensions;

public static class NumericExtensions
{
    private static readonly CultureInfo CulturaBr = new("pt-BR");

    extension(decimal? valor)
    {
        /// <summary>
        /// Formata decimal para string com casas decimais (padrão 2). Ex: 10.5 -> "10,50"
        /// </summary>
        public string ToDecimalString(int casasDecimais = 2)
            => valor?.ToString($"N{casasDecimais}", CulturaBr) ?? "0,00";

        /// <summary>
        /// Formata para moeda brasileira (R$).
        /// </summary>
        public string ToCurrency()
            => valor?.ToString("C", CulturaBr) ?? 0.00m.ToString("C", CulturaBr);
    }

    extension(double? valor)
    {
        /// <summary>
        /// Formata double para string com casas decimais.
        /// </summary>
        public string ToDoubleString(int casasDecimais = 2)
            => valor?.ToString($"N{casasDecimais}", CulturaBr) ?? "0,00";
    }
}
