using FriendlySeed.Models;

namespace FriendlySeed.Services
{
    public interface IMenuRoleService
    {
        Task<IEnumerable<MenuRole>> GetMenuRolesByRoleIdAsync(int roleId);
        Task<IEnumerable<MenuRole>> GetMenuRolesByMenuIdAsync(int menuId);
        Task<bool> CreateMenuRoleAsync(MenuRole menuRole);
        Task<bool> UpdateMenuRoleAsync(MenuRole menuRole);
        Task<bool> DeleteMenuRoleAsync(int menuId, int roleId);
        Task<bool> DeleteMenuRolesByRoleIdAsync(int roleId);
        Task<bool> SetRoleMenusAsync(int roleId, List<int> menuIds);
        Task<List<int>> GetMenuIdsByRoleIdAsync(int roleId);
    }
}

