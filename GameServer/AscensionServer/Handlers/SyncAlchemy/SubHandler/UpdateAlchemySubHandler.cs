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
    public class UpdateAlchemySubHandler : SyncAlchemySubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string alchemyJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.JobAlchemy));
            var alchemyObj = Utility.Json.ToObject<AlchemyDTO>(alchemyJson);
            AscensionServer._Log.Info("传输回去的炼丹数据" + alchemyJson);
            NHCriteria nHCriteriaAlchemy = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", alchemyObj.RoleID);
            var alchemyTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Alchemy>(nHCriteriaAlchemy);
            int Level = 0;
            int Exp = 0;

            if (alchemyTemp!=null)
            {
                if (alchemyObj.JobLevel!=0)
                {
                    Level = alchemyTemp.JobLevel + alchemyObj.JobLevel;
                    ConcurrentSingleton<NHManager>.Instance.Update(new Alchemy() {RoleID= alchemyTemp.RoleID,JobLevel= Level,JobLevelExp= alchemyObj.JobLevelExp,Recipe_Array= alchemyTemp.Recipe_Array });
                    AscensionServer._Log.Info("传输回去的炼丹数据1" + Utility.Json.ToJson(alchemyTemp));
                }
                else
                {
                    Exp= alchemyTemp.JobLevelExp + alchemyObj.JobLevelExp;
                    ConcurrentSingleton<NHManager>.Instance.Update(new Alchemy() { RoleID = alchemyTemp.RoleID, JobLevel = alchemyTemp.JobLevel, JobLevelExp = Exp, Recipe_Array = alchemyTemp.Recipe_Array });
                    AscensionServer._Log.Info("传输回去的炼丹数据2" + Utility.Json.ToJson(alchemyTemp));
                }

                SetResponseData(() =>
                {
                    var alchemy = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Alchemy>(nHCriteriaAlchemy);
              
                    SubDict.Add((byte)ParameterCode.JobAlchemy, Utility.Json.ToJson(alchemy));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
          
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaAlchemy);
        }
      
    }
}
