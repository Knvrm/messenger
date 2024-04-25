using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace consoleAPp
{
    internal class Sql
    {
        public static void SqlQuerySelectAll(MySqlConnection connection)
        {
            string sql = "SELECT * FROM messages";
            MySqlCommand command = new MySqlCommand(sql, connection);

            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"content: {reader["content"]}");
            }
        }

        public static bool SqlQueryCheckChatName(string ChatName, MySqlConnection connection)
        {

            string sql = $"SELECT name FROM mydb.chats WHERE name = \"{ChatName}\"";
            MySqlCommand command = new MySqlCommand(sql, connection);

            MySqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Close();
                return true;
            }
            else
            {
                reader.Close();
                return false;
            }
        }
        public static bool SqlQueryCheckLoginAndPassword(string login, string passwd, MySqlConnection connection)
        {
            // TODO: fix sql-injenction
            // hash + salt
            string sql = $"SELECT login, password FROM mydb.users WHERE login = \"{login}\" and password = \"{passwd}\"";
            MySqlCommand command = new MySqlCommand(sql, connection);

            MySqlDataReader reader = command.ExecuteReader();
            // using auto close
            if (reader.HasRows)
            {
                reader.Close();
                return true;
            }
            else
            {
                reader.Close();
                return false;
            }
        }

        public static bool SqlQueryAddMessage(string chatID, string userID, string text, MySqlConnection connection)
        {
            string sql = $"INSERT INTO mydb.messages (content, ChatParticipants_Chats_idChat, ChatParticipants_Users_idUser) values (\"{text}\", {chatID}, {chatID})";
            MySqlCommand command = new MySqlCommand(sql, connection);

            MySqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Close();
                return true;
            }
            else
            {
                reader.Close();
                return false;
            }
        }

        public static bool SqlQueryCheckUserName(string UserName, MySqlConnection connection)
        {

            string sql = $"SELECT nickname FROM mydb.users WHERE name = \"{UserName}\"";
            MySqlCommand command = new MySqlCommand(sql, connection);

            MySqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Close();
                return true;
            }
            else
            {
                reader.Close();
                return false;
            }
        }
    }
}
