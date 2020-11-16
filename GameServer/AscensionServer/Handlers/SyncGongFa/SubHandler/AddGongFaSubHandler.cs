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
using RedisDotNet;
using Protocol;
namespace AscensionServer
{
    public class AddGongFaSubHandler : SyncGongFaSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Add;
        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string gfJson = Convert.ToString(Utility.GetValue(dict,(byte)ParameterCode.GongFa));
            string  roleJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Role));

            var roleObj = Utility.Json.ToObject<Role>(roleJson);
            var gongfaObj = Utility.Json.ToObject<CultivationMethod>(gfJson);
            Utility.Debug.LogInfo("添加的新的功法为"+ roleJson);

            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
            var roleGongFaObj = NHibernateQuerier.CriteriaSelect<RoleGongFa>(nHCriteriaRoleID);
            var roletemp= NHibernateQuerier.CriteriaSelect<Role>(nHCriteriaRoleID);
            Dictionary<int, int> gongfaDict;
            Dictionary<int, string> DOdict=new Dictionary<int, string>();
                if (roleGongFaObj != null)
                {
                gongfaDict = Utility.Json.ToObject<Dictionary<int, int>>(roleGongFaObj.GongFaIDArray);
                if (gongfaDict.Count>0)
                    {
                        if (gongfaDict.Values.ToList().Contains(gongfaObj.CultivationMethodID))
                        {
                            Utility.Debug.LogInfo("人物已经学会的功法" + roleGongFaObj.GongFaIDArray);
                            Utility.Debug.LogInfo("人物已经学会此功法无法添加新的功法" + gongfaObj.CultivationMethodID);
                            operationResponse.ReturnCode = (short)ReturnCode.Fail;
                            subResponseParameters.Add((byte)ParameterCode.RoleGongFa, null);
                        }
                        else
                        {
                            CultivationMethod cultivationMethod = new CultivationMethod() { CultivationMethodID = gongfaObj.CultivationMethodID, CultivationMethodLevel = gongfaObj.CultivationMethodLevel };
                            cultivationMethod = NHibernateQuerier.Insert(cultivationMethod);
                            gongfaDict.Add(cultivationMethod.ID, cultivationMethod.CultivationMethodID);
                            roleGongFaObj.GongFaIDArray = Utility.Json.ToJson(gongfaDict);
                            NHibernateQuerier.Update(roleGongFaObj);
                            DOdict.Add(0, Utility.Json.ToJson(cultivationMethod));
                            DOdict.Add(1, Utility.Json.ToJson(roleGongFaObj));
                        #region Redis模块
                        RedisHelper.Hash.HashSet<CultivationMethod>(RedisKeyDefine._GongfaPerfix, cultivationMethod.CultivationMethodID.ToString(), cultivationMethod);
                        RedisHelper.Hash.HashSet<RoleGongFa>(RedisKeyDefine._RoleGongfaPerfix, roleObj.RoleID.ToString(), roleGongFaObj);
                        #endregion
                        SetResponseParamters(() =>
                            {
                                subResponseParameters.Add((byte)ParameterCode.RoleGongFa, Utility.Json.ToJson(DOdict));
                                operationResponse.ReturnCode = (short)ReturnCode.Success;
                            });
                        }
                }
                else
                {
                    CultivationMethod cultivationMethod = new CultivationMethod() { CultivationMethodID = gongfaObj.CultivationMethodID, CultivationMethodLevel = gongfaObj.CultivationMethodLevel };
                    cultivationMethod = NHibernateQuerier.Insert(cultivationMethod);
                    gongfaDict.Add(cultivationMethod.ID, cultivationMethod.CultivationMethodID);
                    roleGongFaObj.GongFaIDArray = Utility.Json.ToJson(gongfaDict);
                    NHibernateQuerier.Update(roleGongFaObj);
                    roleGongFaObj.GongFaIDArray = Utility.Json.ToJson(gongfaDict);
                    NHibernateQuerier.Update(roleGongFaObj);
                    roletemp.RoleLevel = 1;
                    DOdict.Add(0, Utility.Json.ToJson(cultivationMethod));
                    DOdict.Add(1, Utility.Json.ToJson(new RoleDTO() { RoleID= roletemp .RoleID,RoleFaction= roletemp .RoleFaction,RoleGender= roletemp .RoleGender,RoleLevel= roletemp.RoleLevel,RoleName= roletemp .RoleName,RoleRoot= roletemp .RoleRoot,RoleTalent= roletemp .RoleTalent}));
                    NHibernateQuerier.Update<Role>(roletemp);
                    OperationData operationData = new OperationData();
                    operationData.DataMessage = Utility.Json.ToJson(DOdict);
                    operationData.OperationCode = (byte)OperationCode.AddFirstGongfa;
                    GameManager.CustomeModule<RoleManager>().SendMessage(roleObj.RoleID, operationData);
                    #region Redis模块
                    RedisHelper.Hash.HashSet<CultivationMethod>(RedisKeyDefine._RoleGongfaPerfix, cultivationMethod.CultivationMethodID.ToString(), cultivationMethod);
                    RedisHelper.Hash.HashSet<RoleGongFa>(RedisKeyDefine._GongfaPerfix, roleObj.RoleID.ToString(), roleGongFaObj);
                    #endregion
                }
            }
            GameManager.ReferencePoolManager.Despawns(nHCriteriaRoleID);
            return operationResponse;
        }
    }
}
