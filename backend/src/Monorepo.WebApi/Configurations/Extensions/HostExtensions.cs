using Autofac;
using Autofac.Extensions.DependencyInjection;
using Monorepo.WebApi.Autofac;

namespace Monorepo.WebApi.Configurations.Extensions;

public static class HostExtensions
{
    public static void ConfigureAutoFac(this IHostBuilder host)
    {
        host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

        host.ConfigureContainer<ContainerBuilder>(bd =>
        {
            bd.RegisterModule(new WebApiModule());
        });
    }
}
