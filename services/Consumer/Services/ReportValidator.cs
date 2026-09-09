using Consumer.Enums;
using Consumer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer.Services
{
    public class ReportValidator
    {
        public bool ValidateReport(Report report)
        {
            if (string.IsNullOrWhiteSpace(report.ReportId) ||
                string.IsNullOrWhiteSpace(report.AgentId) ||
                string.IsNullOrWhiteSpace(report.Unit) ||
                string.IsNullOrWhiteSpace(report.Theater) ||
                string.IsNullOrWhiteSpace(report.Sector) ||
                string.IsNullOrWhiteSpace(report.Location) ||
                string.IsNullOrWhiteSpace(report.ReportId) ||
                string.IsNullOrWhiteSpace(report.SourceType) ||
                string.IsNullOrWhiteSpace(report.Message) || 
                report.Priority == null ||
                report.ReportType == null
                )
            {
                return false;
            }
            if ((string.IsNullOrWhiteSpace(report.SubjectId) && !string.IsNullOrWhiteSpace(report.SubjectType)) ||
                (string.IsNullOrWhiteSpace(report.SubjectType) && !string.IsNullOrWhiteSpace(report.SubjectId))
                )
            {
                return false;
            }
            return true;
        }
    }
}
