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
            NHCriteria nHCriteriaBottleneck = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", bottleneckObj.RoleID);


            Bottleneck bottleneck = NHibernateQuerier.CriteriaSelect<Bottleneck>(nHCriteriaBottleneck);
            if (bottleneck != null)
            {
                NHibernateQuerier.Update(bottleneckObj);
                OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else
            {
                NHibernateQuerier.Insert(bottleneckObj);
                OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            peer.SendOperationResponse(OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaBottleneck);
        }
    }
}
