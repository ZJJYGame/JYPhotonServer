using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
   public class GetAlchemySubHandler:SyncAlchemySubHandler
    {

        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string alchemyJson = Convert.ToString(Utility.GetValue(dict,(byte)ParameterCode.JobAlchemy));
            var alchemyObj = Utility.Json.ToObject<Alchemy>(alchemyJson);
            NHCriteria nHCriteriaalchemy = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", alchemyObj.RoleID);
            var alchemytemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Alchemy>(nHCriteriaalchemy);
            if (alchemytemp!=null)
            {
                if (!string.IsNullOrEmpty(alchemytemp.Recipe_Array))
                {
                    SetResponseData(() =>
                    {
                        SubDict.Add((byte)ParameterCode.JobAlchemy, Utility.Json.ToJson(alchemytemp));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
            }
            else
            {
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                SubDict.Add((byte)ParameterCode.JobAlchemy, Utility.Json.ToJson(new List<string>()));
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaalchemy);
        }

       
    }
}
