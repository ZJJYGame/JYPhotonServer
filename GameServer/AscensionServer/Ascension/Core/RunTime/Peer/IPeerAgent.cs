using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 具体的peer代理接口；
    /// </summary>
    public interface IPeerAgent: IKeyValue<Type, object>, IReference
    {
        int SessionId { get; }
        bool Available { get ; }
        IAscensionPeer Handle { get; set; }
        IRoleEntity RemoteRole { get; }
        ICollection<object> DataCollection { get; }
        void SendMessage(object message);
        void SendEvent(byte opCode, Dictionary<byte,object> userData);
    }
}
