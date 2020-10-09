﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    /// <summary>
    /// 用于适配的Peer接口，管理一个具体实现的client peer 对象
    /// </summary>
    public interface IRemotePeer: IReference
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        int SessionId { get; }
        /// <summary>
        /// 是否存活；
        /// </summary>
        bool Available { get; }
        /// <summary>
        /// peer对象Handle
        /// </summary>
        object Handle { get; }
        /// <summary>
        /// 接收二进制流数据委托；
        /// </summary>
        event Action<object> OnMessageReceive;
        /// <summary>
        /// 发送消息到remotePeer
        /// </summary>
        /// <param name="opCode">操作码</param>
        /// <param name="userData">用户自定义的数据字典</param>
        void SendEventMessage(byte opCode,object userData);
        /// <summary>
        /// 发送消息到remotePeer
        /// </summary>
        /// <param name="message">消息数据</param>
        void SendMessage(object message);
    }
}