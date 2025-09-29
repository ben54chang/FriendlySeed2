using Microsoft.AspNetCore.Mvc;
using FriendlySeed.Services;

namespace FriendlySeed.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IArticleCategoryService _categoryService;

        public ArticleController(IArticleService articleService, IArticleCategoryService categoryService)
        {
            _articleService = articleService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int? categoryId, string? search)
        {
            try
            {
                IEnumerable<Models.Article> articles;

                if (categoryId.HasValue)
                {
                    articles = await _articleService.GetArticlesByCategoryAsync(categoryId.Value);
                }
                else
                {
                    articles = await _articleService.GetPublishedArticlesAsync();
                }

                // 如果有搜尋條件，進行篩選
                if (!string.IsNullOrEmpty(search))
                {
                    articles = articles.Where(a => 
                        a.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        a.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        a.Author.Contains(search, StringComparison.OrdinalIgnoreCase));
                }

                var categories = await _categoryService.GetAllArticleCategoriesAsync();
                
                ViewBag.Categories = categories ?? new List<Models.ArticleCategory>();
                ViewBag.SelectedCategoryId = categoryId;
                ViewBag.SearchTerm = search;

                return View(articles ?? new List<Models.Article>());
            }
            catch (Exception ex)
            {
                // 記錄錯誤但不中斷頁面載入
                ViewBag.Categories = new List<Models.ArticleCategory>();
                return View(new List<Models.Article>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var article = await _articleService.GetArticleByIdAsync(id);
                if (article == null)
                {
                    return NotFound();
                }

                return View(article);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
    }
}
