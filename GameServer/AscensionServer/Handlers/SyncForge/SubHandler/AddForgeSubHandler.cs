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
   public  class AddForgeSubHandler:SyncForgeSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Add;
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string forgeJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.JobForge));
            var forgeObj = Utility.Json.ToObject<ForgeDTO>(forgeJson);
            NHCriteria nHCriteriaforge = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", forgeObj.RoleID);
            var forgeTemp = NHibernateQuerier.CriteriaSelect<Forge>(nHCriteriaforge);
            HashSet<int> forgeHash = new HashSet<int>();
            if (forgeTemp!=null)
            {
                if (string.IsNullOrEmpty(forgeTemp.Recipe_Array))
                {
                    forgeTemp.Recipe_Array = Utility.Json.ToJson(forgeObj.Recipe_Array);
                   
                    NHibernateQuerier.Update(forgeTemp);
                }
                else
                {
                   forgeHash = Utility.Json.ToObject<HashSet<int>>(forgeTemp.Recipe_Array);
                    forgeHash.Add(forgeObj.Recipe_Array.First());
                    forgeTemp.Recipe_Array = Utility.Json.ToJson(forgeHash);
                    NHibernateQuerier.Update(forgeTemp);
                }
                SetResponseData(() =>
                {
                    forgeObj = new ForgeDTO() { RoleID = forgeTemp.RoleID, JobLevel = forgeTemp.JobLevel, JobLevelExp = forgeTemp.JobLevelExp, Recipe_Array = forgeHash };
                    SubDict.Add((byte)ParameterCode.JobForge, Utility.Json.ToJson(forgeObj));
                    Owner.OpResponseData.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
                Owner.OpResponseData.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponseData, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaforge);
        }

    }
}
