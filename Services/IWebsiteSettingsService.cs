using FriendlySeed.Models;

namespace FriendlySeed.Services
{
    public interface IWebsiteSettingsService
    {
        Task<WebsiteSettings?> GetWebsiteSettingsAsync();
        Task<bool> UpdateWebsiteSettingsAsync(WebsiteSettings settings);
        Task<bool> CreateWebsiteSettingsAsync(WebsiteSettings settings);
    }
}
