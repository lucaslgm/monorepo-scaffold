using System.Text.RegularExpressions;

namespace Monorepo.Domain.Commons.Extensions;

public static partial class StringExtensions
{
    public static string ToScreamingSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var startUnderscores = StartUnderscoresRegex().Match(input);
        return startUnderscores + TextoUnderscoreRegex().Replace(input, "$1_$2").ToUpper();
    }

    [GeneratedRegex(@"([a-z0-9])([A-Z])")]
    private static partial Regex TextoUnderscoreRegex();

    [GeneratedRegex(@"^_+")]
    private static partial Regex StartUnderscoresRegex();
}
