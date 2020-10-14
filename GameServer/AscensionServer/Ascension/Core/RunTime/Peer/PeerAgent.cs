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
    /// 具体的peer代理类对象；可适配任意实现了INetworkPeer的对象；
    /// </summary>
    public class PeerAgent: IPeerAgent
    {
        public int SessionId { get { return Handle.SessionId; } }
        public bool Available { get { return Handle.Available; } }
        public IAscensionPeer Handle { get;set; }
        public IRoleEntity RemoteRole { get; private set; }
        public ICollection<object> DataCollection { get { return dataDict.Values; } }
        ConcurrentDictionary<Type, object> dataDict;
        public PeerAgent()
        {
            dataDict = new ConcurrentDictionary<Type, object>();
        }
        public PeerAgent(IAscensionPeer handle) : this()
        {
            this.Handle = handle;
        }
        public void OnInit(IAscensionPeer handle)
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
        public void SendEvent(byte opCode, object userData)
        {
            Handle.SendEventMsg(opCode, userData);
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
        public static PeerAgent Create(IAscensionPeer handle, params object[] netVars)
        {
            if (handle == null)
                throw new ArgumentNullException("Peer is invalid");
            PeerAgent pe = GameManager.ReferencePoolManager.Spawn<PeerAgent>();
            pe.OnInit(handle);
            var length = netVars.Length;
            for (int i = 0; i < length; i++)
            {
                pe.TryAdd(netVars.GetType(), netVars[i]);
            }
            return pe;
        }
        public static PeerAgent Create(IAscensionPeer handle, List<object> netVars)
        {
            if (handle == null)
                throw new ArgumentNullException("Peer is invalid");
            PeerAgent pe = GameManager.ReferencePoolManager.Spawn<PeerAgent>();
            pe.OnInit(handle);
            var length = netVars.Count;
            for (int i = 0; i < length; i++)
            {
                pe.TryAdd(netVars.GetType(), netVars[i]);
            }
            return pe;
        }
    }
}
