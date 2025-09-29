using Dapper;
using FriendlySeed.Data;
using FriendlySeed.Models;

namespace FriendlySeed.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ArticleService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Article>> GetAllArticlesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT a.*, ac.ArticleCategoryName 
                FROM Article a 
                LEFT JOIN ArticleCategory ac ON a.CategoryID = ac.ArticleCategoryID 
                WHERE a.IsActive = 1 
                ORDER BY a.CreatedTime DESC";
            
            return await connection.QueryAsync<Article, ArticleCategory, Article>(sql, (article, category) =>
            {
                article.Category = category;
                return article;
            }, splitOn: "ArticleCategoryName");
        }

        public async Task<Article?> GetArticleByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT a.*, ac.ArticleCategoryName 
                FROM Article a 
                LEFT JOIN ArticleCategory ac ON a.CategoryID = ac.ArticleCategoryID 
                WHERE a.ID = @Id AND a.IsActive = 1";
            
            var result = await connection.QueryAsync<Article, ArticleCategory, Article>(sql, (article, category) =>
            {
                article.Category = category;
                return article;
            }, new { Id = id }, splitOn: "ArticleCategoryName");
            
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<Article>> GetArticlesByCategoryAsync(int categoryId)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT a.*, ac.ArticleCategoryName 
                FROM Article a 
                LEFT JOIN ArticleCategory ac ON a.CategoryID = ac.ArticleCategoryID 
                WHERE a.CategoryID = @CategoryId AND a.IsActive = 1 
                ORDER BY a.CreatedTime DESC";
            
            return await connection.QueryAsync<Article, ArticleCategory, Article>(sql, (article, category) =>
            {
                article.Category = category;
                return article;
            }, new { CategoryId = categoryId }, splitOn: "ArticleCategoryName");
        }

        public async Task<bool> CreateArticleAsync(Article article)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO Article (Alias, CategoryID, Author, Title, Keywords, Description, FilePath, SortOrder, PublishTime, IsTop, IsActive, CreatedTime, UpdatedTime)
                VALUES (@Alias, @CategoryID, @Author, @Title, @Keywords, @Description, @FilePath, @SortOrder, @PublishTime, @IsTop, @IsActive, @CreatedTime, @UpdatedTime)";
            
            var result = await connection.ExecuteAsync(sql, article);
            return result > 0;
        }

        public async Task<bool> UpdateArticleAsync(Article article)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE Article 
                SET Alias = @Alias, CategoryID = @CategoryID, Author = @Author, Title = @Title, 
                    Keywords = @Keywords, Description = @Description, FilePath = @FilePath, 
                    SortOrder = @SortOrder, PublishTime = @PublishTime, IsTop = @IsTop, 
                    IsActive = @IsActive, UpdatedTime = @UpdatedTime
                WHERE ID = @ID";
            
            var result = await connection.ExecuteAsync(sql, article);
            return result > 0;
        }

        public async Task<bool> DeleteArticleAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "UPDATE Article SET IsActive = 0, UpdatedTime = @UpdatedTime WHERE ID = @Id";
            
            var result = await connection.ExecuteAsync(sql, new { Id = id, UpdatedTime = DateTime.Now });
            return result > 0;
        }

        public async Task<bool> ToggleArticleStatusAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE Article 
                SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END, 
                    UpdatedTime = @UpdatedTime 
                WHERE ID = @Id";
            
            var result = await connection.ExecuteAsync(sql, new { Id = id, UpdatedTime = DateTime.Now });
            return result > 0;
        }

        public async Task<IEnumerable<Article>> GetTopArticlesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT a.*, ac.ArticleCategoryName 
                FROM Article a 
                LEFT JOIN ArticleCategory ac ON a.CategoryID = ac.ArticleCategoryID 
                WHERE a.IsTop = 1 AND a.IsActive = 1 
                ORDER BY a.SortOrder, a.CreatedTime DESC";
            
            return await connection.QueryAsync<Article, ArticleCategory, Article>(sql, (article, category) =>
            {
                article.Category = category;
                return article;
            }, splitOn: "ArticleCategoryName");
        }

        public async Task<IEnumerable<Article>> GetPublishedArticlesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT a.*, ac.ArticleCategoryName 
                FROM Article a 
                LEFT JOIN ArticleCategory ac ON a.CategoryID = ac.ArticleCategoryID 
                WHERE a.PublishTime IS NOT NULL AND a.PublishTime <= @Now AND a.IsActive = 1 
                ORDER BY a.PublishTime DESC";
            
            return await connection.QueryAsync<Article, ArticleCategory, Article>(sql, (article, category) =>
            {
                article.Category = category;
                return article;
            }, new { Now = DateTime.Now }, splitOn: "ArticleCategoryName");
        }
    }
}
