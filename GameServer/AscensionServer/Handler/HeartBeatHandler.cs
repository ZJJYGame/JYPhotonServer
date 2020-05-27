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
namespace AscensionServer
{
    public class HeartBeatHandler : BaseHandler
    {
        /// <summary>
        /// 服务器心跳检测处理者
        /// </summary>
        public HeartBeatHandler()
        {
            OpCode = OperationCode.HeartBeat;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            byte data = Convert.ToByte(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.HeartBeat));
            OperationResponse response = new OperationResponse(operationRequest.OperationCode);
            if (data == 1)
            {
                response.OperationCode = (byte)ReturnCode.Success;
                //AscensionServer.log.Info(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>我胡汉三回来了");
            }else
            response.OperationCode = (byte)ReturnCode.Fail;
            peer.SendOperationResponse(response, sendParameters);
        }
    }
}
