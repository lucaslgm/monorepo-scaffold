namespace Monorepo.Domain.Interfaces;

/// <summary>
/// Interface para qualquer serviço que requeira DI.
/// </summary>
/// <typeparam name="T">A própria classe de serviço que está herdando</typeparam>
public interface IService<T> { }
