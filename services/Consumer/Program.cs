using Consumer.Services;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.DependencyInjection;
using System;
namespace Consumer;
using Microsoft.Extensions.Logging;
using Serilog;
using Consumer.Models;
using dotenv.net;

public class Program
{
    public async static Task Main()
    {
        DotEnv.Load();

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton(sp => new ConfigStrings
        {
            Bootsrapservers = Environment.GetEnvironmentVariable("KAFKA_SERVERS")!,
            GroupId = Environment.GetEnvironmentVariable("GROUP_ID")!,
            Topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC")!,
            ElasticsearchEndpoing = Environment.GetEnvironmentVariable("ELASTIC_ENDPOINT")!,
            IndexName = Environment.GetEnvironmentVariable("INDEX_NAME")!
        });
        string uri = Environment.GetEnvironmentVariable("ELASTIC_ENDPOINT")!;
        var settings = new ElasticsearchClientSettings(new Uri(uri));

        
        //serviceCollection.AddLogging(builder => builder.Add())
        serviceCollection.AddSingleton(sp => new ElasticsearchClient(settings));
        serviceCollection.AddSingleton<CreateIndexService>();
        serviceCollection.AddSingleton<ReportValidator>();
        serviceCollection.AddScoped<ConsumeToDbService>();
        serviceCollection.AddSingleton(sp => new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger());
        var provider = serviceCollection.BuildServiceProvider();
        bool IsCreated = await provider.GetRequiredService<CreateIndexService>().CreateIndex();

        CancellationTokenSource cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true; // prevent the process from terminating.
            cts.Cancel();
        };
        Console.WriteLine(IsCreated);
        using var scope = provider.CreateScope();
        await scope.ServiceProvider.GetRequiredService<ConsumeToDbService>().ConsumeLoop(cts.Token);
    }
}