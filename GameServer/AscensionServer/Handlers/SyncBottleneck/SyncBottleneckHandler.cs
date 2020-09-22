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
        public override byte OpCode { get { return (byte)OperationCode.SyncBottleneck; } }
        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            string roleJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleBottleneck));
           // AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>传过来的瓶颈状态" + roleJson);
            var bottleneckObj = Utility.Json.ToObject<Bottleneck>(roleJson);
            NHCriteria nHCriteriaBottleneck = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", bottleneckObj.RoleID);


            Bottleneck bottleneck = NHibernateQuerier.CriteriaSelect<Bottleneck>(nHCriteriaBottleneck);
            if (bottleneck != null)
            {
                NHibernateQuerier.Update(bottleneckObj);
                opResponseData.ReturnCode = (short)ReturnCode.Success;
            }
            else
            {
                NHibernateQuerier.Insert(bottleneckObj);
                opResponseData.ReturnCode = (short)ReturnCode.Success;
            }
            GameManager.ReferencePoolManager.Despawns(nHCriteriaBottleneck);
            return opResponseData;
        }
    }
}
