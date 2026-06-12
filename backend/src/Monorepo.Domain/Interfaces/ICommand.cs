namespace Monorepo.Domain.Interfaces;

/// <summary>
/// Representa um comando que, quando executado, produz uma resposta do tipo <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">O tipo do resultado retornado pelo comando.</typeparam>
public interface ICommand<TResponse> { }
