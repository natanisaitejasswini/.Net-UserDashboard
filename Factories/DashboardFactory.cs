using System.Collections.Generic;
using System;
using System.Linq;
using Dapper;
using System.Data;
using MySql.Data.MySqlClient;
using aspuserdashboard.Models;
using CryptoHelper;

namespace DashboardApp.Factory
{
    public class DashboardRepository : IFactory<User>
    {
        private string connectionString;
        public DashboardRepository()
        {
            connectionString = "server=35.162.227.206;userid=remote;password=password;port=3306;database=aspwall;SslMode=None";
        }

        internal IDbConnection Connection
        {
            get {
                return new MySqlConnection(connectionString);
            }
        }
    
        public void Add_Admin(User item)
        {
            using (IDbConnection dbConnection = Connection) {
                
                string password_Hash = Crypto.HashPassword(item.password);
                string query = $"INSERT INTO users (first_name,last_name, email, password, auth_level, created_at, updated_at) VALUES ('{item.first_name}', '{item.last_name}', '{item.email}', '{password_Hash}', 0, NOW(), NOW())";
                dbConnection.Open();
                dbConnection.Execute(query);
            }
        }

        public void Add(User item)
        {
            using (IDbConnection dbConnection = Connection) {
                
                string password_Hash = Crypto.HashPassword(item.password);
                string query = $"INSERT INTO users (first_name,last_name, email, password, auth_level, created_at, updated_at) VALUES ('{item.first_name}', '{item.last_name}', '{item.email}', '{password_Hash}', 1, NOW(), NOW())";
                dbConnection.Open();
                dbConnection.Execute(query);
            }
        }
        public User FindByID()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("SELECT * FROM users ORDER BY id DESC LIMIT 1").FirstOrDefault();
            }
        }
        public User FindEmail(string email)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("SELECT * FROM users WHERE email = @Email LIMIT 1", new { Email = email }).FirstOrDefault();
            }
        }

        public IEnumerable<User> AllUsers()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("SELECT * FROM users");
            }
        }
        public IEnumerable<User> CountUsers_all()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("SELECT COUNT(*) FROM users");
            }
        }
        public User CurrentUser(int num)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>($"SELECT * FROM users WHERE id = {num}").FirstOrDefault();
            }
        }
        public User Edit(int id, string email, string first_name, string last_name, string image, int auth_level)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>($"UPDATE users SET email = '{email}', first_name = '{first_name}', last_name = '{last_name}', image = '{image}', auth_level = '{auth_level}' WHERE id = '{id}'").FirstOrDefault();
            }
        }
        
        public User Edit_Password(int id, string password)
        {
             using (IDbConnection dbConnection = Connection)
            {
                string password_Hash = Crypto.HashPassword(password);
                dbConnection.Open();
                return dbConnection.Query<User>($"UPDATE users SET password = '{password_Hash}'WHERE id = '{id}'").FirstOrDefault();
            }
        }
        public User Edit_Description(int id, string description)
        {
             using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>($"UPDATE users SET description = '{description}' WHERE id = '{id}'").FirstOrDefault();
            }
        }
         public User FindEditProfile(string num)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>($"SELECT * FROM users WHERE id = {num}").FirstOrDefault();
            }
        }
         public User FindDeleteProfile(string num)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>($"DELETE FROM users WHERE id = {num}").FirstOrDefault();
            }
        }
    }
}