namespace Monorepo.Domain.Interfaces;

/// <summary>
/// Representa uma consulta que, quando executada, produz uma resposta do tipo <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">O tipo de resposta da consulta.</typeparam>
public interface IQuery<TResponse> { }
