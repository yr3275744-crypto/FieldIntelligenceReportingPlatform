using API.Models;
using API.Services;
using dotenv.net;
using Elastic.Clients.Elasticsearch;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();

string uriString = Environment.GetEnvironmentVariable("ELASTIC_ENDPOINT")!;
var clientSettings = new ElasticsearchClientSettings(new Uri(uriString));

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(sp => new ConfigStrings
{
    IndexName = Environment.GetEnvironmentVariable("INDEX_NAME")!
});
builder.Services.AddSingleton(sp => new ElasticsearchClient(clientSettings));
builder.Services.AddSingleton(sp => new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/API-logs.txt")
    .CreateLogger()
);
builder.Services.AddSingleton<IReportService, ReportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
