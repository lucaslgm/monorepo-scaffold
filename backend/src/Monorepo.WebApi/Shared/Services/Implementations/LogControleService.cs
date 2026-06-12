using System.Data;
using Dapper;
using Monorepo.Domain.Constants;
using Monorepo.Domain.Interfaces;
using Monorepo.WebApi.Shared.Logging.Models;
using Monorepo.WebApi.Shared.Persistence.Dapper;
using Monorepo.WebApi.Shared.Services.Interfaces;

namespace Monorepo.WebApi.Shared.Services.Implementations;

public class LogControleService(
    ILogger<LogControleService> logger,
    ILeitorDeArquivos leitorDeArquivos,
    IDbConnectionFactoryProvider connectionProvider) : ILogControleService
{
    public async Task SalvarLogControleAsync(LogControleRequisicao log, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando a gravação do log na base.");

        using var conn = connectionProvider.GetConnection(DbConnections.Log);

        var parameters = new DynamicParameters();
        parameters.Add("@Trace", log.TraceId.ToChar(32));
        parameters.Add("@Rota", log.Rota.ToVarChar(100));
        parameters.Add("@Status", log.StatusCode, DbType.Int16);
        parameters.Add("@Requisicao", log.DadosRequisicao?.ToVarChar(log.DadosRequisicao?.Length ?? 0));
        parameters.Add("@Resposta", log.DadosResposta?.ToVarChar(log.DadosResposta?.Length ?? 0));

        var commandText = leitorDeArquivos.LerTextoCompleto("inserirLogControle.sql");
        var command = new CommandDefinition(commandText, parameters, cancellationToken: cancellationToken);

        await conn.ExecuteAsync(command);
    }
}
