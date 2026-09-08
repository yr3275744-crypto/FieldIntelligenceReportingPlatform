using Consumer.Models;
using Elastic.Clients.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer.Services
{
    public class CreateIndexService
    {
        private readonly ElasticsearchClient _client;
        private readonly ConfigStrings _configStrings
        public CreateIndexService(ElasticsearchClient client,
            ConfigStrings configStrings)
        {
            _client = client;
            _configStrings = configStrings;
        }
        public async Task<bool> Create()
        {
            var response = await _client.Indices.CreateAsync<Report>(c =>
            c.Index(_configStrings.IndexName)
                .Mappings(m => m
                    .Properties(p => p
                        .Keyword(r => r.ReportId)
                        .Date(r => r.Timestamp)
                        .Keyword(r => r.AgentId)
                        .Keyword(r => r.Unit)
                        .Keyword(r => r.Theater)
                        .Keyword(r => r.Sector)
                        .Keyword(r => r.Location)
                        .Keyword(r => r.ReportType)
                        .Keyword(r => r.Priority)
                        .Keyword(r => r.SourceType)
                        .Text(r => r.Message)
                        .Keyword(r => r.SubjectId)
                        .Keyword(r => r.SubjectType)
                             )
                        )
                    );
            if (response == null || !response.IsValidResponse)
            {
                return false;
            }
            return true;
        }
    }
}
