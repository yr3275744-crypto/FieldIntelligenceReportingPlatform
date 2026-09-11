using API.Models;

namespace API.Services
{
    public interface IReportService
    {
        Task<IEnumerable<Report>> Search(string? text,
            string? theater,
            string? sector,
            string? location,
            string? priorities,
            string? reportType,
            DateTime? from);
        Task<IEnumerable<Report>> BySubject(string subjectId);
        Task<IEnumerable<Report>> ByErea(string? theater,
                string? sector,
                string? location,
                string? priorities,
                DateTime? from,
                DateTime? to);
        Task<IEnumerable<ReportCountDto>> CountByGroups();
    }
}
