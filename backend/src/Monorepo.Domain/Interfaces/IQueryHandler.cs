namespace Monorepo.Domain.Interfaces;

/// <summary>
/// Define um manipulador para uma consulta.
/// </summary>
/// <typeparam name="TQuery">O tipo da consulta.</typeparam>
/// <typeparam name="TResponse">O tipo da resposta.</typeparam>
public interface IQueryHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    /// <summary>
    /// Manipula uma consulta de forma assíncrona.
    /// </summary>
    /// <param name="query">O tipo da consulta.</param>
    /// <param name="cancellationToken">Token para cancelamento da operação.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona, contendo a resposta da consulta.</returns>
    Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}
