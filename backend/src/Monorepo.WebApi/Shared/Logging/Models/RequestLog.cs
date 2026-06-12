namespace Monorepo.WebApi.Shared.Logging.Models;

public sealed record RequestLog(
    string Body,
    string Ip,
    string QueryString,
    string? User,
    IDictionary<string, string> Headers,
    IEnumerable<ParametroRota> Parameters);
