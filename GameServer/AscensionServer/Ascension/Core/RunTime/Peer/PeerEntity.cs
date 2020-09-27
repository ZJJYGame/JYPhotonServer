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
    public class PeerEntity : IKeyValue<Type, object>, IReference
    {
        public long SessionId { get { return Handle.SessionId; } }
        public bool Available { get { return Handle.Available; } }
        public IPeer Handle { get; private set; }
        public ICollection<object> DataCollection { get { return dataDict.Values; } }
        ConcurrentDictionary<Type, object> dataDict;
        public PeerEntity()
        {
            dataDict = new ConcurrentDictionary<Type, object>();
        }
        public PeerEntity(IPeer handle) : this()
        {
            this.Handle = handle;
        }
        public void OnInit(IPeer handle)
        {
            this.Handle = handle;
        }
        public bool TryGetValue(Type key, out object value)
        {
            return dataDict.TryGetValue(key, out value);
        }
        public bool ContainsKey(Type key)
        {
            return dataDict.ContainsKey(key);
        }
        public bool TryRemove(Type Key)
        {
            object netVar;
            return dataDict.TryRemove(Key, out netVar);
        }
        public bool TryRemove(Type key, out object value)
        {
            return dataDict.TryRemove(key, out value);
        }
        public bool TryAdd(Type key, object Value)
        {
            return dataDict.TryAdd(key, Value);
        }
        public bool TryUpdate(Type key, object newValue, object comparsionValue)
        {
            return dataDict.TryUpdate(key, newValue, comparsionValue);
        }
        /// <summary>
        /// 发送网络消息到peer；
        /// </summary>
        /// <param name="opCode">操作码</param>
        /// <param name="userData">用户数据</param>
        public void SendEventMessage(byte opCode, object userData)
        {
            Handle.SendEventMessage(opCode, userData);
        }
        /// <summary>
        /// 发送网络消息到peer；
        /// </summary>
        /// <param name="message">普通数据</param>
        public void SendMessage(object message)
        {
            Handle.SendMessage(message);
        }
        public void Clear()
        {
            Handle = null;
            dataDict.Clear();
        }
        public static PeerEntity Create(IPeer handle, params object[] netVars)
        {
            if (handle == null)
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
        public static PeerEntity Create(IPeer handle, List<object> netVars)
        {
            if (handle == null)
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
