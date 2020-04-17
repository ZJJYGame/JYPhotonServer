/*
*Author   Don
*Since 	2020-04-17
*Description 服务器消息处理基类
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
    public abstract class Handler:IHandler
    {
        protected OperationCode opCode;
        public OperationCode OpCode { get { return opCode; } set { opCode = value; } }
        /// <summary>
        /// 操作请求
        /// </summary>
        public abstract void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, ClientPeer peer);
    }
}
