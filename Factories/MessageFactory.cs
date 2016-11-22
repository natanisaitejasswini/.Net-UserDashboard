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
    public class MessageRepository : IFactory<Message>
    {
        private string connectionString;
        public MessageRepository()
        {
            connectionString = "server=127.0.0.1;userid=remote;password=password;port=3306;database=aspwall;SslMode=None";
        }

        internal IDbConnection Connection
        {
            get {
                return new MySqlConnection(connectionString);
            }
        }
    
         public void AddMessage(Message mess_item)
        {
             using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute("INSERT INTO messages (message, created_at, updated_at, user_id, tag_id) VALUES (@message, NOW(), NOW(), @user_id, @tag_id)", mess_item);
            }
        }
        public IEnumerable<Message> FindAllMessages(string mess_id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Message>($"SELECT messages.id, users.first_name, messages.user_id, messages.tag_id, messages.message, messages.created_at FROM messages LEFT JOIN users ON messages.user_id = users.id WHERE tag_id = {mess_id} ORDER BY created_at DESC");
            }
        }
        public void DelMessage(int del_item)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute($"DELETE FROM messages WHERE id = {del_item}");
            }
        }
    }
}