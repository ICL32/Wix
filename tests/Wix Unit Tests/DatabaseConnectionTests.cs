using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wix_Unit_Tests
{
    public class DatabaseConnectionTests
    {
        private readonly string _connectionString = "Server=islamwixserver.mysql.database.azure.com;Port=3306;Database=lithuania-wix;Uid=kleurapcdm;Pwd=iceclaw123!";

        [Fact]
        public void CanOpenConnection_ValidConnectionString_ShouldReturnTrue()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    Assert.True(connection.State == ConnectionState.Open, "Connection should be open.");
                }
                catch (Exception ex)
                {
                    Assert.True(false, $"Exception occurred: {ex.Message}");
                }
            }
        }

        [Fact]
        public void CannotOpenConnection_InvalidConnectionString_ShouldThrowException()
        {
            var invalidConnectionString = "Server=invalid;Port=3306;Uid=kleurapcdm;Pwd=iceclaw123!;Database=lithuania-wix;";

            using (var connection = new MySqlConnection(invalidConnectionString))
            {
                Assert.Throws<MySqlException>(() => connection.Open());
            }
        }

        [Fact]
        public void CanExecuteQuery_ValidQuery_ShouldNotThrowException()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM stores;";
                    var result = cmd.ExecuteScalar();

                    Assert.NotNull(result);
                }
            }
        }
    }
}
