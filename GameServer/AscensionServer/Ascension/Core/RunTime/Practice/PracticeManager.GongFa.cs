using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using RedisDotNet;
using AscensionProtocol.DTO;
using AscensionProtocol;
using AscensionServer.Model;

namespace AscensionServer
{
    public partial class PracticeManager
    {
        #region Redis模块
        /// <summary>
        /// 获得角色所有功法
        /// </summary>
        /// <param name="RoleID"></param>
        async void GetRoleGongFaS2C(int RoleID)
        {
            if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleGongfaPerfix, RoleID.ToString()).Result)
            {
                var dict = await RedisHelper.Hash.HashGetAsync<RoleGongFaDTO>(RedisKeyDefine._RoleGongfaPerfix, RoleID.ToString());
                List<CultivationMethodDTO> gongFaIdList = new List<CultivationMethodDTO>();
                if (dict.GongFaIDArray.Count != 0)
                {
                    foreach (var item in dict.GongFaIDArray)
                    {
                        if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._GongfaPerfix, item.Key.ToString()).Result)
                        {
                            gongFaIdList.Add(await RedisHelper.Hash.HashGetAsync<CultivationMethodDTO>(RedisKeyDefine._RoleGongfaPerfix, RoleID.ToString()));
                        }
                    }
                    Dictionary<byte, object> dictData = new Dictionary<byte, object>();
                    dictData.Add((byte)ParameterCode.GongFa, gongFaIdList);
                    dictData.Add((byte)ParameterCode.RoleGongFa, dict);
                    ResultSuccseS2C(RoleID,PracticeOpcode.GetRoleGongfa,dictData);
                }
                else
                    GetRoleGongFaMySql(RoleID);
            }
            else
                GetRoleGongFaMySql(RoleID);
        }
        /// <summary>
        /// 获得角色秘术模块
        /// </summary>
        /// <param name="RoleID"></param>
        async void GetRoleMiShuS2C(int RoleID)
        {
            if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleMiShuPerfix, RoleID.ToString()).Result)
            {
                var dict = await RedisHelper.Hash.HashGetAsync<RoleMiShuDTO>(RedisKeyDefine._RoleMiShuPerfix, RoleID.ToString());
                List<MiShuDTO> miShuIdList = new List<MiShuDTO>();
                if (dict.MiShuIDArray.Count != 0)
                {
                    foreach (var item in dict.MiShuIDArray)
                    {
                        if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._MiShuPerfix, item.Key.ToString()).Result)
                        {
                            miShuIdList.Add(await RedisHelper.Hash.HashGetAsync<MiShuDTO>(RedisKeyDefine._RoleMiShuPerfix, RoleID.ToString()));
                        }
                    }
                    Dictionary<byte, object> dataDict = new Dictionary<byte, object>();
                    dataDict.Add((byte)ParameterCode.MiShu, miShuIdList);
                    dataDict.Add((byte)ParameterCode.RoleMiShu, dict);
                    ResultSuccseS2C(RoleID, PracticeOpcode.GetRoleMiShu, dataDict);
                }
                else
                    GetRoleMiShuMySql(RoleID);
            }
            else
                GetRoleMiShuMySql(RoleID);
        }

        #endregion

        #region MySql模块
        /// <summary>
        /// 获取数据库数据
        /// </summary>
        /// <param name="RoleID"></param>
        async void GetRoleGongFaMySql(int RoleID)
        {
            List<NHCriteria> nHCriterias = new List<NHCriteria>();
            List<CultivationMethodDTO> gongFaIdList = new List<CultivationMethodDTO>();
            Dictionary<byte, object> dataDict = new Dictionary<byte, object>();
            NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", RoleID);
            var role = NHibernateQuerier.CriteriaSelectAsync<RoleGongFa>(nHCriteriaRole).Result;
            if (role != null)
            {
                if (!string.IsNullOrEmpty(role.GongFaIDArray))
                {
                    foreach (var item in Utility.Json.ToObject<Dictionary<int, int>>(role.GongFaIDArray))
                    {
                        NHCriteria nHCriteriaGongFa = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", item.Key);
                        nHCriterias.Add(nHCriteriaGongFa);
                        var gongfa = NHibernateQuerier.CriteriaSelectAsync<CultivationMethod>(nHCriteriaGongFa).Result;
                        if (gongfa != null)
                        {
                            gongFaIdList.Add(ChangeGongFa(gongfa));
                        }
                    }
                    dataDict = new Dictionary<byte, object>();
                    dataDict.Add((byte)ParameterCode.GongFa, gongFaIdList);                    
                    dataDict.Add((byte)ParameterCode.RoleGongFa, ChangeDataType(role));
                    ResultSuccseS2C(RoleID, PracticeOpcode.GetRoleGongfa, dataDict);
                }
                else
                {
                    ResultFailS2C(RoleID,PracticeOpcode.GetRoleGongfa);
                }
            }
            else
            {
                ResultFailS2C(RoleID, PracticeOpcode.GetRoleGongfa);
            }

        }
        /// <summary>
        /// 获取数据库数据
        /// </summary>
        /// <param name="RoleID"></param>
        async void GetRoleMiShuMySql(int RoleID)
        {
            List<NHCriteria> nHCriterias = new List<NHCriteria>();
            List<MiShuDTO> mishuIdList = new List<MiShuDTO>();
            OperationData opData = new OperationData();
            Dictionary<byte, object> dataDict = new Dictionary<byte, object>();
            NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", RoleID);
            var role = NHibernateQuerier.CriteriaSelectAsync<RoleMiShu>(nHCriteriaRole).Result;
            if (role != null)
            {
                if (!string.IsNullOrEmpty(role.MiShuIDArray))
                {
                    foreach (var item in Utility.Json.ToObject<Dictionary<int, int>>(role.MiShuIDArray))
                    {
                        NHCriteria nHCriteriaMiShu = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", item.Key);
                        nHCriterias.Add(nHCriteriaMiShu);
                        var mishu = NHibernateQuerier.CriteriaSelectAsync<MiShu>(nHCriteriaMiShu).Result;
                        if (mishu != null)
                        {
                            mishuIdList.Add(ChangeMiShu(mishu));
                        }
                    }
                    dataDict = new Dictionary<byte, object>();
                    dataDict.Add((byte)ParameterCode.MiShu, mishuIdList);
                    dataDict.Add((byte)ParameterCode.RoleMiShu, ChangeDataType(role));
                    ResultSuccseS2C(RoleID, PracticeOpcode.GetRoleMiShu, dataDict);
                }
                else
                {
                    ResultFailS2C(RoleID, PracticeOpcode.GetRoleMiShu); 
                }
            }
            else
            {
                ResultFailS2C(RoleID, PracticeOpcode.GetRoleMiShu);
            }

        }
        #endregion
    
        #region
        CultivationMethodDTO ChangeGongFa(CultivationMethod cultivation)
        {
            CultivationMethodDTO cultivationMethodDTO = new CultivationMethodDTO();
            cultivationMethodDTO.CultivationMethodExp = cultivation.CultivationMethodExp;
            cultivationMethodDTO.CultivationMethodID = cultivation.CultivationMethodID;
            cultivationMethodDTO.ID = cultivation.ID;
            cultivationMethodDTO.CultivationMethodLevel = cultivation.CultivationMethodLevel;
            cultivationMethodDTO.CultivationMethodLevelSkillArray = Utility.Json.ToObject<List<int>>(cultivation.CultivationMethodLevelSkillArray);
            return cultivationMethodDTO;
        }

        MiShuDTO ChangeMiShu(MiShu miShu)
        {
            MiShuDTO miShuDTO = new MiShuDTO();
            miShuDTO.ID = miShu.ID;
            miShuDTO.MiShuAdventureSkill = Utility.Json.ToObject<List<int>>(miShu.MiShuAdventtureSkill);
            miShuDTO.MiShuExp = miShu.MiShuExp;
            miShuDTO.MiShuID = miShu.MiShuID;
            miShuDTO.MiShuLevel = miShu.MiShuLevel;
            miShuDTO.MiShuSkillArry = Utility.Json.ToObject<List<int>>(miShu.MiShuSkillArry);

            return miShuDTO;
        }

        RoleGongFaDTO ChangeDataType(RoleGongFa roleGongFa)
        {
            RoleGongFaDTO roleGongFaObj = new RoleGongFaDTO();
            roleGongFaObj.RoleID = roleGongFa.RoleID;
            roleGongFaObj.GongFaIDArray = Utility.Json.ToObject<Dictionary<int ,int>>(roleGongFa.GongFaIDArray);
            return roleGongFaObj;
        }

        RoleMiShuDTO ChangeDataType(RoleMiShu roleMiShu)
        {
            RoleMiShuDTO roleMiShuObj = new RoleMiShuDTO();
            roleMiShuObj.RoleID = roleMiShu.RoleID;
            roleMiShuObj.MiShuIDArray = Utility.Json.ToObject<Dictionary<int, int>>(roleMiShu.MiShuIDArray);
            return roleMiShuObj;
        }
        #endregion

    }
}

