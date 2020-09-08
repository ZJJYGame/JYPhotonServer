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
    public class PeerEntity :  IKeyValue<Type, NetVariable>, IReference
    {
        public long SessionId { get { return Handle.SessionId; } }
        public bool Available { get { return Handle.Available; } }
        public IRoleEntity RoleEntity { get; private set; }
        public IPeer Handle { get; private set; }
        public ICollection<NetVariable> DataCollection { get { return dataDict.Values; } }
        ConcurrentDictionary<Type, NetVariable> dataDict;
        public PeerEntity()
        {
            dataDict = new ConcurrentDictionary<Type, NetVariable>();
        }
        public PeerEntity(IPeer handle) : this()
        {
            this.Handle= handle;
        }
        public void OnInit(IPeer handle)
        {
            this.Handle= handle;
        }
        public bool TryGetValue(Type key, out NetVariable value)
        {
            return dataDict.TryGetValue(key, out value);
        }
        public bool ContainsKey(Type key)
        {
            return dataDict.ContainsKey(key);
        }
        public bool TryRemove(Type Key)
        {
            NetVariable netVar;
            return dataDict.TryRemove(Key, out netVar);
        }
        public bool TryAdd(Type key, NetVariable Value)
        {
            return dataDict.TryAdd(key, Value);
        }
        public bool TryUpdate(Type key, NetVariable newValue, NetVariable comparsionValue)
        {
            return dataDict.TryUpdate(key, newValue, comparsionValue);
        }
        public void SendEventMessage(byte opCode, object userData)
        {
            Handle.SendEventMessage(opCode, userData);
        }
        public void Clear()
        {
            Handle= null;
            dataDict.Clear();
        }
        public static PeerEntity Create(IPeer handle , params NetVariable[] netVars)
        {
            if (handle== null)
                throw new ArgumentNullException("Peer is invalid");
            PeerEntity pe = GameManager.ReferencePoolManager.Spawn<PeerEntity>();
            pe.OnInit(handle);
            ushort length = Convert.ToUInt16(netVars.Length);
            for (int i = 0; i < length; i++)
            {
                pe.TryAdd(netVars.GetType(), netVars[i]);
            }
            return pe;
        }
        public static PeerEntity Create(IPeer handle , List<NetVariable> netVars)
        {
            if (handle== null)
                throw new ArgumentNullException("Peer is invalid");
            PeerEntity pe = GameManager.ReferencePoolManager.Spawn<PeerEntity>();
            pe.OnInit(handle);
            ushort length = Convert.ToUInt16(netVars.Count);
            for (int i = 0; i < length; i++)
            {
                pe.TryAdd(netVars.GetType(), netVars[i]);
            }
            return pe;
        }
    }
}
