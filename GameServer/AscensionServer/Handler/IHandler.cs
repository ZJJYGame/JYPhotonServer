/*
*Author   Don
*Since 	2020-04-17
*Description 消息处理接口
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;

namespace AscensionServer
{
    public interface IHandler
    {
        OperationCode OpCode { get; set; }
        /// <summary>
        /// 操作请求
        /// </summary>
        void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, ClientPeer peer);
    }
}
