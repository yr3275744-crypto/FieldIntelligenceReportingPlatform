using API.Models;

namespace API.Services
{
    public interface IReportService
    {
        Task<IEnumerable<Report>> SearchByText(string text);
        Task<IEnumerable<Report>> BySubject(string subjectId);
    }
}
