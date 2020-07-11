using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using Cosmos;
namespace AscensionServer
{
    public interface IHandler:IBehaviour
    {
        /// <summary>
        /// 操作码
        /// </summary>
        OperationCode OpCode { get;  }
        /// <summary>
        /// 响应事件
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="peer"></param>
        void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer);
    }
}
