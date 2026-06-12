using System.Reflection;

namespace Monorepo.WebApi.Tests.Integration.Shared;

public static class MetodosUteis
{
    public static async Task<string> ObterScript(string nomeArquivo)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var resourceName = assembly.GetManifestResourceNames()
                               .FirstOrDefault(str => str.EndsWith(nomeArquivo))
                           ?? throw new FileNotFoundException($"Recurso '{nomeArquivo}' não encontrado no assembly {assembly.FullName}.");

        await using Stream stream = assembly.GetManifestResourceStream(resourceName)
                                    ?? throw new InvalidOperationException($"Não foi possível abrir o stream para {resourceName}.");

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}
