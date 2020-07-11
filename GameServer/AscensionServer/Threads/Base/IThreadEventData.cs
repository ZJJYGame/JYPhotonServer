using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Photon.SocketServer;

namespace AscensionServer.Threads
{
    public interface IThreadEventData:IReference
    {
        /// <summary>
        /// 客户端集合
        /// </summary>
         ICollection<AscensionPeer> PeerCollection { get; }
        /// <summary>
        /// 事件码
        /// </summary>
        byte EventCode { get; }
        /// <summary>
        /// 序列化后的字典数据
        /// </summary>
        Dictionary<byte, object> Parameters { get; }
        /// <summary>
        /// 设置对象的初始值
        /// </summary>
        /// <param name="peerCollection">peer的集合</param>
        /// <param name="eventCode">事件码</param>
        void SetValue(ICollection<AscensionPeer> peerCollection, byte eventCode);
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="parameterCode">参数码</param>
        /// <param name="data">序列化后的数据</param>
        void AddData(byte parameterCode, object data);
    }
}
