using FriendlySeed.Models;

namespace FriendlySeed.Services
{
    public interface ITeacherRoleService
    {
        Task<IEnumerable<TeacherRole>> GetAllTeacherRolesAsync();
        Task<TeacherRole?> GetTeacherRoleByIdAsync(int id);
        Task<bool> CreateTeacherRoleAsync(TeacherRole teacherRole);
        Task<bool> UpdateTeacherRoleAsync(TeacherRole teacherRole);
        Task<bool> DeleteTeacherRoleAsync(int id);
        Task<bool> ToggleTeacherRoleStatusAsync(int id);
        Task<IEnumerable<TeacherRole>> GetActiveTeacherRolesAsync();
    }
}

