using System.Diagnostics;
using Monorepo.WebApi.Configurations.Factories;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IO;

namespace Monorepo.WebApi.Configurations.Handlers;

public sealed class GlobalExceptionHandler(
    HttpResponseFactory factory,
    ILogger<GlobalExceptionHandler> logger,
    IWebHostEnvironment env,
    RecyclableMemoryStreamManager streamManager) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        // 1. RASTREABILIDADE (Tempo/OpenTelemetry)
        var activity = Activity.Current;
        var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        activity?.SetTag("exception.type", exception.GetType().Name);
        activity?.SetStatus(ActivityStatusCode.Error, exception.Message);

        // 2. CAPTURA DE CONTEXTO (O que o seu Filter fazia de melhor)
        var requestBody = await GetRawBodyAsync(context.Request);
        var userProps = GetUserProperties(context);

        // 3. LOG ESTRUTURADO (Loki)
        logger.LogError(exception,
            "Falha crítica na API | TraceId: {TraceId} | Path: {Path} | User: {User} | Body: {Body}",
            traceId, context.Request.Path, userProps.Usuario, requestBody);

        // 4. RESPOSTA PADRONIZADA
        var detail = env.IsDevelopment()
            ? exception.ToString()
            : $"Erro interno. Forneça o TraceId para o suporte: {traceId}";

        var problemDetails = factory.CreateProblemDetails(
            StatusCodes.Status500InternalServerError,
            "Erro Inesperado no Servidor",
            detail
        );

        // 5. FINALIZAÇÃO DA RESPOSTA
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        context.Response.Headers.TryAdd("X-Trace-Id", traceId);

        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private async Task<string> GetRawBodyAsync(HttpRequest request)
    {
        try
        {
            request.EnableBuffering();
            await using var stream = streamManager.GetStream();
            await request.Body.CopyToAsync(stream);
            var body = System.Text.Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
            request.Body.Position = 0;
            return body.Length > 2000 ? body[..2000] + "..." : body;
        }
        catch
        {
            return "Não foi possível ler o body";
        }
    }

    private static AnonymousUser GetUserProperties(HttpContext context)
    {
        try
        {
            // return context.GetAuthUserProperties();
            return new AnonymousUser();
        }
        catch
        {
            return new AnonymousUser();
        }
    }

    private sealed record AnonymousUser
    {
        public string SessionID { get; } = null!;
        public string Usuario => "Anonymous";
        public string Nome { get; } = null!;
        public string Email { get; } = null!;
        public string Loja { get; } = null!;
        public string Lojista { get; } = null!;
        public string Agente { get; } = null!;
        public string Grupo { get; } = null!;
        public string Setor { get; } = null!;
        public string Token { get; } = null!;
        public string Processo { get; } = null!;
        public bool Master { get; } = false;
    }
}
