﻿/*
*Author : Don
*Since 	:2020-05-25
*Description  : 心跳检测处理
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using Cosmos;
namespace AscensionServer
{
    public class HeartBeatHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.HeartBeat; } }

        /// <summary>
        /// 服务器心跳检测处理者
        /// </summary>
        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            byte heartBeatResult = Convert.ToByte(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.HeartBeat));
            responseParameters.Clear();
            opResponseData.OperationCode = operationRequest.OperationCode;
            if (heartBeatResult == 1)
            {
                opResponseData.ReturnCode = (short)ReturnCode.Success;
            }
            else
            {
                opResponseData.ReturnCode = (short)ReturnCode.Fail;
            }
            return opResponseData;
        }
    }
}
