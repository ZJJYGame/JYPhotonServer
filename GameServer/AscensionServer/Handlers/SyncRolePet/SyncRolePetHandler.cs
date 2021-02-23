/*
*Author : YingDuan_YU
*Since 	:2020-06-012
*Description  :     同步角色宠物数据，主要用于登录后，同步角色宠物数据；
*                          例如添加角色宠物，或者选择角色宠物进入战斗
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
    public class SyncRolePetHandler:Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncRolePet; } }

        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncRolePetSubHandler>();
        }
    }
}


