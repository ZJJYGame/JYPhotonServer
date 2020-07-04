using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;
using AscensionProtocol.DTO;

namespace AscensionServer
{
    public class AddAlchemySubHandler : SyncAlchemySubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string alchemyJson = Convert.ToString(Utility.GetValue(dict,(byte)ParameterCode.JobAlchemy));
            var alchemyObj = Utility.Json.ToObject<Alchemy>(alchemyJson);
            NHCriteria nHCriteriaAlchemy = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", alchemyObj.RoleID);
            var alchemyTemp = Singleton<NHManager>.Instance.CriteriaSelect<Alchemy>(nHCriteriaAlchemy);
            if (alchemyTemp!=null)
            {
                if (string.IsNullOrEmpty(alchemyTemp.Recipe_Array))
                {
                    var alchemyHash = Utility.Json.ToObject<HashSet<int>>(alchemyTemp.Recipe_Array);
                    alchemyHash.Add(Convert.ToInt16(alchemyObj.Recipe_Array));
                    alchemyTemp.Recipe_Array = Utility.Json.ToJson(alchemyHash);
                    Singleton<NHManager>.Instance.Update(alchemyTemp);
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                }
                else
                {
                    alchemyTemp.Recipe_Array = Utility.Json.ToJson(alchemyObj.Recipe_Array);
                    Singleton<NHManager>.Instance.Update(alchemyTemp);
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                }
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaAlchemy);
        }
    }
}
