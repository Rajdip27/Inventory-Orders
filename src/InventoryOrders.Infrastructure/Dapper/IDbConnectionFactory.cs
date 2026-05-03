using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace InventoryOrders.Infrastructure.Dapper;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
public class DbConnectionFactory(IConfiguration _config) : IDbConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        return new SqlConnection(
            _config.GetConnectionString("DefaultConnection")
        );
    }
}