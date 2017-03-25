using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Domain
{
    public class Client
    {
        public User User { get; set; }
        public TcpClient TcpClient { get; set; }
        public NetworkStream Stream { get; private set; }

        public Client(TcpClient c)
        {
            TcpClient = c;
            Stream = c.GetStream();
        }
    }
}
