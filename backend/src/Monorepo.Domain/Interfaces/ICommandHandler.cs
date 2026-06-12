namespace Monorepo.Domain.Interfaces;

/// <summary>
/// Define um handler para um comando específico.
/// </summary>
/// <typeparam name="TCommand">O tipo do comando a ser manipulado.</typeparam>
/// <typeparam name="TResponse">O tipo da resposta retornada pelo handler.</typeparam>
public interface ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    /// <summary>
    /// Manipula um comando de forma assíncrona.
    /// </summary>
    /// <param name="command">O comando a ser manipulado.</param>
    /// <param name="cancellationToken">O token de cancelamento.</param>
    /// <returns>A resposta da operação.</returns>
    Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken);
}
