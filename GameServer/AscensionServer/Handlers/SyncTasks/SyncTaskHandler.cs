/*
*Author : Yingduan_yu
*Since 	:2020-05-25
*Description  : 请求角色任务处理
*/
using System;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using System.Collections.Generic;
namespace AscensionServer
{
    public class SyncTaskHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncTask; } }
        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncTaskSubHandler>();
        }
    }
}
