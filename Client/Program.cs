using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Domain;

namespace Client
{
    class Program
    {
        IFormatter formatter;
        NetworkStream stream;
        IPAddress ip;
        int port;
        string name;

        static void Main(string[] args)
        {
            Console.Title = "Client";

            Program p = new Program();
            p.Run();
        }

        private void Run()
        {
            name = SetName();

            ip = IPAddress.Parse("127.0.0.1");
            port = 10999;
            formatter = new BinaryFormatter();

            using (TcpClient client = new TcpClient())
            {
                client.Connect(ip, port);
                stream = client.GetStream();
                
                Message message = new Message();
                User user = new User(name);
                message.User = user;
                formatter.Serialize(stream, user);

                new Thread(() => DisplayChat(15)).Start();

                while (true)
                {
                    message.Content = Console.ReadLine();
                    formatter.Serialize(stream, message);
                }
            }
        }

        private string SetName()
        {
            string name = "";

            while(name == "")
            {
                Console.Clear();
                Console.Write("Choose name: ");
                name = Console.ReadLine();
            }
            Console.Clear();

            return name;
        }

        private void DisplayChat(int height)
        {
            Message message;
            SetupChat(height);
            while (true)
            {
                message = (Message)formatter.Deserialize(stream);
                WriteMessage(string.Format("{0}: {1}", message.User.Name, message.Content), height);
            }
        }

        private void SetupChat(int height)
        {
            for(int i = 0; i < height; i++)
            {
                Console.WriteLine();
            }
        }

        private void WriteMessage(string message, int height)
        {
            int topPosition = Console.CursorTop;
            int leftPosition = Console.CursorLeft;

            Console.MoveBufferArea(0, 1, Console.WindowWidth, height - 3, 0, 0);
            Console.SetCursorPosition(0, height - 3);
            Console.Write(message);
            Console.Write(new string(' ', Console.WindowWidth - message.Length));
            Console.SetCursorPosition(0, topPosition - 1);
            //Console.Write("Say: " + new string(' ', Console.WindowWidth - 5));
            //Console.SetCursorPosition(0, topPosition - 1);
            Console.SetCursorPosition(leftPosition, height + 2);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(leftPosition, height + 2);
        }

        // Old method that is preserved to use as example
        private void SendString()
        {
            IPAddress localIp = IPAddress.Parse("127.0.0.1");
            int port = 10999;

            string message;

            using (TcpClient client = new TcpClient())
            {
                client.Connect(localIp, port);
                NetworkStream stream = client.GetStream();

                message = Console.ReadLine();
                byte[] data;

                while (true)
                {
                    data = Encoding.ASCII.GetBytes(message);
                    stream.Write(data, 0, data.Length);

                    byte[] bytes = new byte[256];
                    message = null;

                    int i = stream.Read(bytes, 0, bytes.Length);

                    message = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine(message);
                }
            }
        }
    }
}