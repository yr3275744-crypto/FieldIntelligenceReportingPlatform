using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.DependencyInjection;
using System;
namespace Consumer;

public class Program
{
    public async static Task Main()
    {
        var serviceCollection = new ServiceCollection();

        string uri = Environment.GetEnvironmentVariable("ELASTICSEARCH_URI")!;
        var settings = new ElasticsearchClientSettings(new Uri(uri));

        serviceCollection.AddSingleton(sp => new ElasticsearchClient(settings));
    }
}