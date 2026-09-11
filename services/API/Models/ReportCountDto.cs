namespace API.Models
{
    public class ReportCountDto
    {
        public string Priority { get; set; } = string.Empty;

        public string ReportType { get; set; } = string.Empty;

        public string Theater { get; set; } = string.Empty;

        public long Count { get; set; }
    }
}
