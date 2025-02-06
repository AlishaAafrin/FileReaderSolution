using FileConsumerSolution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FileReaderWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileDataController : Controller
    {
        private readonly DataContext _context;
        private readonly ILogger<FileDataController> _logger;
        public FileDataController(DataContext context, ILogger<FileDataController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetData([FromQuery] int pageNo = 1, [FromQuery] int pageSize = 10, [FromQuery] string name = "")
        {
            _logger.LogInformation("satrt");
            var query = _context.FileRecords.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.Content.Contains(name));

            var totalRecords = await query.CountAsync();
            var results = await query.Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
            _logger.LogInformation("end");
            return Ok(new { TotalRecords = totalRecords, Data = results });
          
        }
    }
}
