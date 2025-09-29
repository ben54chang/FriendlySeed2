using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FriendlySeed.Services;

namespace FriendlySeed.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITeacherService _teacherService;
        private readonly IArticleService _articleService;

        public HomeController(IUserService userService, ITeacherService teacherService, IArticleService articleService)
        {
            _userService = userService;
            _teacherService = teacherService;
            _articleService = articleService;
        }

        public async Task<IActionResult> Index()
        {
            // 取得統計資料
            var users = await _userService.GetAllUsersAsync();
            var teachers = await _teacherService.GetAllTeachersAsync();
            var articles = await _articleService.GetAllArticlesAsync();

            ViewBag.UserCount = users.Count();
            ViewBag.TeacherCount = teachers.Count();
            ViewBag.ArticleCount = articles.Count();

            return View();
        }
    }
}
