using Dapper;
using FriendlySeed.Data;
using FriendlySeed.Models;

namespace FriendlySeed.Services
{
    public class EmailSettingsService : IEmailSettingsService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public EmailSettingsService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<EmailSettings?> GetEmailSettingsAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT TOP 1 * FROM EmailSettings WHERE IsActive = 1 ORDER BY CreatedTime DESC";
            return await connection.QueryFirstOrDefaultAsync<EmailSettings>(sql);
        }

        public async Task<bool> UpdateEmailSettingsAsync(EmailSettings settings)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            // 先檢查是否存在記錄
            var existingSettings = await GetEmailSettingsAsync();
            
            if (existingSettings != null)
            {
                // 更新現有記錄
                const string updateSql = @"
                    UPDATE EmailSettings 
                    SET SendMethod = @SendMethod, SenderEmail = @SenderEmail, SenderName = @SenderName,
                        SmtpHost = @SmtpHost, SmtpPort = @SmtpPort, EncryptionType = @EncryptionType,
                        SmtpUsername = @SmtpUsername, SmtpPassword = @SmtpPassword, SendmailConfig = @SendmailConfig,
                        SendmailPath = @SendmailPath, UseGmailAPI = @UseGmailAPI, GmailProjectID = @GmailProjectID,
                        GmailClientID = @GmailClientID, GmailClientSecret = @GmailClientSecret, 
                        GmailRedirectUrl = @GmailRedirectUrl, ApiKey = @ApiKey, IsActive = @IsActive, UpdatedTime = @UpdatedTime
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
                    INSERT INTO EmailSettings (SendMethod, SenderEmail, SenderName, SmtpHost, SmtpPort, EncryptionType,
                        SmtpUsername, SmtpPassword, SendmailConfig, SendmailPath, UseGmailAPI, GmailProjectID,
                        GmailClientID, GmailClientSecret, GmailRedirectUrl, ApiKey, IsActive, CreatedTime, UpdatedTime)
                    VALUES (@SendMethod, @SenderEmail, @SenderName, @SmtpHost, @SmtpPort, @EncryptionType,
                        @SmtpUsername, @SmtpPassword, @SendmailConfig, @SendmailPath, @UseGmailAPI, @GmailProjectID,
                        @GmailClientID, @GmailClientSecret, @GmailRedirectUrl, @ApiKey, @IsActive, @CreatedTime, @UpdatedTime)";
                
                settings.CreatedTime = DateTime.Now;
                settings.UpdatedTime = DateTime.Now;
                var result = await connection.ExecuteAsync(insertSql, settings);
                return result > 0;
            }
        }

        public async Task<bool> CreateEmailSettingsAsync(EmailSettings settings)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO EmailSettings (SendMethod, SenderEmail, SenderName, SmtpHost, SmtpPort, EncryptionType,
                    SmtpUsername, SmtpPassword, SendmailConfig, SendmailPath, UseGmailAPI, GmailProjectID,
                    GmailClientID, GmailClientSecret, GmailRedirectUrl, ApiKey, IsActive, CreatedTime, UpdatedTime)
                VALUES (@SendMethod, @SenderEmail, @SenderName, @SmtpHost, @SmtpPort, @EncryptionType,
                    @SmtpUsername, @SmtpPassword, @SendmailConfig, @SendmailPath, @UseGmailAPI, @GmailProjectID,
                    @GmailClientID, @GmailClientSecret, @GmailRedirectUrl, @ApiKey, @IsActive, @CreatedTime, @UpdatedTime)";
            
            var result = await connection.ExecuteAsync(sql, settings);
            return result > 0;
        }
    }
}
