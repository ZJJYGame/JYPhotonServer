using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using Cosmos;
namespace AscensionServer
{
    public interface IHandler:IBehaviour
    {
        byte Opcode { get; }
        object EncodeMessage(object message);
    }
}
