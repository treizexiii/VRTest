// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Console.WriteLine("Hello, Silo!");


await Host.CreateDefaultBuilder()
    .UseOrleans(builder =>
    {
        builder.UseLocalhostClustering();
        builder.AddMemoryGrainStorageAsDefault();
    })
    .ConfigureLogging(builder => builder.AddConsole())
    .RunConsoleAsync();