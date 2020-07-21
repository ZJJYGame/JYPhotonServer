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
        HashSet<RoleMoveStatusDTO> roleSet = new HashSet<RoleMoveStatusDTO>();
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncMoveStatus;
            base.OnInitialization();
        }
        //获取历练技能cd请求的处理的代码
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResponseData.Clear();
            var roleMoveStatusJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleMoveStatus));
            peer.PeerCache.RoleMoveStatus = Utility.Json.ToObject<RoleMoveStatusDTO>(roleMoveStatusJson);
            AscensionServer._Log.Info("Role:ID " + peer.PeerCache.RoleID + "\n RoleJson :" + roleMoveStatusJson);
            
            roleSet.Clear();
            var peerSet =  AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
            int peerSetLength = peerSet.Count;
            for (int i = 0; i < peerSetLength; i++)
            {
                roleSet.Add(peerSet[i].PeerCache.RoleMoveStatus);
            }
            var roleSetJson = Utility.Json.ToJson(roleSet);

            ResponseData.Add((byte)ParameterCode.RoleMoveStatus, roleSetJson);
            OpResponse.OperationCode = operationRequest.OperationCode;
            OpResponse.ReturnCode = (short)ReturnCode.Success;
            OpResponse.Parameters = ResponseData;
            peer.SendOperationResponse(OpResponse, sendParameters);
            //广播事件
            threadEventParameter.Clear();
            threadEventParameter.Add((byte)ParameterCode.RoleMoveStatus, roleMoveStatusJson);
            QueueThreadEvent(peerSet, EventCode.SyncRoleMoveStatus, threadEventParameter);
        }

    }
}
