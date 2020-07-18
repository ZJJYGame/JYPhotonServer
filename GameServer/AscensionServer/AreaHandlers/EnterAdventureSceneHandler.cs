using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using Cosmos;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Threads;
using AscensionServer.Model;

namespace AscensionServer
{
    public class EnterAdventureSceneHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.EnterAdventureScene;
            base.OnInitialization();
        }

        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            HashSet<Role> roleSet = new HashSet<Role>();
            HashSet<RoleMoveStatusDTO> roleMoveStatusSet = new HashSet<RoleMoveStatusDTO>();
            HashSet<RoleTransformQueueDTO> roleTransformQueueSet = new HashSet<RoleTransformQueueDTO>();
            //var roleJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Role));
            var roleMoveStatusJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleMoveStatus));
            AscensionServer._Log.Info("EnterAdventureScene  :  " + peer.ToString());
            //这条，获取当前玩家未进入探索界面时候所有玩家的集合
            var peerSet = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
            var roleJson = Utility.Json.ToJson(peer.PeerCache.Role);
            AscensionServer.Instance.EnterAdventureScene(peer);

            peer.PeerCache.RoleMoveStatus = Utility.Json.ToObject<RoleMoveStatusDTO>(roleMoveStatusJson);
            int peerSetLength = peerSet.Count;
             roleSet.Clear();
            roleMoveStatusSet.Clear();
            roleTransformQueueSet.Clear();
            for (int i = 0; i < peerSetLength; i++)
            {
                roleSet.Add( peerSet[i].PeerCache.Role);
                roleMoveStatusSet.Add(peerSet[i].PeerCache.RoleMoveStatus);
                roleTransformQueueSet.Add(peerSet[i].PeerCache.RoleTransformQueue);
            }
            var roleSetJson = Utility.Json.ToJson(roleSet);
            var roleMoveStatusSetJson = Utility.Json.ToJson(roleMoveStatusSet);
            var roleTransformQueueSetJson = Utility.Json.ToJson(roleTransformQueueSet);
            var resSetDictJson = Utility.Json.ToJson(AscensionServer.Instance.ResUnitSetDict.Values.ToList());
            ResponseData.Clear();
            OpResponse.OperationCode = operationRequest.OperationCode;
            OpResponse.ReturnCode = (byte)ReturnCode.Success;
            ResponseData.Add((byte)ParameterCode.RoleSet, roleSetJson);
            ResponseData.Add((byte)ParameterCode.RoleMoveStatusSet, roleMoveStatusSetJson);
            ResponseData.Add((byte)ParameterCode.RoleTransformQueueSet,roleTransformQueueSetJson);
            ResponseData.Add((byte)ParameterCode.ResourcesUnitSet,resSetDictJson);
            OpResponse.Parameters = ResponseData;
            peer.SendOperationResponse(OpResponse, sendParameters);
            //广播事件
            threadEventParameter.Add((byte)ParameterCode.Role, roleJson);
            threadEventParameter.Add((byte)ParameterCode.RoleMoveStatus, roleMoveStatusJson);
            ExecuteThreadEvent(peerSet,EventCode.NewPlayer, threadEventParameter);
        }
    }
}
