using FriendlySeed.Models;

namespace FriendlySeed.Services
{
    public interface IEmailSettingsService
    {
        Task<EmailSettings?> GetEmailSettingsAsync();
        Task<bool> UpdateEmailSettingsAsync(EmailSettings settings);
        Task<bool> CreateEmailSettingsAsync(EmailSettings settings);
    }
}
