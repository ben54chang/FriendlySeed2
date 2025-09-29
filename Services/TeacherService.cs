using FriendlySeed.Models;
using FriendlySeed.Data;
using Dapper;

namespace FriendlySeed.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TeacherService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT ID, Email, Name, BirthDate, Gender, Organization, Position, City, District, 
                       Password, CreatedTime, UpdatedTime, IsActive
                FROM Teacher 
                ORDER BY CreatedTime DESC";
            return await connection.QueryAsync<Teacher>(sql);
        }

        public async Task<Teacher?> GetTeacherByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT ID, Email, Name, BirthDate, Gender, Organization, Position, City, District, 
                       Password, CreatedTime, UpdatedTime, IsActive
                FROM Teacher 
                WHERE ID = @Id";
            return await connection.QueryFirstOrDefaultAsync<Teacher>(sql, new { Id = id });
        }

        public async Task<bool> CreateTeacherAsync(Teacher teacher)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = @"
                    INSERT INTO Teacher (Email, Name, BirthDate, Gender, Organization, Position, City, District, 
                                       Password, CreatedTime, UpdatedTime, IsActive)
                    VALUES (@Email, @Name, @BirthDate, @Gender, @Organization, @Position, @City, @District, 
                            @Password, @CreatedTime, @UpdatedTime, @IsActive)";
                
                var parameters = new
                {
                    teacher.Email,
                    teacher.Name,
                    teacher.BirthDate,
                    teacher.Gender,
                    teacher.Organization,
                    teacher.Position,
                    teacher.City,
                    teacher.District,
                    teacher.Password,
                    teacher.CreatedTime,
                    teacher.UpdatedTime,
                    teacher.IsActive
                };
                
                Console.WriteLine($"Creating teacher with IsActive: {teacher.IsActive}");
                
                var result = await connection.ExecuteAsync(sql, parameters);
                
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating teacher: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateTeacherAsync(Teacher teacher)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = @"
                    UPDATE Teacher SET 
                        Email = @Email,
                        Name = @Name,
                        BirthDate = @BirthDate,
                        Gender = @Gender,
                        Organization = @Organization,
                        Position = @Position,
                        City = @City,
                        District = @District,
                        Password = @Password,
                        UpdatedTime = @UpdatedTime,
                        IsActive = @IsActive
                    WHERE ID = @ID";
                
                var result = await connection.ExecuteAsync(sql, new
                {
                    teacher.ID,
                    teacher.Email,
                    teacher.Name,
                    teacher.BirthDate,
                    teacher.Gender,
                    teacher.Organization,
                    teacher.Position,
                    teacher.City,
                    teacher.District,
                    teacher.Password,
                    UpdatedTime = DateTime.Now,
                    teacher.IsActive
                });
                
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating teacher: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteTeacherAsync(int id)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = "DELETE FROM Teacher WHERE ID = @Id";
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting teacher: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ToggleTeacherStatusAsync(int id)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = @"
                    UPDATE Teacher 
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
                Console.WriteLine($"Error toggling teacher status: {ex.Message}");
                return false;
            }
        }
    }
}