namespace Monorepo.WebApi.Shared.Logging.Models;

public sealed record ResponseLog(int StatusCode, string Body, Dictionary<string, string> Headers);
