namespace Monorepo.WebApi.Shared.Extensions;

public static class BooleanExtensions
{
    /// <summary>
    /// Converte bool para "Sim"/"Não" ou valores customizados.
    /// </summary>
    public static string ToSimNao(this bool? valor, string sim = "Sim", string nao = "Não")
    {
        return valor switch
        {
            true => sim,
            false => nao,
            _ => string.Empty
        };
    }

    public static string ToSimNao(this bool valor, string sim = "Sim", string nao = "Não")
        => valor ? sim : nao;
}
