using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Net.Sockets;
using Domain;

namespace ChatServer
{
    public class Server
    {
        List<Client> connectedClients;
        IFormatter formatter;
        IPAddress serverIp;
        int port;

        TcpListener server = null;

        public Server()
        {
            connectedClients = new List<Client>();
            formatter = new BinaryFormatter();
            serverIp = IPAddress.Parse("127.0.0.1");
            port = 10101;
        }

        public void Start()
        {
            try
            {
                server = new TcpListener(serverIp, port);
                server.Start();
                WaitForClients();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}: {1}", e, e.Message);
            }
            finally
            {
                server.Stop();
                Console.WriteLine("Server has stopped..");
            }

            // Stops to read any exceptions caught
            Console.ReadLine();
        }

        private void WaitForClients()
        {
            Console.WriteLine("Waiting for clients..");
            TcpClient c;
            while (true)
            {
                c = server.AcceptTcpClient();
                new Thread(() => HandleClient(c)).Start();
            }
        }

        private void HandleClient(TcpClient c)
        {
            User user = new User("Unknown");
            Client client = new Client(c);
            connectedClients.Add(client);

            user = (User)formatter.Deserialize(client.Stream);
            Console.WriteLine("User {0} is attempting to connect..", user.Name);

            if (UserIsTaken(user))
            {
                string log = string.Format("The name {0} is already taken.", user.Name);
                SendToClient(client, log);
            }
            else
            {
                client.User = user;
                BroadcastToAll(string.Format("{0} has connected.", user.Name));

                while (client.Stream.CanRead)
                {
                    HandleClientRequests(client);
                }
            }
        }

        private void HandleClientRequests(Client client)
        {
            try
            {
                Message message = new Message();
                message = (Message)formatter.Deserialize(client.Stream);

                if (IsCommand(message))
                    HandleCommand(client, message);
                else
                    BroadcastToAll(message);
            }
            catch (System.IO.IOException)
            {
                EndClient(client);
            }

        }

        private void BroadcastToAll(Message message)
        {
            Console.WriteLine(message.ToString());

            for (int i = connectedClients.Count - 1; i >= 0; i--)
            {
                SendToClient(connectedClients[i], message);
            }
        }

        private void BroadcastToAll(string message)
        {
            Message m = new Message(message);
            BroadcastToAll(m);
        }

        private void HandleCommand(Client client, Message message)
        {
            try
            {
                string[] fullCommand = message.Content.Split(' ');
                string commandName = fullCommand[0];

                if (commandName == "/w" || commandName == "/whisper")
                {
                    Message whisper = new Message();
                    whisper.User = message.User;
                    whisper.Content = string.Format("(Whisper) {0}", fullCommand[2]);
                    string toUser = fullCommand[1];
                    Client toClient = FindUser(toUser);

                    if (toClient != null)
                    {
                        SendToClientAndLog(client, whisper);
                        SendToClientAndLog(toClient, whisper);
                    }
                    else
                    {
                        SendToClientAndLog(client, string.Format("Could not find user {0}", toUser));
                    }
                }
            }
            catch
            {
                Console.WriteLine(message.ToString());
                SendToClient(client, string.Format("'{0}' is an invalid command.", message.Content));
            }
        }

        private Client FindUser(string name)
        {
            for (int i = connectedClients.Count - 1; i >= 0; i--)
            {
                if (connectedClients[i].User.Name.ToLower() == name.ToLower())
                    return connectedClients[i];
            }

            return null;
        }

        private bool IsCommand(Message message)
        {
            if (message.Content.Length <= 0)
                return false;

            return message.Content.Substring(0, 1) == "/";
        }

        private void FlushClients()
        {
            for (int i = connectedClients.Count - 1; i >= 0; i--)
            {
                if (!connectedClients[i].Stream.CanWrite)
                {
                    connectedClients.RemoveAt(i);
                }
            }
        }

        private bool UserIsTaken(User user)
        {
            for (int i = connectedClients.Count - 1; i >= 0; i--)
            {
                if (user.Equals(connectedClients[i].User))
                    return true;
            }

            return false;
        }

        private void SendToClient(Client client, Message message)
        {
            formatter.Serialize(client.Stream, message);
        }

        private void SendToClient(Client client, string message)
        {
            Message m = new Message(message);
            SendToClient(client, m);
        }

        private void SendToClientAndLog(Client client, Message message)
        {
            Console.WriteLine(message.ToString());
            SendToClient(client, message);
        }

        private void SendToClientAndLog(Client client, string message)
        {
            Console.WriteLine(message);
            SendToClient(client, message);
        }

        private void EndClient(Client client)
        {
            client.TcpClient.Close();
            FlushClients();
            Console.WriteLine("{0} has disconnected", client.User.Name);
        }
    }
}
