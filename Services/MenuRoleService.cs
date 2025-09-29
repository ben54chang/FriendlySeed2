using Dapper;
using FriendlySeed.Data;
using FriendlySeed.Models;

namespace FriendlySeed.Services
{
    public class MenuRoleService : IMenuRoleService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MenuRoleService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<MenuRole>> GetMenuRolesByRoleIdAsync(int roleId)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT mr.*, m.MenuName 
                FROM MenuRole mr 
                LEFT JOIN Menu m ON mr.MenuID = m.MenuID 
                WHERE mr.RoleID = @RoleId AND mr.IsActive = 1 
                ORDER BY m.SortOrder";
            
            return await connection.QueryAsync<MenuRole, Menu, MenuRole>(sql, (menuRole, menu) =>
            {
                menuRole.Menu = menu;
                return menuRole;
            }, new { RoleId = roleId }, splitOn: "MenuName");
        }

        public async Task<IEnumerable<MenuRole>> GetMenuRolesByMenuIdAsync(int menuId)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT mr.*, r.RoleName 
                FROM MenuRole mr 
                LEFT JOIN Role r ON mr.RoleID = r.RoleID 
                WHERE mr.MenuID = @MenuId AND mr.IsActive = 1 
                ORDER BY r.RoleName";
            
            return await connection.QueryAsync<MenuRole, Role, MenuRole>(sql, (menuRole, role) =>
            {
                menuRole.Role = role;
                return menuRole;
            }, new { MenuId = menuId }, splitOn: "RoleName");
        }

        public async Task<bool> CreateMenuRoleAsync(MenuRole menuRole)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO MenuRole (MenuID, RoleID, IsActive, CreatedTime, UpdatedTime)
                VALUES (@MenuID, @RoleID, @IsActive, @CreatedTime, @UpdatedTime)";
            
            var result = await connection.ExecuteAsync(sql, menuRole);
            return result > 0;
        }

        public async Task<bool> UpdateMenuRoleAsync(MenuRole menuRole)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE MenuRole 
                SET IsActive = @IsActive, UpdatedTime = @UpdatedTime
                WHERE MenuID = @MenuID AND RoleID = @RoleID";
            
            var result = await connection.ExecuteAsync(sql, menuRole);
            return result > 0;
        }

        public async Task<bool> DeleteMenuRoleAsync(int menuId, int roleId)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE MenuRole 
                SET IsActive = 0, UpdatedTime = @UpdatedTime
                WHERE MenuID = @MenuId AND RoleID = @RoleId";
            
            var result = await connection.ExecuteAsync(sql, new { MenuId = menuId, RoleId = roleId, UpdatedTime = DateTime.Now });
            return result > 0;
        }

        public async Task<bool> DeleteMenuRolesByRoleIdAsync(int roleId)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE MenuRole 
                SET IsActive = 0, UpdatedTime = @UpdatedTime
                WHERE RoleID = @RoleId";
            
            var result = await connection.ExecuteAsync(sql, new { RoleId = roleId, UpdatedTime = DateTime.Now });
            return result > 0;
        }

        public async Task<bool> SetRoleMenusAsync(int roleId, List<int> menuIds)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            // 側錄 SQL 語法
            Console.WriteLine("=== SetRoleMenus SQL ===");
            Console.WriteLine($"RoleId: {roleId}");
            Console.WriteLine($"MenuIds: {string.Join(",", menuIds ?? new List<int>())}");
            
            // 開始事務
            using var transaction = connection.BeginTransaction();
            
            try
            {
                // 先刪除現有的選單權限
                const string deleteSql = @"
                    UPDATE MenuRole 
                    SET IsActive = 0, UpdatedTime = @UpdatedTime
                    WHERE RoleID = @RoleId";
                
                Console.WriteLine($"Delete existing MenuRoles SQL: {deleteSql}");
                var deleteResult = await connection.ExecuteAsync(deleteSql, new { RoleId = roleId, UpdatedTime = DateTime.Now }, transaction);
                Console.WriteLine($"Delete MenuRoles result: {deleteResult}");
                
                // 插入新的選單權限
                if (menuIds.Any())
                {
                    const string insertSql = @"
                        INSERT INTO MenuRole (MenuID, RoleID, IsActive, CreatedTime, UpdatedTime)
                        VALUES (@MenuID, @RoleID, 1, @CreatedTime, @UpdatedTime)";
                    
                    Console.WriteLine($"Insert new MenuRoles SQL: {insertSql}");
                    
                    var menuRoles = menuIds.Select(menuId => new MenuRole
                    {
                        MenuID = menuId,
                        RoleID = roleId,
                        IsActive = true,
                        CreatedTime = DateTime.Now,
                        UpdatedTime = DateTime.Now
                    });
                    
                    var insertResult = await connection.ExecuteAsync(insertSql, menuRoles, transaction);
                    Console.WriteLine($"Insert MenuRoles result: {insertResult}");
                }
                else
                {
                    Console.WriteLine("No menu IDs provided, skipping menu role insertion");
                }
                
                transaction.Commit();
                Console.WriteLine("Transaction committed successfully");
                Console.WriteLine("===============================");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                transaction.Rollback();
                Console.WriteLine("Transaction rolled back");
                Console.WriteLine("===============================");
                return false;
            }
        }

        public async Task<List<int>> GetMenuIdsByRoleIdAsync(int roleId)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT MenuID 
                FROM MenuRole 
                WHERE RoleID = @RoleId AND IsActive = 1";
            
            var result = await connection.QueryAsync<int>(sql, new { RoleId = roleId });
            return result.ToList();
        }
    }
}
