using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using consoleAPp;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace consoleAPp
{

    internal class Server
    {
        static async Task Main(string[] args) 
        {
            string connectionString = "server=localhost;database=mydb;user=root;password=root";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.ConnectionString = connectionString;
            connection.Open();

            string sql = "SELECT * FROM mydb.messages";
            MySqlCommand command = new MySqlCommand(sql, connection);


            Console.WriteLine("Connection opened successfully.");

            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"content: {reader["content"]}");
            }

            connection.Close();
            Console.WriteLine("Connection closed.");

            const string ip = "127.0.0.1";
            const int port = 8081;

            serverClass server = new serverClass(ip, port);
            await server.StartServerAsync();
        }
    }
}
