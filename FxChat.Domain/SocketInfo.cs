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
        public Guid Id;
        public Socket Socket{ get; set; }
    }
}
