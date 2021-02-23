/*
 * Author : xianrenzhang
 * Since : 2020 -07-14
 * Description ：同步怪物位置集合到服务器
 */

using AscensionProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class SyncMonsterTransformSetHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncMonsterTransform; } }
        /*
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var JsonResult = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleTransformQueue));
            peer.PeerCache.RoleTransformQueue = Utility.Json.ToObject<RoleTransformQueueDTO>(JsonResult);
            peer.PeerCache.RoleTransformQueue.RoleID = peer.PeerCache.RoleID;
            peer.IsSendedTransform = false;
            ResponseData.Clear();
            OpResponse.OperationCode = operationRequest.OperationCode;
            OpResponse.ReturnCode = (short)ReturnCode.Success;
            peer.SendOperationResponse(OpResponse, sendParameters);

        }*/
    }
}


