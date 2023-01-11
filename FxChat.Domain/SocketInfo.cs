using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FxChat.Domain
{
    public class SocketInfo
    {
        public string Id => (Socket == null) ? "" : Socket.RemoteEndPoint.ToString();
        public Socket Socket { get; set; }
    }
}
