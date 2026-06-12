namespace Monorepo.WebApi.Shared.Logging.Models;

public class LogControleRequisicao
{
    // O SQL cuidará do IDENTITY(1,1)
    public string TraceId { get; init; } = string.Empty;
    public string Rota { get; init; } = string.Empty;
    public short StatusCode { get; set; }
    public string? DadosRequisicao { get; set; }
    public string? DadosResposta { get; set; }
    // public string DadosTipo { get; init; } = "API_REST"; // Valor padrão para sua coluna DADOS_TIPO
}


