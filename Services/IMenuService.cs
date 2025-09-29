using FriendlySeed.Models;

namespace FriendlySeed.Services
{
    public interface IMenuService
    {
        Task<IEnumerable<Menu>> GetAllMenusAsync();
        Task<IEnumerable<Menu>> GetActiveMenusAsync();
        Task<Menu?> GetMenuByIdAsync(int id);
        Task<bool> CreateMenuAsync(Menu menu);
        Task<bool> UpdateMenuAsync(Menu menu);
        Task<bool> DeleteMenuAsync(int id);
        Task<bool> ToggleMenuStatusAsync(int id);
        Task<IEnumerable<Menu>> GetMenuTreeAsync();
    }
}