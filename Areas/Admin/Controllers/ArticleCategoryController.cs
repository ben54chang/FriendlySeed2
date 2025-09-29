using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FriendlySeed.Services;
using FriendlySeed.Models;

namespace FriendlySeed.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ArticleCategoryController : Controller
    {
        private readonly IArticleCategoryService _articleCategoryService;
        private readonly ILogger<ArticleCategoryController> _logger;

        public ArticleCategoryController(IArticleCategoryService articleCategoryService, ILogger<ArticleCategoryController> logger)
        {
            _articleCategoryService = articleCategoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Starting to load article categories");
                var categories = await _articleCategoryService.GetAllArticleCategoriesAsync();
                _logger.LogInformation("Successfully loaded {Count} article categories", categories.Count());
                return View(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading article categories: {Message}", ex.Message);
                _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
                TempData["ErrorMessage"] = $"載入文章分類資料時發生錯誤: {ex.Message}";
                return View(new List<ArticleCategory>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleCategory model)
        {
            try
            {
                // 新增時將 ArticleCategoryID 設為 0
                model.ArticleCategoryID = 0;
                _logger.LogInformation("Create method called with ArticleCategoryName: {ArticleCategoryName}", model.ArticleCategoryName);
                
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    var modelErrors = ModelState.Where(x => x.Value.Errors.Count > 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
                    _logger.LogWarning("ModelState is invalid: {Errors}", string.Join(", ", errors));
                    _logger.LogWarning("ModelState details: {ModelErrors}", string.Join(", ", modelErrors.Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value)}")));
                    _logger.LogWarning("Model values: ArticleCategoryName={ArticleCategoryName}, SortOrder={SortOrder}, IsActive={IsActive}", 
                        model.ArticleCategoryName, model.SortOrder, model.IsActive);
                    TempData["ErrorMessage"] = "請檢查輸入的資料: " + string.Join(", ", errors);
                    return RedirectToAction("Index");
                }

                _logger.LogInformation("Calling CreateArticleCategoryAsync for ArticleCategoryName: {ArticleCategoryName}", model.ArticleCategoryName);
                var result = await _articleCategoryService.CreateArticleCategoryAsync(model);
                
                if (result)
                {
                    _logger.LogInformation("ArticleCategory created successfully: {ArticleCategoryName}", model.ArticleCategoryName);
                    TempData["SuccessMessage"] = "文章分類建立成功";
                }
                else
                {
                    _logger.LogWarning("ArticleCategory creation failed: {ArticleCategoryName}", model.ArticleCategoryName);
                    TempData["ErrorMessage"] = "文章分類建立失敗";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating article category: {ArticleCategoryName}", model.ArticleCategoryName);
                TempData["ErrorMessage"] = "建立文章分類時發生錯誤: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArticleCategory model)
        {
            try
            {
                _logger.LogInformation("Edit method called with ArticleCategoryID: {ArticleCategoryID}, ArticleCategoryName: {ArticleCategoryName}", model.ArticleCategoryID, model.ArticleCategoryName);
                
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    var modelErrors = ModelState.Where(x => x.Value.Errors.Count > 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
                    _logger.LogWarning("ModelState is invalid: {Errors}", string.Join(", ", errors));
                    _logger.LogWarning("ModelState details: {ModelErrors}", string.Join(", ", modelErrors.Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value)}")));
                    _logger.LogWarning("Model values: ArticleCategoryName={ArticleCategoryName}, SortOrder={SortOrder}, IsActive={IsActive}", 
                        model.ArticleCategoryName, model.SortOrder, model.IsActive);
                    TempData["ErrorMessage"] = "請檢查輸入的資料: " + string.Join(", ", errors);
                    return RedirectToAction("Index");
                }

                _logger.LogInformation("Calling UpdateArticleCategoryAsync for ArticleCategoryID: {ArticleCategoryID}, ArticleCategoryName: {ArticleCategoryName}", model.ArticleCategoryID, model.ArticleCategoryName);
                var result = await _articleCategoryService.UpdateArticleCategoryAsync(model);
                
                if (result)
                {
                    _logger.LogInformation("ArticleCategory updated successfully: ArticleCategoryID {ArticleCategoryID}, ArticleCategoryName: {ArticleCategoryName}", model.ArticleCategoryID, model.ArticleCategoryName);
                    TempData["SuccessMessage"] = "文章分類更新成功";
                }
                else
                {
                    _logger.LogWarning("ArticleCategory update failed: ArticleCategoryID {ArticleCategoryID}, ArticleCategoryName: {ArticleCategoryName}", model.ArticleCategoryID, model.ArticleCategoryName);
                    TempData["ErrorMessage"] = "文章分類更新失敗";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating article category: ArticleCategoryID {ArticleCategoryID}, ArticleCategoryName: {ArticleCategoryName}", model.ArticleCategoryID, model.ArticleCategoryName);
                TempData["ErrorMessage"] = "更新文章分類時發生錯誤: " + ex.Message;
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var result = await _articleCategoryService.ToggleArticleCategoryStatusAsync(id);
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
                _logger.LogError(ex, "Error toggling article category status");
                return Json(new { success = false, message = "更新狀態時發生錯誤" });
            }
        }

        [HttpGet]
        public IActionResult TestEdit()
        {
            return View(new ArticleCategory());
        }

        [HttpGet]
        public IActionResult SimpleEdit()
        {
            return View(new ArticleCategory());
        }

        [HttpGet]
        public IActionResult DebugTest()
        {
            return View(new ArticleCategory());
        }

        [HttpGet]
        public IActionResult TestSimple()
        {
            return View();
        }

        [HttpGet]
        public IActionResult MinimalTest()
        {
            return View();
        }
    }
}
