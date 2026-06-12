using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace Monorepo.WebApi.Tests.Integration.Setups;

public class MockacoServiceSetup : IAsyncLifetime
{
    private readonly IContainer _banrisulContainer = new ContainerBuilder("natenho/mockaco")
        .WithName("mockaco-banrisulapi")
        .WithReuse(true)
        .WithPortBinding(7000, 5000)
        .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilInternalTcpPortIsAvailable(5000))
        .WithBindMount(ToAbsolute($"./Setups/Mockaco/Mocks/BanrisulApi"), "/app/Mocks", AccessMode.ReadWrite)
        .Build();

    private static string ToAbsolute(string path) => Path.GetFullPath(path);

    public async Task InitializeAsync()
    {
        using var cancelationToken = new CancellationTokenSource(TimeSpan.FromMinutes(1));

        // await _banrisulContainer.StartAsync(cancelationToken.Token).ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
        using var cancelationToken = new CancellationTokenSource(TimeSpan.FromMinutes(1));

        await _banrisulContainer.StopAsync(cancelationToken.Token);
    }
}
