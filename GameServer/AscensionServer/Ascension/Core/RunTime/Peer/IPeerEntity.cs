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
    public interface IPeerEntity: IReference,IKeyValue<Type,object>
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
        /// 发送消息到remotePeer
        /// </summary>
        /// <param name="opData">消息数据</param>
        void SendMessage(OperationData  opData);
        void SendMessage(byte opCode, Dictionary<byte, object> userData);
        void SendMessage(byte opCode, short subCode, Dictionary<byte, object> userData);

    }
}


