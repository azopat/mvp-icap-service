﻿using Glasswall.IcapServer.CloudProxyApp.Configuration;
using Glasswall.IcapServer.CloudProxyApp.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Glasswall.IcapServer.CloudProxyApp
{
    class Program
    {
        static IServiceProvider _serviceProvider;

        static int Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables()
              .AddCommandLine(args, CommandLineSwitchMapping.Mapping)
              .Build();

            try
            {
                var services = new ServiceCollection();
                _serviceProvider = services.ConfigureServices(configuration);
                return _serviceProvider.GetRequiredService<CloudProxyApplication>().Run();
            }
            catch(InvalidApplicationConfigurationException iace)
            {
                Console.WriteLine($"Invalid Configuration: {iace.Message}");
                return (int)ReturnOutcome.GW_ERROR;
            }
            finally
            {
                DisposeServices();
            }
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
                return;

            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
