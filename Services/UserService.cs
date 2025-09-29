using Dapper;
using FriendlySeed.Data;
using FriendlySeed.Models;
using BCrypt.Net;

namespace FriendlySeed.Services
{
    public class UserService : IUserService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT u.*
                FROM [User] u 
                WHERE u.IsActive = 1 
                ORDER BY u.CreatedTime DESC";
            
            var users = await connection.QueryAsync<User>(sql);
            
            // 為每個使用者添加角色名稱
            foreach (var user in users)
            {
                if (!string.IsNullOrEmpty(user.RoleID))
                {
                    user.RoleNames = await GetRoleNamesByIdsAsync(user.RoleID);
                }
            }
            
            return users;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT u.*
                FROM [User] u 
                WHERE u.ID = @Id AND u.IsActive = 1";
            
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT u.*
                FROM [User] u 
                WHERE u.Username = @Username AND u.IsActive = 1";
            
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT u.*
                FROM [User] u 
                WHERE u.Email = @Email AND u.IsActive = 1";
            
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                const string sql = @"
                    INSERT INTO [User] (Username, DisplayName, Avatar, Description, RoleID, Email, Password, IsActive)
                    VALUES (@Username, @DisplayName, @Avatar, @Description, @RoleID, @Email, @Password, @IsActive)";
                
                // 加密密碼（密碼為必填，不能為空）
                if (string.IsNullOrEmpty(user.Password))
                {
                    Console.WriteLine("Error: Password is required but empty");
                    throw new ArgumentException("密碼為必填");
                }
                
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                Console.WriteLine($"Password hashed successfully for user: {user.Username}");
                
                Console.WriteLine($"Creating user: Username={user.Username}, RoleID={user.RoleID}, Email={user.Email}, IsActive={user.IsActive}");
                
                // 準備參數
                var parameters = new
                {
                    Username = user.Username,
                    DisplayName = user.DisplayName,
                    Avatar = user.Avatar,
                    Description = user.Description,
                    RoleID = user.RoleID,
                    Email = user.Email,
                    Password = user.Password,
                    IsActive = user.IsActive
                };
                
                // 輸出實際的 SQL 和參數
                Console.WriteLine("=== SQL 除錯資訊 ===");
                Console.WriteLine($"SQL: {sql}");
                Console.WriteLine("參數:");
                Console.WriteLine($"  Username: '{parameters.Username}'");
                Console.WriteLine($"  DisplayName: '{parameters.DisplayName}'");
                Console.WriteLine($"  Avatar: '{parameters.Avatar}'");
                Console.WriteLine($"  Description: '{parameters.Description}'");
                Console.WriteLine($"  RoleID: '{parameters.RoleID}'");
                Console.WriteLine($"  Email: '{parameters.Email}'");
                Console.WriteLine($"  Password: '{parameters.Password.Substring(0, Math.Min(20, parameters.Password.Length))}...' (已截斷)");
                Console.WriteLine($"  IsActive: {parameters.IsActive}");
                Console.WriteLine("==================");
                
                var result = await connection.ExecuteAsync(sql, parameters);
                
                Console.WriteLine($"User creation result: {result}");
                
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            // 如果密碼為空，則不更新密碼欄位
            if (string.IsNullOrEmpty(user.Password))
            {
                const string sql = @"
                    UPDATE [User] 
                    SET Username = @Username, DisplayName = @DisplayName, Avatar = @Avatar, 
                        Description = @Description, RoleID = @RoleID, Email = @Email, 
                        IsActive = @IsActive, UpdatedTime = @UpdatedTime
                    WHERE ID = @ID";
                
                var parameters = new
                {
                    ID = user.ID,
                    Username = user.Username,
                    DisplayName = user.DisplayName,
                    Avatar = user.Avatar,
                    Description = user.Description,
                    RoleID = user.RoleID,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    UpdatedTime = DateTime.Now
                };
                
                // 輸出實際的 SQL 和參數
                Console.WriteLine("=== UPDATE SQL 除錯資訊 (無密碼) ===");
                Console.WriteLine($"SQL: {sql}");
                Console.WriteLine("參數:");
                Console.WriteLine($"  ID: {parameters.ID}");
                Console.WriteLine($"  Username: '{parameters.Username}'");
                Console.WriteLine($"  DisplayName: '{parameters.DisplayName}'");
                Console.WriteLine($"  Avatar: '{parameters.Avatar}'");
                Console.WriteLine($"  Description: '{parameters.Description}'");
                Console.WriteLine($"  RoleID: '{parameters.RoleID}'");
                Console.WriteLine($"  Email: '{parameters.Email}'");
                Console.WriteLine($"  IsActive: {parameters.IsActive}");
                Console.WriteLine($"  UpdatedTime: {parameters.UpdatedTime}");
                Console.WriteLine("=====================================");
                
                var result = await connection.ExecuteAsync(sql, parameters);
                Console.WriteLine($"User update result (no password): {result}");
                return result > 0;
            }
            else
            {
                const string sql = @"
                    UPDATE [User] 
                    SET Username = @Username, DisplayName = @DisplayName, Avatar = @Avatar, 
                        Description = @Description, RoleID = @RoleID, Email = @Email, 
                        Password = @Password, IsActive = @IsActive, UpdatedTime = @UpdatedTime
                    WHERE ID = @ID";
                
                // 如果密碼有變更，則加密
                if (!user.Password.StartsWith("$2"))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                }
                
                var parameters = new
                {
                    ID = user.ID,
                    Username = user.Username,
                    DisplayName = user.DisplayName,
                    Avatar = user.Avatar,
                    Description = user.Description,
                    RoleID = user.RoleID,
                    Email = user.Email,
                    Password = user.Password,
                    IsActive = user.IsActive,
                    UpdatedTime = DateTime.Now
                };
                
                // 輸出實際的 SQL 和參數
                Console.WriteLine("=== UPDATE SQL 除錯資訊 (有密碼) ===");
                Console.WriteLine($"SQL: {sql}");
                Console.WriteLine("參數:");
                Console.WriteLine($"  ID: {parameters.ID}");
                Console.WriteLine($"  Username: '{parameters.Username}'");
                Console.WriteLine($"  DisplayName: '{parameters.DisplayName}'");
                Console.WriteLine($"  Avatar: '{parameters.Avatar}'");
                Console.WriteLine($"  Description: '{parameters.Description}'");
                Console.WriteLine($"  RoleID: '{parameters.RoleID}'");
                Console.WriteLine($"  Email: '{parameters.Email}'");
                Console.WriteLine($"  Password: '{parameters.Password.Substring(0, Math.Min(20, parameters.Password.Length))}...' (已截斷)");
                Console.WriteLine($"  IsActive: {parameters.IsActive}");
                Console.WriteLine($"  UpdatedTime: {parameters.UpdatedTime}");
                Console.WriteLine("====================================");
                
                var result = await connection.ExecuteAsync(sql, parameters);
                Console.WriteLine($"User update result (with password): {result}");
                return result > 0;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "UPDATE [User] SET IsActive = 0, UpdatedTime = @UpdatedTime WHERE ID = @Id";
            
            var result = await connection.ExecuteAsync(sql, new { Id = id, UpdatedTime = DateTime.Now });
            return result > 0;
        }

        public async Task<bool> ToggleUserStatusAsync(int id)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                const string sql = @"
                    UPDATE [User] 
                    SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END,
                        UpdatedTime = @UpdatedTime
                    WHERE ID = @Id";
                
                var result = await connection.ExecuteAsync(sql, new 
                { 
                    Id = id, 
                    UpdatedTime = DateTime.Now 
                });
                
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error toggling user status: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(int id, string newPassword)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                const string sql = @"
                    UPDATE [User] 
                    SET Password = @Password, UpdatedTime = @UpdatedTime
                    WHERE ID = @Id";
                
                // 加密新密碼
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                
                var result = await connection.ExecuteAsync(sql, new 
                { 
                    Id = id, 
                    Password = hashedPassword,
                    UpdatedTime = DateTime.Now 
                });
                
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing password: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null) return false;
            
            return BCrypt.Net.BCrypt.Verify(password, user.Password);
        }

        private async Task<string> GetRoleNamesByIdsAsync(string roleIds)
        {
            if (string.IsNullOrEmpty(roleIds))
                return string.Empty;

            using var connection = _connectionFactory.CreateConnection();
            
            // 將逗號分隔的角色ID轉換為整數陣列
            var roleIdArray = roleIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(id => int.Parse(id.Trim()))
                                   .ToArray();
            
            if (roleIdArray.Length == 0)
                return string.Empty;

            // 建立 IN 查詢的參數
            var parameters = new DynamicParameters();
            var inClause = string.Join(",", roleIdArray.Select((id, index) => $"@roleId{index}"));
            
            for (int i = 0; i < roleIdArray.Length; i++)
            {
                parameters.Add($"roleId{i}", roleIdArray[i]);
            }

            var sql = $@"
                SELECT RoleName 
                FROM [Role] 
                WHERE RoleID IN ({inClause})
                ORDER BY RoleID";

            var roleNames = await connection.QueryAsync<string>(sql, parameters);
            return string.Join(", ", roleNames);
        }
    }
}
