using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Paddings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffie_Hellman_Protocol
{
    internal class DBManager
    {
        public static int GetUserId(string UserName, MySqlConnection connection)
        {
            string sql = "SELECT idUser FROM mydb.users WHERE login = @UserName";
            MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserName", UserName);

            //Что возвращается по умолчанию, если запись не найдена
            if (command.ExecuteScalar() != null)
                return Convert.ToInt32(command.ExecuteScalar());
            else
                return -1;
        }
        public static bool IsPasswordCorrect(string login, byte[] hashedPassword, byte[] salt, MySqlConnection connection)
        {
            // Запрос для получения хэша для данного логина
            string sql = "SELECT hash FROM mydb.users WHERE login = @login";
            MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@login", login);

            // Выполнение запроса и чтение результата
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    return false; // Логин не найден
                }

                reader.Read();
                byte[] storedHash = (byte[])reader["hash"];

                // Сравнение полученного хэша с хэшем из базы данных
                return Hash.AreHashesEqual(storedHash, hashedPassword);
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
                result.Append(chatId).Append(" "); // join
            }
            // Удалить послекдний пробел, если список не пустой
            if (result.Length > 0)
            {
                result.Length--; // Уменьшаем длину строки на один символ, чтобы удалить последний пробел
                // проверить, что пробел удаляется
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
            // Проверка на sql инъекцию
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
        public static bool AddUser(string login, byte[] salt, byte[] hashedPassword, MySqlConnection connection)
        {
            string sql = "INSERT INTO mydb.users (nickname, login, hash, salt) VALUES (@Nickname, @Login, @Hash, @Salt)";
            MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Nickname", login);  // Используем логин в качестве никнейма
            command.Parameters.AddWithValue("@Login", login);
            command.Parameters.AddWithValue("@Hash", hashedPassword);
            command.Parameters.AddWithValue("@Salt", salt);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public static string GetAllUserNames(MySqlConnection connection)
        {
            List<string> userNames = new List<string>();

            string query = "SELECT login FROM users";

            MySqlCommand command = new MySqlCommand(query, connection);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string userName = reader.GetString(0); 
                    userNames.Add(userName);
                }
            }

            return string.Join(" ", userNames);
        }

        public static bool AddMessage(string chatID, string userID, string text, MySqlConnection connection)
        {
            string sql = "INSERT INTO mydb.messages (content, ChatParticipants_Chats_idChat, ChatParticipants_Users_idUser) " +
                         "VALUES (@Content, @ChatID, @UserID)";

            MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Content", text);
            command.Parameters.AddWithValue("@ChatID", chatID);
            command.Parameters.AddWithValue("@UserID", userID);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }
        public static List<Tuple<string, string>> GetMessagesWithSenders(int curChatId, MySqlConnection connection)
        {
            List<Tuple<string, string>> messages = new List<Tuple<string, string>>();

            string sql = @"SELECT m.content, u.nickname
                   FROM mydb.messages m 
                   JOIN mydb.users u ON m.ChatParticipants_Users_idUser = u.idUser
                   WHERE m.ChatParticipants_Chats_idChat = @ChatId ORDER BY id";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@ChatId", curChatId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string content = reader.GetString("content");
                        string senderNickname = reader.GetString("nickname");

                        messages.Add(Tuple.Create(senderNickname, content));
                    }
                }
            }

            return messages;
        }
        public static byte[] GetUserSalt(string userName, MySqlConnection connection)
        {
            string sql = "SELECT salt FROM mydb.users WHERE login = @UserName";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@UserName", userName);

                // Execute the query and retrieve the salt
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    return (byte[])result;
                }
                else
                {
                    return null; // User does not exist
                }
            }
        }
        public static bool IsUserNameExist(string UserName, MySqlConnection connection)
        {
            string sql = "SELECT COUNT(*) FROM mydb.users WHERE login = @UserName";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@UserName", UserName);
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }
        public static bool AddChat(string user1Name, string user2Name, MySqlConnection connection)
        {
            // Генерируем название чата
            string chatName = $"{user1Name}_{user2Name}";

            // SQL-запрос для вставки записи в таблицу chats
            string insertChatQuery = "INSERT INTO mydb.chats (name) VALUES (@ChatName);";

            // SQL-запрос для вставки записей в таблицу ChatParticipants
            string insertChatParticipantsQuery = @"
                INSERT INTO mydb.ChatParticipants (Chats_idChat, Users_idUser)
                SELECT @ChatId, idUser FROM mydb.users WHERE login IN (@User1Name, @User2Name);";

            // Запускаем транзакцию
            using (MySqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    // Вставляем запись в таблицу chats и получаем idChat
                    MySqlCommand command = new MySqlCommand(insertChatQuery, connection, transaction);
                    command.Parameters.AddWithValue("@ChatName", chatName);
                    command.ExecuteNonQuery();
                    long chatId = command.LastInsertedId;

                    // Вставляем записи в таблицу ChatParticipants
                    command = new MySqlCommand(insertChatParticipantsQuery, connection, transaction);
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    command.Parameters.AddWithValue("@User1Name", user1Name);
                    command.Parameters.AddWithValue("@User2Name", user2Name);
                    command.ExecuteNonQuery();

                    // Фиксируем транзакцию
                    transaction.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    // Обработка ошибок
                    Console.WriteLine($"Ошибка при создании чата: {ex.Message}");
                    transaction.Rollback();
                    return false;
                }
            }
        }
        public static string GetLoginByUserId(int userId, MySqlConnection connection)
        {
            string login = "";

            string sql = "SELECT login FROM mydb.users WHERE idUser = @UserId";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);

                // Выполняем запрос и получаем результат
                object result = command.ExecuteScalar();

                // Если результат не равен null, приводим его к строке и сохраняем в переменной login
                if (result != null)
                {
                    login = result.ToString();
                }
            }

            return login;
        }

    }
}
