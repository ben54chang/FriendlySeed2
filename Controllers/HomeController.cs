using Microsoft.AspNetCore.Mvc;
using FriendlySeed.Services;

namespace FriendlySeed.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IArticleService _articleService;
        private readonly IWebsiteSettingsService _websiteSettingsService;

        public HomeController(ILogger<HomeController> logger, IArticleService articleService, IWebsiteSettingsService websiteSettingsService)
        {
            _logger = logger;
            _articleService = articleService;
            _websiteSettingsService = websiteSettingsService;
        }

        public IActionResult Index()
        {
            // 重定向到靜態網頁
            return Redirect("/static/index.html");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
