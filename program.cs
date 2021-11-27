using System;
using Microsoft.Extensions.Hosting;
using NetDaemon;
using NetDaemon.Extensions.Persistance;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.Extensions.Options;
using NetDaemon.Common.Configuration;

try
{
    await Host.CreateDefaultBuilder(args)
        .UseDefaultNetDaemonLogging()
        .UseNetDaemon()
        .AddAppServices()
        .Build()
        .RunAsync();
}
catch (Exception e)
{
    Console.WriteLine($"Failed to start host... {e}");
}
finally
{
    NetDaemon.NetDaemonExtensions.CleanupNetDaemon();
}


