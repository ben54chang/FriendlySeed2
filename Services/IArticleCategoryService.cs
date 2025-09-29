using FriendlySeed.Models;

namespace FriendlySeed.Services
{
    public interface IArticleCategoryService
    {
        Task<IEnumerable<ArticleCategory>> GetAllArticleCategoriesAsync();
        Task<ArticleCategory?> GetArticleCategoryByIdAsync(int id);
        Task<bool> CreateArticleCategoryAsync(ArticleCategory articleCategory);
        Task<bool> UpdateArticleCategoryAsync(ArticleCategory articleCategory);
        Task<bool> DeleteArticleCategoryAsync(int id);
        Task<bool> ToggleArticleCategoryStatusAsync(int id);
    }
}