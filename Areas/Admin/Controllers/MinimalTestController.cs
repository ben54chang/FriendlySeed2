using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FriendlySeed.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class MinimalTestController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Test(string ArticleCategoryName, int SortOrder, bool IsActive)
        {
            try
            {
                // 直接接收參數，不使用模型綁定
                if (string.IsNullOrEmpty(ArticleCategoryName))
                {
                    TempData["ErrorMessage"] = "請輸入分類名稱";
                    return View("Index");
                }

                TempData["SuccessMessage"] = $"測試成功！名稱: {ArticleCategoryName}, 排序: {SortOrder}, 啟用: {IsActive}";
                return View("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "發生錯誤: " + ex.Message;
                return View("Index");
            }
        }
    }
}
