/*
*Author : Don
*Since 	:2020-06-01
*Description  :     同步角色数据，主要用于登录后，同步角色数据；
*                          例如创建角色，或者选择角色进入主界面
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
    public class SyncRoleHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.LoginArea; } }
        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncRoleSubHandler>();
        }
    }
}


