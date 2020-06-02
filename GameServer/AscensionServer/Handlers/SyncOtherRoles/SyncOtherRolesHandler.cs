/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 同步其他玩家处理者
*/
using System.Collections.Generic;
using AscensionProtocol;
using Photon.SocketServer;

namespace AscensionServer
{
   public class SyncOtherRolesHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncOtherRoles;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncOtherRolesSubHandler>();
        }
    }
}
