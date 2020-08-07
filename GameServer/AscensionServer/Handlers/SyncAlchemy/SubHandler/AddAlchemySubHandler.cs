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
            var alchemyObj = Utility.Json.ToObject<AlchemyDTO>(alchemyJson);
            NHCriteria nHCriteriaAlchemy = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", alchemyObj.RoleID);
            var alchemyTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Alchemy>(nHCriteriaAlchemy);
            HashSet<int> alchemyHash=new HashSet<int>();
            if (alchemyTemp!=null)
            {             
                if (string.IsNullOrEmpty(alchemyTemp.Recipe_Array))
                {                 
                    alchemyTemp.Recipe_Array = Utility.Json.ToJson(alchemyObj.Recipe_Array);
                    ConcurrentSingleton<NHManager>.Instance.Update(alchemyTemp);
                }
                else
                {
                    alchemyHash = Utility.Json.ToObject<HashSet<int>>(alchemyTemp.Recipe_Array);
                    alchemyHash.Add(alchemyObj.Recipe_Array.First());
                    alchemyTemp.Recipe_Array = Utility.Json.ToJson(alchemyHash);
                    ConcurrentSingleton<NHManager>.Instance.Update(alchemyTemp);
                }
                SetResponseData(() =>
                {
                    alchemyObj = new AlchemyDTO() {RoleID= alchemyTemp.RoleID,JobLevel= alchemyTemp.JobLevel,JobLevelExp= alchemyTemp.JobLevelExp, Recipe_Array = alchemyHash };

                    SubDict.Add((byte)ParameterCode.JobAlchemy, alchemyObj);
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;

                });
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaAlchemy);
        }
    }
}
