using API.Models;

namespace API.Services
{
    public interface IReportService
    {
        Task<IEnumerable<Report>> SearchByText(string text);
        Task<IEnumerable<Report>> BySubject(string subjectId);
        Task<IEnumerable<Report>> ByErea(string? theater,
                string? sector,
                string? location,
                string? priorities,
                DateTime? from,
                DateTime? to);
        //Task<IEnumerable<Report>> ByPriorityAndDate(
        //    string? priorities,
        //    DateTime? from,
        //    DateTime? to);
    }
}
