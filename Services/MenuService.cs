using Dapper;
using FriendlySeed.Models;
using System.Data;
using FriendlySeed.Data;

namespace FriendlySeed.Services
{
    public class MenuService : IMenuService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MenuService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Menu>> GetAllMenusAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT * FROM [Menu] 
                ORDER BY SortOrder, MenuName";

            return await connection.QueryAsync<Menu>(sql);
        }

        public async Task<IEnumerable<Menu>> GetActiveMenusAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT * FROM [Menu] 
                WHERE IsActive = 1
                ORDER BY SortOrder, MenuName";

            return await connection.QueryAsync<Menu>(sql);
        }

        public async Task<Menu?> GetMenuByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT * FROM [Menu] 
                WHERE MenuID = @Id";

            return await connection.QueryFirstOrDefaultAsync<Menu>(sql, new { Id = id });
        }

        public async Task<bool> CreateMenuAsync(Menu menu)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO [Menu] (MenuName, ParentID, SortOrder, IsActive, IconUrl, MenuUrl)
                VALUES (@MenuName, @ParentID, @SortOrder, @IsActive, @IconUrl, @MenuUrl)";

            var result = await connection.ExecuteAsync(sql, menu);
            return result > 0;
        }

        public async Task<bool> UpdateMenuAsync(Menu menu)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE [Menu] 
                SET MenuName = @MenuName, ParentID = @ParentID, SortOrder = @SortOrder, 
                    IsActive = @IsActive, IconUrl = @IconUrl, MenuUrl = @MenuUrl, 
                    UpdatedTime = @UpdatedTime
                WHERE MenuID = @MenuID";

            menu.UpdatedTime = DateTime.Now;
            var result = await connection.ExecuteAsync(sql, menu);
            return result > 0;
        }

        public async Task<bool> DeleteMenuAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "DELETE FROM [Menu] WHERE MenuID = @Id";

            var result = await connection.ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }

        public async Task<bool> ToggleMenuStatusAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE [Menu] 
                SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END,
                    UpdatedTime = @UpdatedTime
                WHERE MenuID = @Id";

            var result = await connection.ExecuteAsync(sql, new { Id = id, UpdatedTime = DateTime.Now });
            return result > 0;
        }

        public async Task<IEnumerable<Menu>> GetMenuTreeAsync()
        {
            var allMenus = await GetActiveMenusAsync();
            var menuList = allMenus.ToList();

            // 建立父子關係
            var menuDict = menuList.ToDictionary(m => m.MenuID);
            var rootMenus = new List<Menu>();

            foreach (var menu in menuList)
            {
                if (menu.ParentID == null)
                {
                    rootMenus.Add(menu);
                }
                else if (menuDict.ContainsKey(menu.ParentID.Value))
                {
                    menuDict[menu.ParentID.Value].Children.Add(menu);
                }
            }

            return rootMenus.OrderBy(m => m.SortOrder);
        }
    }
}