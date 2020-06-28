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
            string roleJson=Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleBottleneck));
            var bottleneckObj = Utility.Json.ToObject<Bottleneck>(roleJson);
            NHCriteria nHCriteriaBottleneck = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", bottleneckObj.RoleID);
          
            Bottleneck bottleneck = Singleton<NHManager>.Instance.CriteriaSelect<Bottleneck>(nHCriteriaBottleneck);
            if (bottleneck!=null)
            {
                Singleton<NHManager>.Instance.Update(bottleneckObj);
            }
            else
            {
                Singleton<NHManager>.Instance.Insert(bottleneckObj);
            }
            //peer.SendOperationResponse(OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaBottleneck);
        }
    }
}
