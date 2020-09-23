using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
    public class UpdateMiShuSubHandler : SyncMiShuSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Update;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            ResetResponseData(operationRequest);
            var receivedRoleData = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Role));
            var receivedData = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.MiShu));
            Utility.Debug.LogInfo(">>>>>>>>>>>>接收秘术数据：" + receivedRoleData + ">>>>>>>>>>>>>>>>>>>>>>");
            Utility.Debug.LogInfo(">>>>>>>>>>>>接收秘术数据：" + receivedData + ">>>>>>>>>>>>>>>>>>>>>>");
            var receivedRoleObj = Utility.Json.ToObject<RoleMiShu>(receivedRoleData);
            var receivedObj = Utility.Json.ToObject<MiShu>(receivedData);
            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", receivedRoleObj.RoleID);
            bool exist = NHibernateQuerier.Verify<RoleMiShu>(nHCriteriaRoleID);
            int intInfoObj = 0;
            int intLevel = 0;
            if (exist)
            {
                RoleMiShu MishuInfo = NHibernateQuerier.CriteriaSelect<RoleMiShu>(nHCriteriaRoleID);
                NHCriteria nHCriteriaMiShuID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", receivedObj.ID);
                bool existMiShu = NHibernateQuerier.Verify<MiShu>(nHCriteriaMiShuID);
                if (existMiShu)
                {
                    MiShu MishuInfoExp = NHibernateQuerier.CriteriaSelect<MiShu>(nHCriteriaMiShuID);
                    foreach (var item in Utility.Json.ToObject<Dictionary<int,int>>(MishuInfo.MiShuIDArray))
                    {
                        if (item.Key == receivedObj.ID)
                        {
                            if (receivedObj.MiShuLevel != 0)
                            {
                                MishuInfoExp.MiShuExp = 0;
                                intInfoObj = MishuInfoExp.MiShuExp + receivedObj.MiShuExp;
                                intLevel = MishuInfoExp.MiShuLevel + receivedObj.MiShuLevel;
                                NHibernateQuerier.Update(new MiShu() { ID = MishuInfoExp.ID, MiShuID = MishuInfoExp.MiShuID, MiShuLevel = (short)intLevel, MiShuSkillArry = MishuInfoExp.MiShuSkillArry, MiShuExp = intInfoObj });
                            }
                            else
                            {
                                intInfoObj = MishuInfoExp.MiShuExp + receivedObj.MiShuExp;
                                NHibernateQuerier.Update(new MiShu() { ID = MishuInfoExp.ID, MiShuID = MishuInfoExp.MiShuID, MiShuLevel = MishuInfoExp.MiShuLevel, MiShuSkillArry = MishuInfoExp.MiShuSkillArry, MiShuExp = intInfoObj });

                            }
                        }
                    }
                }

                operationResponse.Parameters = subResponseParameters;
                operationResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else
            {
                operationResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            GameManager.ReferencePoolManager.Despawn(nHCriteriaRoleID);
            return operationResponse;
        }
    }
}

