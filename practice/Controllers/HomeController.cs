using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practice.Data;
using practice.Models;
using System.Linq;
using System.Threading.Tasks;

namespace practice.Controllers
{
    [Route("home")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var surveys = await _context.Surveys
                .Include(s => s.Hashtags)
                .ToListAsync();
            return View(surveys);
        }
    }
}
