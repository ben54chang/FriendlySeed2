using Dapper;
using FriendlySeed.Data;
using FriendlySeed.Models;

namespace FriendlySeed.Services
{
    public class WebsiteSettingsService : IWebsiteSettingsService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public WebsiteSettingsService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<WebsiteSettings?> GetWebsiteSettingsAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT TOP 1 * FROM WebsiteSettings WHERE IsActive = 1 ORDER BY CreatedTime DESC";
            return await connection.QueryFirstOrDefaultAsync<WebsiteSettings>(sql);
        }

        public async Task<bool> UpdateWebsiteSettingsAsync(WebsiteSettings settings)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            // 先檢查是否存在記錄
            var existingSettings = await GetWebsiteSettingsAsync();
            
            if (existingSettings != null)
            {
                // 更新現有記錄
                const string updateSql = @"
                    UPDATE WebsiteSettings 
                    SET WebsiteName = @WebsiteName, CopyrightText = @CopyrightText, ContactEmail = @ContactEmail,
                        MetaKeywords = @MetaKeywords, MetaDescription = @MetaDescription, IsActive = @IsActive, 
                        UpdatedTime = @UpdatedTime
                    WHERE ID = @ID";
                
                settings.ID = existingSettings.ID;
                settings.UpdatedTime = DateTime.Now;
                var result = await connection.ExecuteAsync(updateSql, settings);
                return result > 0;
            }
            else
            {
                // 建立新記錄
                const string insertSql = @"
                    INSERT INTO WebsiteSettings (WebsiteName, CopyrightText, ContactEmail, 
                        MetaKeywords, MetaDescription, IsActive, CreatedTime, UpdatedTime)
                    VALUES (@WebsiteName, @CopyrightText, @ContactEmail, 
                        @MetaKeywords, @MetaDescription, @IsActive, @CreatedTime, @UpdatedTime)";
                
                settings.CreatedTime = DateTime.Now;
                settings.UpdatedTime = DateTime.Now;
                var result = await connection.ExecuteAsync(insertSql, settings);
                return result > 0;
            }
        }

        public async Task<bool> CreateWebsiteSettingsAsync(WebsiteSettings settings)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO WebsiteSettings (WebsiteName, CopyrightText, ContactEmail, Favicon, Logo, OgImage, 
                    ContentSecurityPolicy, PageTitle, MetaKeywords, MetaDescription, OtherMetaTags, 
                    GoogleAnalytics, GoogleTagManager, FacebookPixel, BlockSearchEngine, IsActive, CreatedTime, UpdatedTime)
                VALUES (@WebsiteName, @CopyrightText, @ContactEmail, @Favicon, @Logo, @OgImage, 
                    @ContentSecurityPolicy, @PageTitle, @MetaKeywords, @MetaDescription, @OtherMetaTags, 
                    @GoogleAnalytics, @GoogleTagManager, @FacebookPixel, @BlockSearchEngine, @IsActive, @CreatedTime, @UpdatedTime)";
            
            var result = await connection.ExecuteAsync(sql, settings);
            return result > 0;
        }
    }
}
