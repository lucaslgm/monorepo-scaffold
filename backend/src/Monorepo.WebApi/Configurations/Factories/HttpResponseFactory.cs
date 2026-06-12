using System.Diagnostics;
using CSharpFunctionalExtensions;
using Monorepo.Domain.Commons.Entities;
using Monorepo.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Monorepo.WebApi.Configurations.Factories;

public sealed class HttpResponseFactory(IHttpContextAccessor httpContextAccessor) : IService<HttpResponseFactory>
{
    private string RequestPath => httpContextAccessor.HttpContext?.Request.Path ?? string.Empty;

    public IActionResult MapResult<T>(Result<T, Error> result)
    {
        if (result.IsSuccess)
        {
            return Create200(result.Value);
        }

        var error = result.Error;

        return error.Code switch
        {
            ErrorCode.Validation => Create400("Erro de Validação", "Um ou mais campos não atendem às regras de negócio.", error),
            ErrorCode.Unauthorized => Create401("Não Autorizado", "Usuário sem autorização para acessar este recurso.", error),
            ErrorCode.Forbidden => Create403("Proibido", "Usuário sem permissão para acessar este recurso.", error),
            ErrorCode.NotFound => Create404("Recurso não encontrado", "O recurso solicitado não foi encontrado.", error),
            ErrorCode.Conflict => Create409("Conflito de Negócio", "Existe um recurso ou estado conflitante com esta operação.", error),
            ErrorCode.InternalError => Create500("Erro inesperado", "Houve um erro ao processar a requisição", error),
            _ => Create400("Erro na Operação", "Ocorreu um erro ao processar a solicitação.", error)
        };
    }

    private OkObjectResult Create200<T>(T body, string message = "Operação realizada com sucesso.") =>
        new(new ApiResponse<T>(StatusCodes.Status200OK, message, body, RequestPath));

    private BadRequestObjectResult Create400(string title, string detail, Error? error = null)
    {
        var problemDetails = CreateProblemDetails(StatusCodes.Status400BadRequest, title, detail, error);
        return new BadRequestObjectResult(problemDetails);
    }

    private UnauthorizedObjectResult Create401(string title, string detail, Error? error = null)
    {
        var problemDetails = CreateProblemDetails(StatusCodes.Status401Unauthorized, title, detail, error);
        return new UnauthorizedObjectResult(problemDetails);
    }

    private ObjectResult Create403(string title, string detail, Error? error = null)
    {
        var problemDetails = CreateProblemDetails(StatusCodes.Status403Forbidden, title, detail, error);
        return new ObjectResult(problemDetails) { StatusCode = StatusCodes.Status403Forbidden };
    }


    private NotFoundObjectResult Create404(string title, string detail, Error? error = null)
    {
        var problemDetails = CreateProblemDetails(StatusCodes.Status404NotFound, title, detail, error);
        return new NotFoundObjectResult(problemDetails);
    }

    private ConflictObjectResult Create409(string title, string detail, Error? error = null)
    {
        var problemDetails = CreateProblemDetails(StatusCodes.Status409Conflict, title, detail, error);
        return new ConflictObjectResult(problemDetails);
    }

    private ObjectResult Create500(string title, string detail, Error? error = null)
    {
        var problemDetails = CreateProblemDetails(StatusCodes.Status500InternalServerError, title, detail, error);
        return new ObjectResult(problemDetails) { StatusCode = StatusCodes.Status500InternalServerError };
    }

    public ProblemDetails CreateProblemDetails(int status, string title, string detail, Error? error = null)
    {
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Type = GetTypeUri(status),
            Instance = RequestPath
        };

        problem.Extensions.Add("traceId", Activity.Current?.TraceId.ToString() ?? httpContextAccessor.HttpContext?.TraceIdentifier);

        if (error != null)
        {
            problem.Extensions.Add("error", new { message = error.Message, code = error.Code.ToString() });
        }

        return problem;
    }

    private static string GetTypeUri(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            StatusCodes.Status401Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
            StatusCodes.Status403Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            StatusCodes.Status404NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            StatusCodes.Status409Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            StatusCodes.Status500InternalServerError => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            _ => "https://tools.ietf.org/html/rfc7231"
        };
    }
}

public record ApiResponse<T>(int Status, string Title, T Data, string Instance = "");
