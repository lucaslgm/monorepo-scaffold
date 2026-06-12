using Monorepo.WebApi.Shared.Logging.Models;

namespace Monorepo.WebApi.Shared.Services.Interfaces;

public interface ILogControleService
{
    Task SalvarLogControleAsync(LogControleRequisicao log, CancellationToken cancellationToken = default);
}
