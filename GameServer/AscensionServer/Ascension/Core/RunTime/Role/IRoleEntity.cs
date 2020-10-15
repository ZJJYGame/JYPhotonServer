using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public interface IRoleEntity: IKeyValue<Type, object>, IReference
    {
        int RoleId { get; }
        int SessionId { get; }
        object[] Find(Predicate<object> handler);
        void SendMessage(object data);
        void SendEvent(byte opCode, Dictionary<byte,object> data);
    }
}
