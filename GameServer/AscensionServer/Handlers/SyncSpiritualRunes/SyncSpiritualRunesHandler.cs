﻿/*
*Author : YingDuan_YU
*Since 	:2020-07-23
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
   public class SyncSpiritualRunesHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncSpiritualRunes;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncSpiritualRuneSubHandler>();
        }
    }
}