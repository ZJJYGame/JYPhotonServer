/*
*Author : Don
*Since 	:2020-04-18
*Description  : 请求角色数值处理者
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;

namespace AscensionServer.Handler
{
    public class SyncRoleStatusHandler : BaseHandler
    {
        public SyncRoleStatusHandler()
        {
            opCode = AscensionProtocol.OperationCode.SyncRoleStatus;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {

        }
    }
}
