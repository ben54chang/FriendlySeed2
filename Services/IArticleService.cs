using FriendlySeed.Models;

namespace FriendlySeed.Services
{
    public interface IArticleService
    {
        Task<IEnumerable<Article>> GetAllArticlesAsync();
        Task<Article?> GetArticleByIdAsync(int id);
        Task<IEnumerable<Article>> GetArticlesByCategoryAsync(int categoryId);
        Task<bool> CreateArticleAsync(Article article);
        Task<bool> UpdateArticleAsync(Article article);
        Task<bool> DeleteArticleAsync(int id);
        Task<bool> ToggleArticleStatusAsync(int id);
        Task<IEnumerable<Article>> GetTopArticlesAsync();
        Task<IEnumerable<Article>> GetPublishedArticlesAsync();
    }
}
