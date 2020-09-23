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
            Dictionary<int, int> gongfaDict;
            Dictionary<int, string> DOdict=new Dictionary<int, string>();

                if (roleGongFaObj != null)
                {
                if (!string.IsNullOrEmpty(roleGongFaObj.GongFaIDArray))
                    {
                    gongfaDict = Utility.Json.ToObject<Dictionary<int, int>>(roleGongFaObj.GongFaIDArray);
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

                        RedisHelper.Hash.HashSet<CultivationMethod>("CultivationMethod", cultivationMethod.CultivationMethodID.ToString(), cultivationMethod);
                        RedisHelper.Hash.HashSet<RoleGongFa>("RoleGongFa", roleObj.RoleID.ToString(), roleGongFaObj);
                        #endregion
                        SetResponseParamters(() =>
                            {
                                subResponseParameters.Add((byte)ParameterCode.RoleGongFa, Utility.Json.ToJson(DOdict));
                                operationResponse.ReturnCode = (short)ReturnCode.Success;
                            });
                        }
                    }
            }
            GameManager.ReferencePoolManager.Despawns(nHCriteriaRoleID);
            return operationResponse;
        }
    }
}
