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
    public class CommentRepository : IFactory<Comment>
    {
        private string connectionString;
        public CommentRepository()
        {
            connectionString = "server=localhost;userid=root;password=root;port=8889;database=aspwall;SslMode=None";
        }

        internal IDbConnection Connection
        {
            get {
                return new MySqlConnection(connectionString);
            }
        }
    
        public void AddComment(Comment comment_item)
        {
             using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                Console.WriteLine("number in factory::::" + comment_item.user_id);
                dbConnection.Execute("INSERT INTO comments (comment, created_at, updated_at, message_id, user_id) VALUES (@comment, NOW(), NOW(), @message_id, @user_id)", comment_item);
            }
        }
        public IEnumerable<Comment> FindAllComments()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Comment>("SELECT *  FROM comments, users, messages WHERE comments.message_id = messages.id AND comments.user_id = users.id");
            }
        }
    }
}