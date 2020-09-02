using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;
using System.Security.Principal;

namespace AscensionServer
{
    /// <summary>
    /// peer的变量；可追加DTO；
    /// </summary>
    public class PeerEntity : IKeyValue<GenericValuePair<Type, byte>, DataTransferObject>
    {
        public IPeer ClientPeer { get; private set; }
        Dictionary<GenericValuePair<Type, byte>, Variable> variableDict;
        public PeerEntity()
        {
            variableDict = new Dictionary<GenericValuePair<Type, byte>, Variable>();
        }
        public PeerEntity(IPeer peer):this()
        {
            this.ClientPeer = peer;
        }
        public void SetPeer(IPeer peer)
        {
            this.ClientPeer = peer;
        }

        public bool TryGetValue(GenericValuePair<Type, byte> key, out DataTransferObject value)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(GenericValuePair<Type, byte> key)
        {
            throw new NotImplementedException();
        }

        public bool TryRemove(GenericValuePair<Type, byte> Key)
        {
            throw new NotImplementedException();
        }

        public bool TryAdd(GenericValuePair<Type, byte> key, DataTransferObject Value)
        {
            throw new NotImplementedException();
        }

        public bool TryUpdate(GenericValuePair<Type, byte> key, DataTransferObject newValue, DataTransferObject comparsionValue)
        {
            throw new NotImplementedException();
        }
    }
}
