using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using Cosmos;
using AscensionProtocol;
using AscensionServer.Threads;

namespace AscensionServer
{
    public class ExitAdventureSceneHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.ExitAdventureScene; } }
        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            //AscensionServer.Instance.ExitAdventureScene(peer);
            //这条，获取玩家已经离开探索界面时候所有玩家的集合
            //var peerSet = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();

            responseParameters.Clear();
            operationResponse.OperationCode = operationRequest.OperationCode;
            operationResponse.ReturnCode = (byte)ReturnCode.Success;
            //peer.SendOperationResponse(OpResponse, sendParameters);

            //var roleJson = Utility.Json.ToJson(peer.PeerCache.Role);
            //var roleMoveStatusJson = Utility.Json.ToJson(peer.PeerCache.RoleMoveStatus);
            //threadEventParameter.Clear();
            //广播事件
            //threadEventParameter.Add((byte)ParameterCode.Role, roleJson);
            //threadEventParameter.Add((byte)ParameterCode.RoleMoveStatus, roleMoveStatusJson);
            //QueueThreadEvent(peerSet, EventCode.DeletePlayer, threadEventParameter);
            return operationResponse;
        }
    }
}
