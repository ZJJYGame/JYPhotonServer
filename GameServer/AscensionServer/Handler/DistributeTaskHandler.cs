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

namespace AscensionServer.Handler
{
    public class DistributeTaskHandler : BaseHandler
    {
        public DistributeTaskHandler()
        {
            opCode = OperationCode.DistributeTask;
        }

        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string roletask = Convert.ToString(Utility.GetValue(operationRequest.Parameters,(byte)ObjectParameterCode.Role));
            var roletaskobj = Utility.ToObject<DistributeTaskDTO>(roletask);



        }
       

    }
}
