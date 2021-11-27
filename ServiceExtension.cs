using Microsoft.Extensions.Hosting;
using NetDaemon.Extensions.Persistance;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.Extensions.Options;
using NetDaemon.Common.Configuration;

static class AppServicesExtension
{
    public static IHostBuilder AddAppServices(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureServices((_, services) =>
        {
            services.AddSingleton<IDataRepository>(n => new DataRepository(
                Path.Combine(
                     n.GetRequiredService<IOptions<NetDaemonSettings>>().Value.GetAppSourceDirectory()
                    , ".storage")));
        });
    }
}