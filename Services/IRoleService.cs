using FriendlySeed.Models;

namespace FriendlySeed.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role?> GetRoleByIdAsync(int id);
        Task<bool> CreateRoleAsync(Role role);
        Task<bool> CreateRoleWithMenusAsync(Role role, List<int> menuIds);
        Task<bool> UpdateRoleAsync(Role role);
        Task<bool> DeleteRoleAsync(int id);
        Task<bool> ToggleRoleStatusAsync(int id);
        Task<IEnumerable<Role>> GetActiveRolesAsync();
        Task<Role?> GetRoleWithMenusAsync(int id);
        Task<bool> UpdateRoleWithMenusAsync(Role role, List<int> menuIds);
    }
}
