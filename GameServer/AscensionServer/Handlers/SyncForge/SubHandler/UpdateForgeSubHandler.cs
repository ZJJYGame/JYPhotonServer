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
    public class UpdateForgeSubHandler : SyncForgeSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string forgeJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.JobForge));
            var forgeObj = Utility.Json.ToObject<Forge>(forgeJson);
            NHCriteria nHCriteriaforge = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", forgeObj.RoleID);
            var forgeTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Forge>(nHCriteriaforge);
            int Level = 0;
            int Exp = 0;
            if (forgeTemp!=null)
            {
                if (forgeObj.JobLevel != 0)
                {
                    Level = forgeTemp.JobLevel + forgeObj.JobLevel;
                    ConcurrentSingleton<NHManager>.Instance.Update(new Forge() { RoleID = forgeTemp.RoleID, JobLevel = Level, JobLevelExp = forgeObj.JobLevelExp, Recipe_Array = forgeTemp.Recipe_Array });
                    AscensionServer._Log.Info("传输回去的锻造数据1" + Utility.Json.ToJson(forgeTemp));
                }
                else
                {
                    Exp = forgeTemp.JobLevelExp + forgeObj.JobLevelExp;
                    ConcurrentSingleton<NHManager>.Instance.Update(new Forge() { RoleID = forgeTemp.RoleID, JobLevel = forgeTemp.JobLevel, JobLevelExp = Exp, Recipe_Array = forgeTemp.Recipe_Array });
                    AscensionServer._Log.Info("传输回去的锻造数据2" + Utility.Json.ToJson(forgeTemp));
                }
                SetResponseData(() =>
                {
                    var forge = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Forge>(nHCriteriaforge);

                    SubDict.Add((byte)ParameterCode.JobForge, Utility.Json.ToJson(forge));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaforge);
        }
    }
}
