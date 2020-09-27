﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;
using AscensionProtocol.DTO;
using RedisDotNet;
namespace AscensionServer
{
    public class UpdateDailyMessageSubHandler : SyncDailyMessageSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Update;

        public  override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            return operationResponse;
        }
    }
}