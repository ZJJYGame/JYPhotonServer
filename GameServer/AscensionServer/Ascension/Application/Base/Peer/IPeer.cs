using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    public interface IPeer:IRemotePeer
    {
        /// <summary>
        /// peer对象
        /// </summary>
        object PeerHandle { get; }
    }
}
