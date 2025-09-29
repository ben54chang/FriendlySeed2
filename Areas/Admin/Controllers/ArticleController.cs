using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FriendlySeed.Models;
using FriendlySeed.Services;

namespace FriendlySeed.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IArticleCategoryService _categoryService;
        private readonly ILogger<ArticleController> _logger;

        public ArticleController(IArticleService articleService, IArticleCategoryService categoryService, ILogger<ArticleController> logger)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var articles = await _articleService.GetAllArticlesAsync();
                var categories = await _categoryService.GetAllArticleCategoriesAsync();
                ViewBag.Categories = categories;
                return View(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading articles");
                TempData["ErrorMessage"] = "載入文章資料時發生錯誤";
                return View(new List<Article>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        public async Task<IActionResult> GetArticle(int id)
        {
            try
            {
                var article = await _articleService.GetArticleByIdAsync(id);
                if (article == null)
                {
                    return Json(new { success = false, message = "文章不存在" });
                }

                return Json(new
                {
                    success = true,
                    id = article.ID,
                    title = article.Title,
                    author = article.Author,
                    categoryID = article.CategoryID,
                    keywords = article.Keywords,
                    description = article.Description,
                    filePath = article.FilePath,
                    sortOrder = article.SortOrder,
                    isTop = article.IsTop,
                    isActive = article.IsActive,
                    publishTime = article.PublishTime?.ToString("yyyy-MM-ddTHH:mm")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting article {Id}", id);
                return Json(new { success = false, message = "載入文章資料時發生錯誤" });
            }
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllArticleCategoriesAsync();
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Article article)
        {
            try
            {
                // 新增時將 ID 設為 0
                article.ID = 0;
                _logger.LogInformation("Create method called with Title: {Title}", article.Title);
                
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogWarning("ModelState is invalid: {Errors}", string.Join(", ", errors));
                    TempData["ErrorMessage"] = "請檢查輸入的資料: " + string.Join(", ", errors);
                    return RedirectToAction("Index");
                }

                _logger.LogInformation("Calling CreateArticleAsync for Title: {Title}", article.Title);
                var result = await _articleService.CreateArticleAsync(article);
                
                if (result)
                {
                    _logger.LogInformation("Article created successfully: {Title}", article.Title);
                    TempData["SuccessMessage"] = "文章建立成功";
                }
                else
                {
                    _logger.LogWarning("Article creation failed: {Title}", article.Title);
                    TempData["ErrorMessage"] = "文章建立失敗";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating article: {Title}", article.Title);
                TempData["ErrorMessage"] = "建立文章時發生錯誤: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            var categories = await _categoryService.GetAllArticleCategoriesAsync();
            ViewBag.Categories = categories;
            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Article article)
        {
            try
            {
                _logger.LogInformation("Edit method called with ID: {ID}, Title: {Title}", article.ID, article.Title);
                
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogWarning("ModelState is invalid: {Errors}", string.Join(", ", errors));
                    TempData["ErrorMessage"] = "請檢查輸入的資料: " + string.Join(", ", errors);
                    return RedirectToAction("Index");
                }

                _logger.LogInformation("Calling UpdateArticleAsync for ID: {ID}, Title: {Title}", article.ID, article.Title);
                var result = await _articleService.UpdateArticleAsync(article);
                
                if (result)
                {
                    _logger.LogInformation("Article updated successfully: ID {ID}, Title: {Title}", article.ID, article.Title);
                    TempData["SuccessMessage"] = "文章更新成功";
                }
                else
                {
                    _logger.LogWarning("Article update failed: ID {ID}, Title: {Title}", article.ID, article.Title);
                    TempData["ErrorMessage"] = "文章更新失敗";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating article: ID {ID}, Title: {Title}", article.ID, article.Title);
                TempData["ErrorMessage"] = "更新文章時發生錯誤: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var result = await _articleService.ToggleArticleStatusAsync(id);
                if (result)
                {
                    return Json(new { success = true, message = "狀態更新成功" });
                }
                else
                {
                    return Json(new { success = false, message = "狀態更新失敗" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling article status");
                return Json(new { success = false, message = "更新狀態時發生錯誤" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _articleService.DeleteArticleAsync(id);
                if (success)
                {
                    return Json(new { success = true, message = "文章刪除成功" });
                }
                else
                {
                    return Json(new { success = false, message = "刪除文章時發生錯誤" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting article");
                return Json(new { success = false, message = "刪除文章時發生錯誤" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return Json(new { success = false, message = "請選擇要上傳的檔案" });
                }

                // 檢查檔案大小 (10MB)
                if (file.Length > 10 * 1024 * 1024)
                {
                    return Json(new { success = false, message = "檔案大小不能超過 10MB" });
                }

                // 檢查檔案類型
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return Json(new { success = false, message = "不支援的檔案類型" });
                }

                // 創建上傳目錄
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "article");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // 生成唯一檔案名
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadPath, fileName);
                var relativePath = $"/uploads/article/{fileName}";

                // 儲存檔案
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("檔案上傳成功: {FileName} -> {FilePath}", file.FileName, relativePath);

                return Json(new { success = true, filePath = relativePath, fileName = file.FileName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file: {FileName}", file?.FileName);
                return Json(new { success = false, message = "檔案上傳失敗: " + ex.Message });
            }
        }
    }
}
