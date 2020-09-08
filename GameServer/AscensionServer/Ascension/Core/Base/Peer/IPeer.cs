using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    /// <summary>
    /// What is session and conversation?
    /// Just as a session is a logical connection between the LUs, a conversation is a 
    /// logical connection between two transaction programs. ... LU 6.2 treats a session as a 
    /// reusable connection between two LUs. One session can support only one 
    /// conversation at a time, but one session can support many conversations in 
    /// sequence.
    /// </summary>
    public interface IPeer: IReference
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        long SessionId { get; }
        /// <summary>
        /// 是否存活；
        /// </summary>
        bool Available { get; }
        /// <summary>
        /// peer对象Handle
        /// </summary>
        object Handle { get; }
        /// <summary>
        /// 发送消息到remotePeer
        /// </summary>
        /// <param name="opCode">操作码</param>
        /// <param name="userData">用户自定义的数据字典</param>
        void SendEventMessage(byte opCode,object userData);
    }
}
