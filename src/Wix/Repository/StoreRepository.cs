using Dapper;
using System.Data;
using MySql.Data.MySqlClient;
using Wix.Models;
using Microsoft.Extensions.Configuration;

namespace Wix.Repository
{
    public interface IStoreRepository
    {
        IEnumerable<StoreModel> GetAll();
        StoreModel? GetById(string id);
        void Add(StoreModel store);
        void Update(StoreModel store);
        bool Delete(string id);
    }

    public class StoreRepository : IStoreRepository
    {
        private readonly string _connectionString;

        public StoreRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection Connection => new MySqlConnection(_connectionString);

        public IEnumerable<StoreModel> GetAll()
        {
            using (var db = Connection)
            {
                return db.Query<StoreModel>("SELECT * FROM Stores").ToList();
            }
        }

        public StoreModel? GetById(string id)
        {
            using (var db = Connection)
            {
                return db.QuerySingleOrDefault<StoreModel>("SELECT * FROM Stores WHERE Id = @Id", new { Id = id });
            }
        }

        public void Add(StoreModel store)
        {
            using (var db = Connection)
            {
                string sql = "INSERT INTO Stores (Id, Title, Content, Views, TimeStamp) VALUES (@Id, @Title, @Content, @Views, @TimeStamp)";
                db.Execute(sql, store);
            }
        }

        public void Update(StoreModel store)
        {
            using (var db = Connection)
            {
                string sql = "UPDATE Stores SET Title = @Title, Content = @Content, Views = @Views, TimeStamp = @TimeStamp WHERE Id = @Id";
                db.Execute(sql, store);
            }
        }

        public bool Delete(string id)
        {
            using (var db = Connection)
            {
                string sql = "DELETE FROM Stores WHERE Id = @Id";
                int rowsAffected = db.Execute(sql, new { Id = id });
                return rowsAffected > 0;
            }
        }
    }
}
