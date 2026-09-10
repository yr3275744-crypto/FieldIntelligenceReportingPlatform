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
        public async Task<IEnumerable<Report>> ByErea(string? theater,
            string? sector,
            string? location,
            string? priorities,
            DateTime? from,
            DateTime? to)
        {
            var queries = new List<Query>();
            if (theater != null)
            {
                queries.Add(new TermQuery()
                {
                    Field = Infer.Field<Report>(r => r.Theater),
                    Value = theater
                });
            }
            if (sector != null)
            {
                queries.Add(new TermQuery()
                {
                    Field = Infer.Field<Report>(r => r.Sector),
                    Value = sector
                });
            }
            if (location != null)
            {
                queries.Add(new TermQuery()
                {
                    Field = Infer.Field<Report>(r => r.Location),
                    Value = location
                });
            }
            if (priorities != null)
            {
                queries.Add(new TermQuery()
                {
                    Field = Infer.Field<Report>(r => r.Priority),
                    Value = priorities
                });
            }

            if (from != null)
            {
                queries.Add(new DateRangeQuery()
                {
                    Field = Infer.Field<Report>(r => r.Timestamp),
                    Gte = from
                });
            }

            if (to != null)
            {
                queries.Add(new DateRangeQuery()
                {
                    Field = Infer.Field<Report>(r => r.Timestamp),
                    Lte = to
                });
            }
            var response = await _client.SearchAsync<Report>(s => s
                .Indices(_indexName)
                .Size(10000)
                .Query(q => q
                    .Bool(b => b
                        .Filter(queries))));
            return response.Documents;
        }
        //public async Task<IEnumerable<Report>> ByPriorityAndDate(
        //    string? priorities,
        //    DateTime? from,
        //    DateTime? to)
        //{
        //    var queries = new List<Query>();

        //    if (priorities != null)
        //    {
        //        queries.Add(new TermQuery()
        //        {
        //            Field = Infer.Field<Report>(r => r.Priority),
        //            Value = priorities
        //        });
        //    }

        //    if (from != null)
        //    {
        //        queries.Add(new DateRangeQuery()
        //        {
        //            Field = Infer.Field<Report>(r => r.Timestamp),
        //            Gte = from
        //        });
        //    }

        //    if (to != null)
        //    {
        //        queries.Add(new DateRangeQuery()
        //        {
        //            Field = Infer.Field<Report>(r => r.Timestamp),
        //            Lte = to
        //        });
        //    }

        //    var response = await _client.SearchAsync<Report>(s => s
        //        .Indices(_indexName)
        //        .Size(10000)
        //        .Query(q => q
        //            .Bool(b => b
        //                .Filter(queries)
        //            )
        //        )
        //    );

        //    return response.Documents;
        //}
    }
}
