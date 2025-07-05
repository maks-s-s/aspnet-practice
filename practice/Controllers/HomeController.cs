using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practice.Data;
using practice.Models;
using System.Linq;
using System.Threading.Tasks;

namespace practice.Controllers
{
    [Route("home")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public HomeController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var surveys = await _context.Surveys
                .Include(s => s.Hashtags)
                .ToListAsync();

            var user = await _userManager.GetUserAsync(User);
            ViewData["UserName"] = user?.UserName ?? "Guest";
            ViewData["UserEmail"] = user?.Email ?? "No email";

            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                ViewData["UserRole"] = roles.FirstOrDefault() ?? "Unknown";
            }
            else
            {
                ViewData["UserRole"] = "Unknown";
            }

            return View(surveys);
        }
    }
}
