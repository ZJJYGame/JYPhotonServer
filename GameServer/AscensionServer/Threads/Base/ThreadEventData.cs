using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Photon.SocketServer;

namespace AscensionServer.Threads
{
    /// <summary>
    /// 通用线程事件数据
    /// </summary>
    public class ThreadEventData : IThreadEventData
    {
        /// <summary>
        /// 客户端集合
        /// </summary>
        public ICollection<AscensionPeer> PeerCollection { get; private set; }
        /// <summary>
        /// 事件码
        /// </summary>
        public byte EventCode { get; private set; }
        /// <summary>
        /// 序列化后的字典数据
        /// </summary>
        public Dictionary<byte, object> Parameters { get;  set; }
        public ThreadEventData()
        {
            PeerCollection = null;
            EventCode = 0;
            Parameters = new Dictionary<byte, object>();
        }
        public ThreadEventData(ICollection<AscensionPeer> peerCollection, byte eventCode)
        {
            this.PeerCollection = peerCollection;
            this.EventCode = eventCode;
            Parameters = new Dictionary<byte, object>();
        }
        /// <summary>
        /// 设置对象的初始值
        /// </summary>
        /// <param name="peerCollection">peer的集合</param>
        /// <param name="eventCode">事件码</param>
        public void SetValue(ICollection<AscensionPeer> peerCollection, byte eventCode)
        {
            this.PeerCollection = peerCollection;
            this.EventCode = eventCode;
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="parameterCode">参数码</param>
        /// <param name="data">序列化后的数据</param>
        public void AddData(byte parameterCode, object data)
        {
            if (!Parameters.ContainsKey(parameterCode))
                Parameters.Add(parameterCode, data);
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="parameters"></param>
        public void SetData(Dictionary<byte, object> parameters)
        {
            this.Parameters = parameters;
        }
        /// <summary>
        /// 引用池回收时候调用的函数
        /// </summary>
        public void Clear()
        {
            PeerCollection = null;
            EventCode = 0;
            Parameters.Clear();
        }
    }
}
