using Consumer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer.Models
{
    public class Report
    {
        public string ReportId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string AgentId { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Theater { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public ValidReportTypes ReportType { get; set; }
        public ValidPriorities Priority { get; set; }
        public string SourceType { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? SubjectId { get; set; }
        public string? SubjectType { get; set; }
    }
}
