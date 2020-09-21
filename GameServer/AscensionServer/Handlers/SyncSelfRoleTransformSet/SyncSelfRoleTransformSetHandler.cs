/*
*Author : ZHD
*Since 	:2020-04-18
*Description  : 同步滋生在客户端的位置集合到服务器
*/
using System.Collections.Generic;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using Cosmos;
using System;
namespace AscensionServer
{
    public class SyncSelfRoleTransformSetHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncSelfRoleTransformQueue;
            base.OnInitialization();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var JsonResult = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleTransformQueue));
            //peer.PeerCache.RoleTransformQueue = Utility.Json.ToObject<RoleTransformQueueDTO>(JsonResult);
            //peer.PeerCache.RoleTransformQueue.RoleID = peer.PeerCache.RoleID;
            //peer.IsSendedTransform = false;
            ResponseData.Clear();
            OpResponse.OperationCode = operationRequest.OperationCode;
            OpResponse.ReturnCode = (short)ReturnCode.Success;
            peer.SendOperationResponse(OpResponse, sendParameters);
        }
    }
}
