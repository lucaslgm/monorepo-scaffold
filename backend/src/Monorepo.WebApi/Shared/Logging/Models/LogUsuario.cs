namespace Monorepo.WebApi.Shared.Logging.Models;

public sealed record LogUsuario
{
    public string Id { get; init; } = null!;
    public RequestLog Request { get; init; } = null!;
    public ResponseLog Response { get; init; } = null!;
}
