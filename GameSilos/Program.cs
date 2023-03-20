// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;

Console.WriteLine("Hello, Silo!");


await Host.CreateDefaultBuilder()
    .UseOrleans(config =>
    {
        config.UseLocalhostClustering();
        config.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "dev";
            options.ServiceId = "GameSilos";
        });
        config.AddMemoryGrainStorageAsDefault();
    })
    .ConfigureLogging(builder => builder.AddConsole())
    .RunConsoleAsync();
     
     
     //38334063