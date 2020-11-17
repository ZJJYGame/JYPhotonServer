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
    public class AddMiShuSubHandler : SyncMiShuSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Add;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string msJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.MiShu));
            string roleJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Role));

            var roleObj = Utility.Json.ToObject<Role>(roleJson);
            var mishuObj = Utility.Json.ToObject<MiShu>(msJson);
            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
            var roleMiShuObj = NHibernateQuerier.CriteriaSelect<RoleMiShu>(nHCriteriaRoleID);
            if (roleMiShuObj==null)
            {
                roleMiShuObj = GameManager.ReferencePoolManager.Spawn<RoleMiShu>();
                roleMiShuObj = NHibernateQuerier.Insert<RoleMiShu>(roleMiShuObj);
            }
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, MiShuData>>(out var mishuDataDict);
           
            var rolemishuredisObj= RedisHelper.Hash.HashGet<RoleMiShuDTO>(RedisKeyDefine._MiShuPerfix, roleObj.RoleID.ToString());
            var roleMishuMySQL = Utility.Json.ToObject<List<int>>(roleMiShuObj.MiShuIDArray);
            if (!roleMishuMySQL.Contains(mishuObj.MiShuID))
            {
                #region 生成新的秘术
                var mishuRedisObj = GameManager.ReferencePoolManager.Spawn<MiShuDTO>();
                var mishuMysqlObj = GameManager.ReferencePoolManager.Spawn<MiShu>();
                mishuMysqlObj.MiShuID = mishuObj.MiShuID;
                mishuMysqlObj = NHibernateQuerier.InsertAsync<MiShu>(mishuMysqlObj).Result;
                mishuRedisObj.ID = mishuMysqlObj.ID;
                mishuRedisObj.MiShuID = mishuMysqlObj.MiShuID;

                if (mishuDataDict[mishuObj.MiShuID].mishuSkillDatas[0].Skill_Array_One.Count > 0)
                {
                    mishuRedisObj.MiShuSkillArry = new List<int>() { mishuDataDict[mishuObj.MiShuID].mishuSkillDatas[0].Skill_Array_One[0] };
                }
                if (mishuDataDict[mishuObj.MiShuID].mishuSkillDatas[0].Skill_Array_Two.Count > 0)
                {
                    mishuRedisObj.MiShuAdventureSkill = new List<int>() { mishuDataDict[mishuObj.MiShuID].mishuSkillDatas[0].Skill_Array_Two[0] };
                }
                mishuMysqlObj.MiShuAdventtureSkill = Utility.Json.ToJson(mishuRedisObj.MiShuAdventureSkill);
                mishuMysqlObj.MiShuSkillArry = Utility.Json.ToJson(mishuRedisObj.MiShuSkillArry);

                #endregion
                //Redis存在先存
                if (rolemishuredisObj != null)
                {
                    if (!rolemishuredisObj.MiShuIDArray.Contains(mishuObj.MiShuID))
                    {
                        NHibernateQuerier.UpdateAsync(mishuMysqlObj);
                        RedisHelper.Hash.HashSetAsync<MiShuDTO>(RedisKeyDefine._MiShuPerfix + roleObj.RoleID, roleObj.RoleID.ToString(), mishuRedisObj);

                        rolemishuredisObj.MiShuIDArray.Add(mishuRedisObj.MiShuID);
                        RedisHelper.Hash.HashSetAsync<RoleMiShuDTO>(RedisKeyDefine._RoleMiShuPerfix + roleObj.RoleID, roleObj.RoleID.ToString(), rolemishuredisObj);

                        roleMiShuObj.MiShuIDArray = Utility.Json.ToJson(rolemishuredisObj);
                        NHibernateQuerier.UpdateAsync(roleMiShuObj);
                    }
                }
                //Redis没有先存MySQL
                else
                {
                    NHibernateQuerier.UpdateAsync(mishuMysqlObj);
                    RedisHelper.Hash.HashSetAsync<MiShuDTO>(RedisKeyDefine._MiShuPerfix + roleObj.RoleID, roleObj.RoleID.ToString(), mishuRedisObj);

                    rolemishuredisObj = GameManager.ReferencePoolManager.Spawn<RoleMiShuDTO>();
                    rolemishuredisObj.MiShuIDArray = new List<int>() { mishuRedisObj.MiShuID };
                    RedisHelper.Hash.HashSetAsync<RoleMiShuDTO>(RedisKeyDefine._RoleMiShuPerfix + roleObj.RoleID, roleObj.RoleID.ToString(), rolemishuredisObj);

                    roleMiShuObj.MiShuIDArray = Utility.Json.ToJson(rolemishuredisObj);
                    NHibernateQuerier.UpdateAsync(roleMiShuObj);
                }
                SetResponseParamters(() =>
                {
                    subResponseParameters.Add((byte)ParameterCode.MiShu, Utility.Json.ToJson(mishuRedisObj));                 
                    subResponseParameters.Add((byte)ParameterCode.RoleMiShu, Utility.Json.ToJson(rolemishuredisObj));
                    operationResponse.ReturnCode = (byte)ReturnCode.Success;
                });
                GameManager.ReferencePoolManager.Despawns(mishuRedisObj, mishuMysqlObj, nHCriteriaRoleID);
            }
            else
            {
                SetResponseParamters(() =>
                {
                    operationResponse.ReturnCode = (byte)ReturnCode.Fail;
                });
            }
            return operationResponse;
        }

    }
}
