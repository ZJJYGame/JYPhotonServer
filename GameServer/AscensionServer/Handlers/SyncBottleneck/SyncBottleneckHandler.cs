using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using AscensionServer.Model;
using System.Collections.Generic;
using System;
using Cosmos;

namespace AscensionServer
{
   public class SyncBottleneckHandler:Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncBottleneck;
            base.OnInitialization();

        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string roleJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleBottleneck));
           // AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>传过来的瓶颈状态" + roleJson);
            var bottleneckObj = Utility.Json.ToObject<Bottleneck>(roleJson);
            NHCriteria nHCriteriaBottleneck = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", bottleneckObj.RoleID);


            Bottleneck bottleneck = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Bottleneck>(nHCriteriaBottleneck);
            if (bottleneck != null)
            {
                ConcurrentSingleton<NHManager>.Instance.Update(bottleneckObj);
                OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else
            {
                ConcurrentSingleton<NHManager>.Instance.Insert(bottleneckObj);
                OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            peer.SendOperationResponse(OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaBottleneck);
        }
    }
}
