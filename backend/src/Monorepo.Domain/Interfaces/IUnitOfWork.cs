namespace Monorepo.Domain.Interfaces;

/// <summary>
/// Define o contrato para o padrão Unit of Work.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Salva todas as alterações feitas no contexto para o banco de dados.
    /// </summary>
    /// <param name="cancellationToken">O token de cancelamento.</param>
    /// <returns>O número de entradas de estado gravadas no banco de dados.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Inicia uma nova transação de banco de dados.
    /// </summary>
    /// <param name="cancellationToken">O token de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirma a transação de banco de dados atual.
    /// </summary>
    /// <param name="cancellationToken">O token de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Reverte a transação de banco de dados atual.
    /// </summary>
    /// <param name="cancellationToken">O token de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
