using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Client";

            IPAddress localIp = IPAddress.Parse("127.0.0.1");
            int port = 8998;

            using(TcpClient client = new TcpClient())
            {
                client.Connect(localIp, port);

                NetworkStream stream = client.GetStream();

                byte[] data = Encoding.ASCII.GetBytes("Message from client!");

                stream.Write(data, 0, data.Length);
            }

            Console.WriteLine("Message sent..");
            Console.ReadLine();
        }
    }
}
