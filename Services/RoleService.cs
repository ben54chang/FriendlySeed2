using Dapper;
using FriendlySeed.Data;
using FriendlySeed.Models;
using System.Data;

namespace FriendlySeed.Services
{
    public class RoleService : IRoleService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RoleService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM Role ORDER BY CreatedTime DESC";
            return await connection.QueryAsync<Role>(sql);
        }

        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM Role WHERE RoleID = @Id";
            return await connection.QueryFirstOrDefaultAsync<Role>(sql, new { Id = id });
        }

        public async Task<bool> CreateRoleAsync(Role role)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO Role (RoleName, IsActive, CreatedTime, UpdatedTime)
                VALUES (@RoleName, @IsActive, @CreatedTime, @UpdatedTime);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            
            // 側錄 SQL 語法
            Console.WriteLine("=== CreateRole SQL ===");
            Console.WriteLine($"SQL: {sql}");
            Console.WriteLine($"Parameters: RoleName={role.RoleName}, IsActive={role.IsActive}, CreatedTime={role.CreatedTime}, UpdatedTime={role.UpdatedTime}");
            
            var newRoleId = await connection.QuerySingleAsync<int>(sql, role);
            role.RoleID = newRoleId;
            
            Console.WriteLine($"New RoleID: {newRoleId}");
            Console.WriteLine("=====================");
            
            return newRoleId > 0;
        }

        public async Task<bool> CreateRoleWithMenusAsync(Role role, List<int> menuIds)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            // 側錄 SQL 語法
            Console.WriteLine("=== CreateRoleWithMenus Store Procedure ===");
            Console.WriteLine($"RoleName: {role.RoleName}, IsActive: {role.IsActive}");
            Console.WriteLine($"MenuIds: {string.Join(",", menuIds ?? new List<int>())}");
            
            var parameters = new DynamicParameters();
            parameters.Add("@RoleName", role.RoleName);
            parameters.Add("@IsActive", role.IsActive);
            parameters.Add("@MenuIds", menuIds != null && menuIds.Any() ? string.Join(",", menuIds) : (string)null);
            parameters.Add("@NewRoleID", dbType: DbType.Int32, direction: ParameterDirection.Output);
            
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>("sp_CreateRoleWithMenus", parameters, commandType: CommandType.StoredProcedure);
            
            role.RoleID = parameters.Get<int>("@NewRoleID");
            
            Console.WriteLine($"New RoleID: {role.RoleID}");
            Console.WriteLine($"Store Procedure Result: {result?.Result}");
            Console.WriteLine("=====================");
            
            return role.RoleID > 0;
        }

        public async Task<bool> UpdateRoleAsync(Role role)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE Role 
                SET RoleName = @RoleName, IsActive = @IsActive, UpdatedTime = @UpdatedTime
                WHERE RoleID = @RoleID";
            
            var result = await connection.ExecuteAsync(sql, role);
            return result > 0;
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "UPDATE Role SET IsActive = 0, UpdatedTime = @UpdatedTime WHERE RoleID = @Id";
            
            var result = await connection.ExecuteAsync(sql, new { Id = id, UpdatedTime = DateTime.Now });
            return result > 0;
        }

        public async Task<bool> ToggleRoleStatusAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE Role 
                SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END,
                    UpdatedTime = @UpdatedTime
                WHERE RoleID = @Id";

            var result = await connection.ExecuteAsync(sql, new { Id = id, UpdatedTime = DateTime.Now });
            return result > 0;
        }

        public async Task<IEnumerable<Role>> GetActiveRolesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM Role WHERE IsActive = 1 ORDER BY RoleName";
            return await connection.QueryAsync<Role>(sql);
        }

        public async Task<Role?> GetRoleWithMenusAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM Role WHERE RoleID = @Id";
            var role = await connection.QueryFirstOrDefaultAsync<Role>(sql, new { Id = id });
            
            if (role != null)
            {
                // 取得角色的選單權限（只包含子選單 - ParentID IS NOT NULL）
                const string menuSql = @"
                    SELECT m.MenuID, m.MenuName, m.ParentID, m.SortOrder, m.MenuUrl, m.IconUrl
                    FROM Menu m
                    INNER JOIN MenuRole mr ON m.MenuID = mr.MenuID
                    WHERE mr.RoleID = @RoleId AND mr.IsActive = 1 AND m.IsActive = 1 AND m.ParentID IS NOT NULL
                    ORDER BY m.SortOrder";
                
                var menus = await connection.QueryAsync<Menu>(menuSql, new { RoleId = id });
                role.AvailableMenus = menus.ToList();
                role.SelectedMenuIds = menus.Select(m => m.MenuID).ToList();
            }
            
            return role;
        }

        public async Task<bool> UpdateRoleWithMenusAsync(Role role, List<int> menuIds)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            // 側錄 SQL 語法
            Console.WriteLine("=== UpdateRoleWithMenus Store Procedure ===");
            Console.WriteLine($"Role: RoleID={role.RoleID}, RoleName={role.RoleName}, IsActive={role.IsActive}");
            Console.WriteLine($"MenuIds: {string.Join(",", menuIds ?? new List<int>())}");
            
            var parameters = new DynamicParameters();
            parameters.Add("@RoleID", role.RoleID);
            parameters.Add("@RoleName", role.RoleName);
            parameters.Add("@IsActive", role.IsActive);
            parameters.Add("@MenuIds", menuIds != null && menuIds.Any() ? string.Join(",", menuIds) : (string)null);
            
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>("sp_UpdateRoleWithMenus", parameters, commandType: CommandType.StoredProcedure);
            
            Console.WriteLine($"Store Procedure Result: {result?.Result}");
            Console.WriteLine($"Store Procedure Message: {result?.Message}");
            Console.WriteLine("===============================");
            
            return result?.Result == "SUCCESS";
        }
    }
}
