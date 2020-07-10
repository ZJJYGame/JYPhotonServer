using AscensionProtocol;
using Photon.SocketServer;
using System;
using Cosmos;
using AscensionProtocol.DTO;
using AscensionServer.Threads;
using System.Collections.Generic;
namespace AscensionServer
{
    public class SyncResourcesHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncResources;
            base.OnInitialization();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResponseData.Clear();
            var roleMoveStatusJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.ResourcesUnitSet));
            var roleObj = Utility.Json.ToObject<int>(roleMoveStatusJson); ;
            AscensionServer._Log.Info("ID " + peer.PeerCache.RoleID + "\n RoleJson :" + roleMoveStatusJson);
            if (AscensionServer.Instance.resDic.ContainsKey(roleObj))
            {
                ResponseData.Add((byte)ParameterCode.ResourcesUnitSet, Utility.Json.ToJson(AscensionServer.Instance.resDic[roleObj]));
                OpResponse.OperationCode = operationRequest.OperationCode;
                OpResponse.ReturnCode = (short)ReturnCode.Success;
                OpResponse.Parameters = ResponseData;
            }
            else
            {
                OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            peer.SendOperationResponse(OpResponse, sendParameters);
        }
    }
}