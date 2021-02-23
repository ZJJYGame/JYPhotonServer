﻿/*
*Author : YingDuan_YU
*Since 	:2020-06-012
*Description  :     同步角色副职业等级及配方之炼丹；
*                          
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
    public class SyncAlchemyHandler:Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncAlchemy; } }
        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncAlchemySubHandler>();
        }
    }
}


