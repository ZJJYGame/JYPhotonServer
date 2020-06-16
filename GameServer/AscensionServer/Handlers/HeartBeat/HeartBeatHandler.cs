/*
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
        /// <summary>
        /// 服务器心跳检测处理者
        /// </summary>
        public override void OnInitialization()
        {
            OpCode = OperationCode.HeartBeat;
            base.OnInitialization();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            byte heartBeatResult = Convert.ToByte(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.HeartBeat));
            ResponseData.Clear();
            OpResponse.OperationCode = operationRequest.OperationCode;
            if (heartBeatResult == 1)
            {
                OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else
            {
                OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            peer.SendOperationResponse(OpResponse, sendParameters);
        }
    }
}
