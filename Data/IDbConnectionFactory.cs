using System.Data;

namespace FriendlySeed.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
