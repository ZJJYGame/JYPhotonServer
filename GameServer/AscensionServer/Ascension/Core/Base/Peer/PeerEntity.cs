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
    /// Peer实体对象，保存了peer的具体引用
    /// </summary>
    public class PeerEntity : IKeyValue<Type, NetVariable>, IReference
    {
        public long SessionId { get; private set; }
        public IPeer ClientPeer { get; private set; }
        public IRole Role { get; private set; }
        public ICollection<NetVariable> NetVariableCollection { get { return variableDict.Values; } }
        ConcurrentDictionary<Type, NetVariable> variableDict;
        public PeerEntity()
        {
            variableDict = new ConcurrentDictionary<Type, NetVariable>();
        }
        public PeerEntity(IPeer peer) : this()
        {
            this.ClientPeer = peer;
        }
        public void SetPeer(IPeer peer)
        {
            this.ClientPeer = peer;
        }
        public bool TryGetValue(Type key, out NetVariable value)
        {
            return variableDict.TryGetValue(key, out value);
        }
        public bool ContainsKey(Type key)
        {
            return variableDict.ContainsKey(key);
        }
        public bool TryRemove(Type Key)
        {
            NetVariable netVar;
            return variableDict.TryRemove(Key, out netVar);
        }
        public bool TryAdd(Type key, NetVariable Value)
        {
            return variableDict.TryAdd(key, Value);
        }
        public bool TryUpdate(Type key, NetVariable newValue, NetVariable comparsionValue)
        {
            return variableDict.TryUpdate(key, newValue, comparsionValue);
        }
        public void Clear()
        {
            ClientPeer = null;
            variableDict.Clear();
        }
        public static PeerEntity Create(IPeer peer, params NetVariable[] netVars)
        {
            if (peer == null)
                throw new ArgumentNullException("Peer is invalid");
            PeerEntity pe = GameManager.ReferencePoolManager.Spawn<PeerEntity>();
            pe.SetPeer(peer);
            ushort length = Convert.ToUInt16(netVars.Length);
            for (int i = 0; i < length; i++)
            {
                pe.TryAdd(netVars.GetType(), netVars[i]);
            }
            return pe;
        }
        public static PeerEntity Create(IPeer peer, List<NetVariable> netVars)
        {
            if (peer == null)
                throw new ArgumentNullException("Peer is invalid");
            PeerEntity pe = GameManager.ReferencePoolManager.Spawn<PeerEntity>();
            pe.SetPeer(peer);
            ushort length = Convert.ToUInt16(netVars.Count);
            for (int i = 0; i < length; i++)
            {
                pe.TryAdd(netVars.GetType(), netVars[i]);
            }
            return pe;
        }
    }
}
