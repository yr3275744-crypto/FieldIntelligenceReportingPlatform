using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }
        [HttpGet("reports/search")]
        public async Task<ActionResult<IEnumerable<Report>>> SearchByText(
            [FromQuery] string text)
        {
            var result = await _reportService.SearchByText(text);
            return Ok(result);
        }
        [HttpGet("subjects/{subjectId}/reports")]
        public async Task<ActionResult<IEnumerable<Report>>> BySubject(string subjectId)
        {
            var result = await _reportService.BySubject(subjectId);
            return Ok(result);
        }
        [HttpGet("reports")]
        public async Task<ActionResult<IEnumerable<Report>>> FilterReports(
            [FromQuery] string? theater,
            [FromQuery] string? sector,
            [FromQuery] string? location,
            [FromQuery] string? priorities,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var result = await _reportService.ByErea(theater, sector, location, priorities, from, to);
            return Ok(result);
        }
        //[HttpGet("reports")]
        //public async Task<ActionResult<IEnumerable<Report>>> ByPriorityAndDate(
        //    string? priorities,
        //    DateTime? from,
        //    DateTime? to)
        //{
        //    var result = await _reportService.ByPriorityAndDate(priorities, from, to);
        //    return Ok(result);
        //}
    }
}
