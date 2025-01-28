using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleServer1
{
    class Program
    {
        private static List<TcpClient> clients = new List<TcpClient>();

        static void Main()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            Console.WriteLine("Сервер запущен...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                clients.Add(client);
                Console.WriteLine("Клиент подключен.");
                Thread thread = new Thread(HandleClient);
                thread.Start(client);
            }
        }

        private static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            try
            {
                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Получено сообщение: {message}");
                    BroadcastMessage(message, client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            finally
            {
                clients.Remove(client);
                client.Close();
                Console.WriteLine("Клиент отключен.");
            }
        }

        private static void BroadcastMessage(string message, TcpClient sender)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            foreach (var client in clients)
            {
                if (client != sender)
                {
                    try
                    {
                        NetworkStream stream = client.GetStream();
                        stream.Write(data, 0, data.Length);
                    }
                    catch
                    {
                        
                    }
                }
            }
        }
    }
}
