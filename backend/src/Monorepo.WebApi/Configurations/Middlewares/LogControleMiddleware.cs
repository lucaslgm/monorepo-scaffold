using System.Diagnostics;
using Monorepo.WebApi.Shared.Logging.Models;
using Monorepo.WebApi.Shared.Services.Interfaces;

namespace Monorepo.WebApi.Configurations.Middlewares;

public class LogControleMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ILogControleService logControleService)
    {
        var request = context.Request;
        var response = context.Response;
        var rota = request.Path.Value?.ToLower() ?? "";

        if (rota.Contains("/swagger", StringComparison.OrdinalIgnoreCase) ||
            rota.Contains("/index.html") ||
            rota.Contains("favicon.ico"))
        {
            await next(context);
            return;
        }

        var traceId = Activity.Current?.TraceId.ToHexString() ?? context.TraceIdentifier;


        var queryParams = request.QueryString.HasValue
            ? request.QueryString.Value
            : string.Empty;

        request.EnableBuffering();
        var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
        request.Body.Position = 0;

        var originalBodyStream = response.Body;
        using var responseBodyStream = new MemoryStream();
        response.Body = responseBodyStream;

        string entradaFinal;

        if (!string.IsNullOrWhiteSpace(requestBody) && !string.IsNullOrWhiteSpace(queryParams))
        {
            entradaFinal = $"[Query]: {queryParams} | [Body]: {requestBody}";
        }
        else
        {
            entradaFinal = !string.IsNullOrWhiteSpace(requestBody) ? requestBody : queryParams;
        }

        try
        {
            await next(context);
        }
        finally
        {
            responseBodyStream.Position = 0;
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
            responseBodyStream.Position = 0;
            await responseBodyStream.CopyToAsync(originalBodyStream);

            var logEntry = new LogControleRequisicao
            {
                TraceId = traceId,
                Rota = request.Path.Value ?? "/",
                StatusCode = (short)response.StatusCode,
                DadosRequisicao = entradaFinal,
                DadosResposta = responseBody,
            };

            await logControleService.SalvarLogControleAsync(logEntry);
        }
    }
}
