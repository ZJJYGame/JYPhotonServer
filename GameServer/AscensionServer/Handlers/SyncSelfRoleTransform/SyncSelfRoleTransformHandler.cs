/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 同步客户端自身位置到服务器存储
*/
using System.Collections.Generic;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using Cosmos;
using System;
namespace AscensionServer
{
   public class SyncSelfRoleTransformHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncSelfRoleTransform;
            base.OnInitialization();
            //OnSubHandlerInitialization<SyncOtherRolesSubHandler>();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var JsonResult = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleTransfrom));
            peer.RoleTransformJson = JsonResult;
            peer.RoleTransform = Utility.Json.ToObject<RoleTransformDTO>(JsonResult);
            ResponseData.Clear();
            OpResponse.OperationCode = operationRequest.OperationCode;
            OpResponse.ReturnCode = (short)ReturnCode.Success;
            peer.SendOperationResponse(OpResponse, sendParameters);
        }
    }
}
