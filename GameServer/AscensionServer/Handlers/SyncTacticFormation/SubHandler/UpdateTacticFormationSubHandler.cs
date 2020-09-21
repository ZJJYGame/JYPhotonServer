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
    public class UpdateTacticFormationSubHandler : SyncTacticFormationSubHandler

    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string tacticformationJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.JobTacticFormation));
            var tacticformationObj = Utility.Json.ToObject<TacticFormationDTO>(tacticformationJson);
            NHCriteria  nHCriteriatacticformation = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", tacticformationObj.RoleID);
            var tacticformationTemp =NHibernateQuerier.CriteriaSelect<TacticFormation>(nHCriteriatacticformation);
            int Level = 0;
            int Exp = 0;
            //AscensionServer._Log.Info("传输回去的锻造数据" + forgeJson);

            if (tacticformationTemp != null)
            {
                if (tacticformationObj.JobLevel != 0)
                {
                    Level = tacticformationTemp.JobLevel + tacticformationObj.JobLevel;
                    tacticformationObj = new TacticFormationDTO() { RoleID = tacticformationTemp.RoleID, JobLevel = Level, JobLevelExp = tacticformationObj.JobLevelExp, Recipe_Array = Utility.Json.ToObject<HashSet<int>>(tacticformationTemp.Recipe_Array) };
                    NHibernateQuerier.Update(tacticformationObj);

                }
                else
                {
                    Exp = tacticformationTemp.JobLevelExp + tacticformationObj.JobLevelExp;
                    tacticformationObj = new TacticFormationDTO() { RoleID = tacticformationTemp.RoleID, JobLevel = tacticformationTemp.JobLevel, JobLevelExp = Exp, Recipe_Array =Utility.Json.ToObject<HashSet<int>>(tacticformationTemp.Recipe_Array) };
                    NHibernateQuerier.Update(tacticformationObj);
                }
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.JobTacticFormation, Utility.Json.ToJson(tacticformationObj));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriatacticformation);
        }
    }
}
