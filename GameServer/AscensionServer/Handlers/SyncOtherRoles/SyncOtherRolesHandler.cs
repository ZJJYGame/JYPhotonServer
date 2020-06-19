/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 同步其他玩家处理者
*/
using System.Collections.Generic;
using AscensionProtocol;
using Photon.SocketServer;
using Cosmos;
using System;
namespace AscensionServer
{
   public class SyncOtherRolesHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncOtherRoles;
            base.OnInitialization();
            //OnSubHandlerInitialization<SyncOtherRolesSubHandler>();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var JsonResult = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleTransfrom));
            peer.RoleTransformJson = JsonResult;
            ResponseData.Clear();
            OpResponse.OperationCode = operationRequest.OperationCode;
            OpResponse.ReturnCode = (short)ReturnCode.Success;
            peer.SendOperationResponse(OpResponse, sendParameters);
        }
    }
}
