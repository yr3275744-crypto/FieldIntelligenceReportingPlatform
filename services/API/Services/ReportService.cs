using API.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Serilog.Core;
namespace API.Services
{
    public class ReportService : IReportService
    {
        private readonly Logger _logger;
        private readonly ElasticsearchClient _client;
        private readonly string _indexName;
        public ReportService(
            Logger logger,
            ElasticsearchClient client,
            ConfigStrings configStrings)
        {
            _logger = logger;
            _client = client;
            _indexName = configStrings.IndexName;
        }
        public async Task<IEnumerable<Report>> SearchByText(string text)
        {
            var response = await _client
                .SearchAsync<Report>(search => search
                .Indices(_indexName)
                .Query(query => query
                    .Match(m => m
                    .Field(f => f.Message)
                    .Query(text)
                    ))
                .Size(10000));
            return response.Documents;    
        }
        public async Task<IEnumerable<Report>> BySubject(string subjectId)
        {
            var response = await _client
                .SearchAsync<Report>(search => search
                .Indices(_indexName)
                .Query(query => query
                    .Term(t => t
                        .Field(r => r.SubjectId)
                        .Value(subjectId)
                        ))
                .Size(10000)
                    );
            return response.Documents;
        }

    }
}
