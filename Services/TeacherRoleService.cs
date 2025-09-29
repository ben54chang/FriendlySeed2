using Dapper;
using FriendlySeed.Data;
using FriendlySeed.Models;

namespace FriendlySeed.Services
{
    public class TeacherRoleService : ITeacherRoleService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TeacherRoleService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<TeacherRole>> GetAllTeacherRolesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM TeacherRole ORDER BY CreatedTime DESC";
            return await connection.QueryAsync<TeacherRole>(sql);
        }

        public async Task<TeacherRole?> GetTeacherRoleByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM TeacherRole WHERE TeacherRoleID = @Id";
            return await connection.QueryFirstOrDefaultAsync<TeacherRole>(sql, new { Id = id });
        }

        public async Task<bool> CreateTeacherRoleAsync(TeacherRole teacherRole)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO TeacherRole (TeacherRoleName, IsActive, CreatedTime, UpdatedTime)
                VALUES (@TeacherRoleName, @IsActive, @CreatedTime, @UpdatedTime)";
            
            var result = await connection.ExecuteAsync(sql, teacherRole);
            return result > 0;
        }

        public async Task<bool> UpdateTeacherRoleAsync(TeacherRole teacherRole)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE TeacherRole 
                SET TeacherRoleName = @TeacherRoleName, IsActive = @IsActive, UpdatedTime = @UpdatedTime
                WHERE TeacherRoleID = @TeacherRoleID";
            
            var result = await connection.ExecuteAsync(sql, teacherRole);
            return result > 0;
        }

        public async Task<bool> DeleteTeacherRoleAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "DELETE FROM TeacherRole WHERE TeacherRoleID = @Id";
            var result = await connection.ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }

        public async Task<bool> ToggleTeacherRoleStatusAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE TeacherRole 
                SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END,
                    UpdatedTime = GETDATE()
                WHERE TeacherRoleID = @Id";
            
            var result = await connection.ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }

        public async Task<IEnumerable<TeacherRole>> GetActiveTeacherRolesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM TeacherRole WHERE IsActive = 1 ORDER BY TeacherRoleName";
            return await connection.QueryAsync<TeacherRole>(sql);
        }
    }
}

