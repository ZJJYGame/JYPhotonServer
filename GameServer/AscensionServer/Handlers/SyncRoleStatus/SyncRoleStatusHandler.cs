/*
*Author : Don
*Since 	:2020-04-18
*Description  : 请求角色数值处理者，返回角色的status
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
namespace AscensionServer
{
    public class SyncRoleStatusHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncRoleStatus;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncRoleStatusSubHandler>();
        }
    }
}
