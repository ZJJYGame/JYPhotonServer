/*
 * Author : xianrenZhang
 * Since : 2020-07-02
 * Description ：同步移动状态
 * */
using AscensionProtocol;
using Photon.SocketServer;
using System;
using Cosmos;
using AscensionProtocol.DTO;
using AscensionServer.Threads;
using System.Collections.Generic;

namespace AscensionServer
{
    public class SyncMoveStatusHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncMoveStatus; } }
        HashSet<RoleMoveStatusDTO> roleSet = new HashSet<RoleMoveStatusDTO>();
        //获取历练技能cd请求的处理的代码
        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            ResponseData.Clear();
            var roleMoveStatusJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleMoveStatus));
            //peer.PeerCache.RoleMoveStatus = Utility.Json.ToObject<RoleMoveStatusDTO>(roleMoveStatusJson);
            
            roleSet.Clear();
            //var peerSet =  AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
            //int peerSetLength = peerSet.Count;
            //for (int i = 0; i < peerSetLength; i++)
            //{
            //    roleSet.Add(peerSet[i].PeerCache.RoleMoveStatus);
            //}
            var roleSetJson = Utility.Json.ToJson(roleSet);

            ResponseData.Add((byte)ParameterCode.RoleMoveStatus, roleSetJson);
            OpResponseData.OperationCode = operationRequest.OperationCode;
            OpResponseData.ReturnCode = (short)ReturnCode.Success;
            OpResponseData.Parameters = ResponseData;
            //广播事件
            threadEventParameter.Clear();
            threadEventParameter.Add((byte)ParameterCode.RoleMoveStatus, roleMoveStatusJson);
            //QueueThreadEvent(peerSet, EventCode.SyncRoleMoveStatus, threadEventParameter);
            return OpResponseData;
        }
    }
}
