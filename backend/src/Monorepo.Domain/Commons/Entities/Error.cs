namespace Monorepo.Domain.Commons.Entities;

public record Error(string Message, ErrorCode Code)
{
    // 400 - Validation
    public static Error Validation(string message) => new(message, ErrorCode.Validation);

    // 401 - Unauthorized
    public static Error Unauthorized(string message = "Usuário não autenticado.")
        => new(message, ErrorCode.Unauthorized);

    // 403 - Forbidden
    public static Error Forbidden(string message = "Usuário não possui permissão para esta operação.")
        => new(message, ErrorCode.Forbidden);

    // 404 - Not Found
    public static Error NotFound(string message) => new(message, ErrorCode.NotFound);

    // 409 - Conflict
    public static Error Conflict(string message) => new(message, ErrorCode.Conflict);

    // 500 - Internal Server Error
    public static Error InternalError(string message = "Ocorreu um erro inesperado no servidor.")
        => new(message, ErrorCode.InternalError);
}

public enum ErrorCode
{
    Validation = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    Conflict = 409,
    InternalError = 500
}
