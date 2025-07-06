using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<HomeController> _logger;

        public HomeController(AppDbContext context, UserManager<User> userManager, ILogger<HomeController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Начинаем загрузку списка опросов");

            try
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
                    _logger.LogInformation($"Пользователь {user.UserName} с ролью {ViewData["UserRole"]} загрузил главную страницу.");
                }
                else
                {
                    ViewData["UserRole"] = "Unknown";
                    _logger.LogWarning("Неавторизованный пользователь загрузил главную страницу.");
                }

                return View(surveys);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке главной страницы");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet("error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
