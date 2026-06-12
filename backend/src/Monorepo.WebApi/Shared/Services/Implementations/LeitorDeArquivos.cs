using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Monorepo.WebApi.Shared.Services.Interfaces;

namespace Monorepo.WebApi.Shared.Services.Implementations;

[ExcludeFromCodeCoverage]
public class LeitorDeArquivos : ILeitorDeArquivos
{
    public string LerTextoCompleto(string nomeArquivo)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var resourceName =
            assembly.GetManifestResourceNames().FirstOrDefault(str => str.EndsWith(nomeArquivo))
            ?? throw new Exception($"SQL {nomeArquivo} não encontrado!");

        using Stream stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
