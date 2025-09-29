using Dapper;
using FriendlySeed.Data;
using FriendlySeed.Models;

namespace FriendlySeed.Services
{
    public class ArticleCategoryService : IArticleCategoryService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ArticleCategoryService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ArticleCategory>> GetAllArticleCategoriesAsync()
        {
            try
            {
                Console.WriteLine("Starting GetAllArticleCategoriesAsync");
                using var connection = _connectionFactory.CreateConnection();
                Console.WriteLine("Connection created successfully");
                
                const string sql = "SELECT * FROM ArticleCategory ORDER BY IsActive DESC, SortOrder, CreatedTime";
                Console.WriteLine($"Executing SQL: {sql}");
                
                var categories = await connection.QueryAsync<ArticleCategory>(sql);
                Console.WriteLine($"Query executed successfully, found {categories?.Count() ?? 0} categories");
                
                return categories ?? new List<ArticleCategory>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllArticleCategoriesAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<ArticleCategory?> GetArticleCategoryByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM ArticleCategory WHERE ArticleCategoryID = @Id";
            return await connection.QueryFirstOrDefaultAsync<ArticleCategory>(sql, new { Id = id });
        }

        public async Task<bool> CreateArticleCategoryAsync(ArticleCategory articleCategory)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                const string sql = @"
                    INSERT INTO ArticleCategory (ArticleCategoryName, CategoryKeywords, CategoryDescription, SortOrder, IsActive)
                    VALUES (@ArticleCategoryName, @CategoryKeywords, @CategoryDescription, @SortOrder, @IsActive)";
                
                // 確保數值欄位有正確的值
                if (articleCategory.SortOrder <= 0)
                {
                    articleCategory.SortOrder = 10;
                }
                
                Console.WriteLine($"Creating article category: {articleCategory.ArticleCategoryName}, SortOrder: {articleCategory.SortOrder}, IsActive: {articleCategory.IsActive}");
                
                var result = await connection.ExecuteAsync(sql, articleCategory);
                Console.WriteLine($"Insert result: {result} rows affected");
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateArticleCategoryAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateArticleCategoryAsync(ArticleCategory articleCategory)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                const string sql = @"
                    UPDATE ArticleCategory 
                    SET ArticleCategoryName = @ArticleCategoryName, 
                        CategoryKeywords = @CategoryKeywords, 
                        CategoryDescription = @CategoryDescription, 
                        SortOrder = @SortOrder, 
                        IsActive = @IsActive, 
                        UpdatedTime = GETDATE()
                    WHERE ArticleCategoryID = @ArticleCategoryID";
                
                // 確保數值欄位有正確的值
                if (articleCategory.SortOrder <= 0)
                {
                    articleCategory.SortOrder = 10;
                }
                
                // 先檢查記錄是否存在
                var checkSql = "SELECT COUNT(*) FROM ArticleCategory WHERE ArticleCategoryID = @ArticleCategoryID";
                var exists = await connection.QuerySingleAsync<int>(checkSql, new { ArticleCategoryID = articleCategory.ArticleCategoryID });
                Console.WriteLine($"Record exists check: {exists} records found for ArticleCategoryID={articleCategory.ArticleCategoryID}");
                
                if (exists == 0)
                {
                    Console.WriteLine("ERROR: ArticleCategory record not found!");
                    return false;
                }
                
                Console.WriteLine($"Updating article category: ArticleCategoryID={articleCategory.ArticleCategoryID}, ArticleCategoryName={articleCategory.ArticleCategoryName}, IsActive={articleCategory.IsActive}");
                Console.WriteLine($"SQL: {sql}");
                Console.WriteLine($"Parameters: ArticleCategoryID={articleCategory.ArticleCategoryID}, ArticleCategoryName={articleCategory.ArticleCategoryName}, CategoryKeywords={articleCategory.CategoryKeywords}, CategoryDescription={articleCategory.CategoryDescription}, SortOrder={articleCategory.SortOrder}, IsActive={articleCategory.IsActive}, UpdatedTime={articleCategory.UpdatedTime}");
                
                var result = await connection.ExecuteAsync(sql, articleCategory);
                Console.WriteLine($"Update result: {result} rows affected");
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateArticleCategoryAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteArticleCategoryAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "UPDATE ArticleCategory SET IsActive = 0, UpdatedTime = GETDATE() WHERE ArticleCategoryID = @Id";
            
            var result = await connection.ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }

        public async Task<bool> ToggleArticleCategoryStatusAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE ArticleCategory 
                SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END, 
                    UpdatedTime = GETDATE() 
                WHERE ArticleCategoryID = @Id";
            
            var result = await connection.ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }
    }
}