using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FriendlySeed.Models;

namespace FriendlySeed.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class TestCategoryController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new ArticleCategoryTest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ArticleCategoryTest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    var modelErrors = ModelState.Where(x => x.Value.Errors.Count > 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
                    
                    var errorDetails = string.Join(", ", modelErrors.Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value)}"));
                    TempData["ErrorMessage"] = "請檢查輸入的資料: " + string.Join(", ", errors) + " | 詳細: " + errorDetails;
                    return View("Index", model);
                }

                TempData["SuccessMessage"] = "測試成功！模型驗證通過";
                return View("Index", new ArticleCategoryTest());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "發生錯誤: " + ex.Message;
                return View("Index", model);
            }
        }
    }
}
