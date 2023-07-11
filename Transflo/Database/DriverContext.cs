using Microsoft.Data.SqlClient;
using System.Data;

namespace Transflo.Database
{
    public class DriverContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public DriverContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DriverDatabase");
        }
        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}
