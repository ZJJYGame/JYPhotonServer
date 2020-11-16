using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
    public class UpdateGongFaSubHandler : SyncGongFaSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Update;
        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var receivedRoleData = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Role));
            var receivedData = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.GongFa));
            var receivedRoleObj = Utility.Json.ToObject<RoleGongFa>(receivedRoleData);
            var receivedObj = Utility.Json.ToObject<CultivationMethod>(receivedData);
            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", receivedRoleObj.RoleID);
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, List<GongFa>>>(out var gongFaDataDict);


            bool exist = NHibernateQuerier.Verify<RoleGongFa>(nHCriteriaRoleID);
            int intInfoObj = 0;
            int intLevel = 0; 
            if (exist)
            {
                RoleGongFa GongfaInfo = NHibernateQuerier.CriteriaSelect<RoleGongFa>(nHCriteriaRoleID);
                NHCriteria nHCriteriaGongFaID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", receivedObj.ID);
                bool existGongFa = NHibernateQuerier.Verify<CultivationMethod>(nHCriteriaGongFaID);
                if (existGongFa)
                {
                    CultivationMethod GongfaInfoExp = NHibernateQuerier.CriteriaSelect<CultivationMethod>(nHCriteriaGongFaID);
                   var dicGongFaId =   Utility.Json.ToObject<Dictionary<int,int>>(GongfaInfo.GongFaIDArray);

                    if (dicGongFaId.ContainsKey(receivedObj.ID))
                    {
                        if (receivedObj.CultivationMethodLevel != 0)
                        {
                            GongfaInfoExp.CultivationMethodExp = 0;
                            intInfoObj = GongfaInfoExp.CultivationMethodExp + receivedObj.CultivationMethodExp;
                            intLevel = GongfaInfoExp.CultivationMethodLevel + receivedObj.CultivationMethodLevel;

                            NHibernateQuerier.Update(new CultivationMethod() { ID = GongfaInfoExp.ID, CultivationMethodID = GongfaInfoExp.CultivationMethodID, CultivationMethodLevel = (short)intLevel, CultivationMethodLevelSkillArray = GongfaInfoExp.CultivationMethodLevelSkillArray, CultivationMethodExp = intInfoObj });
                            Role role= NHibernateQuerier.CriteriaSelect<Role>(nHCriteriaRoleID);
                            role.RoleLevel= intLevel;
                          NHibernateQuerier.Update(role);

                            SetResponseParamters(() => {
                                operationResponse.Parameters = subResponseParameters;
                                subResponseParameters.Add((byte)ParameterCode.Role, Utility.Json.ToJson(new RoleDTO() { RoleID = role.RoleID, RoleFaction = role.RoleFaction, RoleGender = role.RoleGender, RoleLevel = role.RoleLevel, RoleName = role.RoleName, RoleRoot = role.RoleRoot, RoleTalent = role.RoleTalent }));
                                operationResponse.ReturnCode = (short)ReturnCode.Success;
                            });
                        }
                        else
                        {
                            intInfoObj = GongfaInfoExp.CultivationMethodExp + receivedObj.CultivationMethodExp;
                            NHibernateQuerier.Update(new CultivationMethod() { ID = GongfaInfoExp.ID, CultivationMethodID = GongfaInfoExp.CultivationMethodID, CultivationMethodLevel = GongfaInfoExp.CultivationMethodLevel, CultivationMethodLevelSkillArray = GongfaInfoExp.CultivationMethodLevelSkillArray, CultivationMethodExp = intInfoObj });
                            operationResponse.ReturnCode = (short)ReturnCode.ItemAlreadyExists;
                        }
                    }
                }             
                operationResponse.Parameters = subResponseParameters;
                GameManager.ReferencePoolManager.Despawn(nHCriteriaGongFaID);
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
