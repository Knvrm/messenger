using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffie_Hellman_Protocol
{
    internal class DBManager
    {
        public static void SelectAll(MySqlConnection connection)
        {
            string sql = "SELECT * FROM messages";
            MySqlCommand command = new MySqlCommand(sql, connection);

            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"content: {reader["content"]}");
            }
        }

        public static bool IsChatName(string ChatName, MySqlConnection connection)
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
        public static int GetUserId(string UserName, MySqlConnection connection)
        {
            string sql = "SELECT idUser FROM mydb.users WHERE login = @UserName";
            MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserName", UserName);

            int userId = -1;

            userId = Convert.ToInt32(command.ExecuteScalar());


            return userId;
        }
        public static bool IsLoginAndPassword(string login, string passwd, MySqlConnection connection)
        {
            // hash + salt
            string sql = "SELECT login, password FROM mydb.users WHERE login = @login and password = @passwd";
            MySqlCommand command = new MySqlCommand(sql, connection);

            // добавление параметров
            command.Parameters.AddWithValue("@login", login);
            command.Parameters.AddWithValue("@passwd", passwd);

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
        public static string GetChatsByUserId(int userId, MySqlConnection connection)
        {
            StringBuilder result = new StringBuilder();

            string sql = "SELECT Chats_idChat FROM mydb.chatparticipants WHERE Users_idUser = @UserId";
            MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int chatId = reader.GetInt32("Chats_idChat");
                result.Append(chatId).Append(" ");
            }

            // Удалить последний пробел, если список не пустой
            if (result.Length > 0)
            {
                result.Length--; // Уменьшаем длину строки на один символ, чтобы удалить последний пробел
            }
            reader.Close();
            command.Dispose();
            return result.ToString();
        }

        public static string GetChatNamesByIds(string chatIdsString, MySqlConnection connection)
        {
            List<string> chatNames = new List<string>();

            // Преобразование строки чисел в список int
            List<int> chatIds = chatIdsString.Split(' ').Select(int.Parse).ToList();

            // Подготовка SQL запроса с использованием IN оператора
            StringBuilder sqlBuilder = new StringBuilder("SELECT name FROM mydb.chats WHERE idChat IN (");
            for (int i = 0; i < chatIds.Count; i++)
            {
                sqlBuilder.Append("@ChatId").Append(i);
                if (i < chatIds.Count - 1)
                {
                    sqlBuilder.Append(", ");
                }
            }
            sqlBuilder.Append(")");

            MySqlCommand command = new MySqlCommand(sqlBuilder.ToString(), connection);
            // Добавление параметров для каждого идентификатора чата
            for (int i = 0; i < chatIds.Count; i++)
            {
                command.Parameters.AddWithValue("@ChatId" + i, chatIds[i]);
            }

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string chatName = reader.GetString("name");
                    chatNames.Add(chatName);
                }
            }

            return string.Join(" ", chatNames);
        }



        public static bool AddMessage(string chatID, string userID, string text, MySqlConnection connection)
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

        public static bool IsUserName(string UserName, MySqlConnection connection)
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
