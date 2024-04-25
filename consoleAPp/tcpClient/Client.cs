using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using consoleAPp;
using System.Numerics;

namespace tcpClient
{
    internal class Client
    {
        static async Task Main(string[] args)
        {
            // Создание экземпляр клиентского приложения
            socket client = new socket();
            
            // IP адрес и порт сервера
            const string serverIP = "127.0.0.1"; // IP адрес сервера
            const int serverPort = 8081; // Порт сервера
            await client.ConnectAsync(serverIP, serverPort);
            
            Console.WriteLine(":::Client:::");
            string result = "";
            Console.WriteLine("Введите логин и пароль");
            // Отправка и принятие сообщений
            while (true)
            {
                string UserAutentification = Console.ReadLine();
                if (UserAutentification != "")
                {
                    
                    await client.SendAsync(UserAutentification);
                    await client.ReceiveMessages();
                    result = client.GetText();
                }
                Console.WriteLine(result);
                if (result != "Неправильно введены учетные данные, попробуйте снова.") 
                {
                    while(true)
                    {
                        Console.WriteLine("Введите \"1\", чтобы отправить сообщение, или \"2\", чтобы закрыть соединение.");

                        string userInput = Console.ReadLine(); // Сохранение введенного значение

                        if (userInput == "1")
                        {
                            Console.WriteLine("Введите ваше сообщение в формате <Название чата, которому хотите отправить сообщение> <Текст сообщения>:");
                            await client.SendAsync(Console.ReadLine());
                            await client.ReceiveMessages();;
                        }
                        else if (userInput == "2")
                        {
                            await client.CloseAsync();
                            return;
                        }
                        await client.ReceiveMessages();
                    }   
                }
            }
        }
    }
}
