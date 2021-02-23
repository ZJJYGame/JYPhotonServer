/*
*Author : YingDuan_YU
*Since 	:2020-06-012
*Description  :     同步宠物数据，主要用于登录后，同步宠物数据；
*                          例如添加宠物，或者选择宠物进入战斗
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
   public  class SyncPetHandler:Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncPet; } }
        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncPetSubHandler>();
        }
    }
}


