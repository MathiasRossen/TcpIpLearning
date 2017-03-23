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
using ClientUI.Views;
using Domain;

namespace ClientUI
{
    class ClientHandler
    {
        Chat chatDisplay;
        IPAddress ip;
        int port;
        IFormatter formatter;
        NetworkStream stream;

        public ClientHandler(Chat chatDisplay)
        {
            this.chatDisplay = chatDisplay;

            port = 10999;
            ip = IPAddress.Parse("127.0.0.1");
            formatter = new BinaryFormatter();
        }

        public void Connect(User user)
        {
            TcpClient client = new TcpClient();

            try
            {
                client.Connect(ip, port);
                stream = client.GetStream();

                formatter.Serialize(stream, user);
                new Thread(() => ChatWindowUpdater());
            }
            catch
            {
                // Empty for now
            }
        }

        public void SendMessage(Message message)
        {
            formatter.Serialize(stream, message);
        }

        private void ChatWindowUpdater()
        {
            Message message = new Message();

            while (true)
            {
                message = (Message)formatter.Deserialize(stream);
                chatDisplay.RecieveMessage(message);
            }
        }
    }
}
