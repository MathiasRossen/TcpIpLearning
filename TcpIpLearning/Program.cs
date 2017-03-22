using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Server";
            Program p = new Program();
            p.Run();
        }

        private void Run()
        {
            TcpListener server = null;

            try
            {
                int port = 8998;
                IPAddress localAddress = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddress, port);

                server.Start();

                string data = null;
                byte[] bytes = new byte[256];

                while (true)
                {
                    Console.WriteLine("Waiting for connection...");

                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;
                    NetworkStream stream = client.GetStream();

                    int i;

                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Recieved: {0}", data);

                        data = data.ToUpper();

                        byte[] msg = Encoding.ASCII.GetBytes(data);

                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    client.Close();
                }
            }
            catch(SocketException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.Stop();
            }

            Console.WriteLine("Server has stopped..");
            Console.ReadLine();
        }
    }
}
