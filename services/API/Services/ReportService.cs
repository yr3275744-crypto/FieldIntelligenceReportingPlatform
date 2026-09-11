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
        public async Task<IEnumerable<Report>> Search(string? text,
            string? theater,
            string? sector,
            string? location,
            string? priorities,
            string? reportType,
            DateTime? from)
        {
            var queries = new List<Query>();
            if (text != null)
            {
                queries.Add(new MatchQuery
                {
                    Field = Infer.Field<Report>(r => r.Message),
                    Query = text
                });
            }
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
            if (reportType != null)
            {
                queries.Add(new TermQuery()
                {
                    Field = Infer.Field<Report>(r => r.ReportType),
                    Value = reportType
                });
            }
            if (from != null)
            {
                queries.Add(new DateRangeQuery()
                {
                    Field = Infer.Field<Report>(r => r.Timestamp),
                    Gte = from.Value
                });
            }

            var response = await _client.SearchAsync<Report>(s => s
                .Indices(_indexName)
                .Size(10000)
                .Query(q => q
                    .Bool(b => b
                        .Must(queries))));

            if (!response.IsValidResponse)
            {
                _logger.Error("Elasticsearch search failed: {Error}", response.DebugInformation);
            }
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
        public async Task<IEnumerable<ReportCountDto>> CountByGroups()
        {
            var response = await _client.SearchAsync<Report>(s => s
                .Indices(_indexName)
                .Size(0)
                .Aggregations(a => a
                    .Add("by_priority", a => a
                        .Terms(t => t
                            .Field(r => r.Priority)
                        )
                        .Aggregations(a => a
                            .Add("by_report_type", a => a
                                .Terms(t => t
                                    .Field(r => r.ReportType)
                                    )
                                .Aggregations(a => a
                                    .Add("by_theater", a => a
                                        .Terms(t => t
                                            .Field(r => r.Theater)
                                    )
                                )
                            )
                        )
                    ))));
            if (response == null ||!response.IsValidResponse)
            {
                _logger.Information("Failed to get report counts.");
                return Enumerable.Empty<ReportCountDto>();
            }

            var result = new List<ReportCountDto>();
            var prioritiesGroup = response.Aggregations!.GetStringTerms("by_priority");
            if (prioritiesGroup == null)
            {
                return result;
            }
            foreach (var priorityBucket in prioritiesGroup.Buckets)
            {

                var reportTypeAgg = priorityBucket
                    .Aggregations!.GetStringTerms("by_report_type");
                if (reportTypeAgg == null) { continue; }
                foreach (var reportTypeBucket in reportTypeAgg.Buckets)
                {
                    var theaterAgg =
                        reportTypeBucket.Aggregations!.GetStringTerms("by_theater");
                    if (theaterAgg == null) { continue; }
                    foreach (var theaterBucket in theaterAgg.Buckets)
                    {
                        result.Add(new ReportCountDto
                        {
                            Priority = priorityBucket.Key.ToString(),
                            ReportType = reportTypeBucket.Key.ToString(),
                            Theater = theaterBucket.Key.ToString(),
                            Count = theaterBucket.DocCount
                        });
                    }
                }
            }
             return result;
        }
    }
}
