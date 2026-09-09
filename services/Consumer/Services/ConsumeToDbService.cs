using Confluent.Kafka;
using Consumer.Models;
using Elastic.Clients.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Consumer.Services
{
    public class ConsumeToDbService
    {
        private readonly ConfigStrings _strings;
        private readonly ReportValidator _validator;
        private readonly ElasticsearchClient _client;
        private readonly IConsumer<Ignore, string> _consumer;

        private Report ConvertInputToReport(ReportInput input)
        {
            Report report = new Report
            {
                AgentId = input.AgentId,
                Location = input.Location,
                Message = input.Message,
                Priority = input.Priority,
                ProcessedAt = DateTime.UtcNow,
                ReportId = input.ReportId,
                ReportType = input.ReportType,
                Sector = input.Sector,
                SourceType = input.SourceType,
                SubjectId = input.SubjectId,
                SubjectType = input.SubjectType,
                Theater = input.Theater,
                Timestamp = input.Timestamp,
                Unit = input.Unit
            };
            return report;
        }
        public ConsumeToDbService(ConfigStrings strings,
            ReportValidator validator,
            ElasticsearchClient client)
        {
            _strings = strings;
            _validator = validator;
            _client = client;

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _strings.Bootsrapservers,
                GroupId = _strings.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        }

        public async Task ConsumeLoop(CancellationToken token)
        {
            _consumer.Subscribe(_strings.Topic);

            //CancellationTokenSource cts = new CancellationTokenSource();
            //Console.CancelKeyPress += (_, e) => {
            //    e.Cancel = true; // prevent the process from terminating.
            //    cts.Cancel();
            //};

            int validReports = 0;
            try
            {
                //var cts = new CancellationTokenSource();

                while (!token.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(token);
                    if (consumeResult == null || consumeResult.Message.Value == null)
                    {
                        continue;
                    }

                    var SeralizationOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    try
                    {
                        ReportInput? reportInput = JsonSerializer.Deserialize<ReportInput>(consumeResult.Message.Value, SeralizationOptions);
                        if (reportInput == null)
                        {
                            Console.WriteLine("missing value");
                            continue;
                        }
                        Report report = ConvertInputToReport(reportInput);
                        var isValid = _validator.ValidateReport(report);
                        if (!isValid)
                        {
                            Console.WriteLine("invalid report");
                            continue;
                        }
                        var response = await _client.IndexAsync(report, i => i
                        .Index(_strings.IndexName)
                        .Id(report.ReportId)
                        );
                        if (response == null || !response.IsValidResponse)
                        {
                            Console.WriteLine("The report send faild.");
                        }
                        else
                        {
                            Console.WriteLine("The report send successfully");
                            validReports++;
                        }
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine("========== DESERIALIZATION ERROR ==========");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(consumeResult.Message.Value);
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine($"somthing get wrong, reason: {ex.Message}\n " +
                    $"trace: {ex.StackTrace}\n ");
            }
            finally
            {
                _consumer.Unsubscribe();
                _consumer.Close();
                Console.WriteLine($"valid reports send: {validReports}");
            }
        }
    }
}
