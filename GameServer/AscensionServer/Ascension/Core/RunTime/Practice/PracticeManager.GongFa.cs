using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using RedisDotNet;
using AscensionProtocol.DTO;
using Protocol;
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
                var dict = await RedisHelper.Hash.HashGetAsync<Dictionary<string, int>>(RedisKeyDefine._RoleGongfaPerfix, RoleID.ToString());
                List<CultivationMethodDTO> gongFaIdList = new List<CultivationMethodDTO>();
                if (dict.Count != 0)
                {
                    foreach (var item in dict)
                    {
                        if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._GongfaPerfix, item.Key).Result)
                        {
                            gongFaIdList.Add(await RedisHelper.Hash.HashGetAsync<CultivationMethodDTO>(RedisKeyDefine._RoleGongfaPerfix, RoleID.ToString()));
                        }
                    }
                    OperationData opData = new OperationData();
                    opData.DataMessage = Utility.Json.ToJson(gongFaIdList);
                    opData.OperationCode = (byte)OperationCode.SyncPractice;
                    opData.ReturnCode = (byte)ReturnCode.Success;
                    GameEntry.RoleManager.SendMessage(RoleID, opData);
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
                var dict = await RedisHelper.Hash.HashGetAsync<Dictionary<string, int>>(RedisKeyDefine._RoleMiShuPerfix, RoleID.ToString());
                List<MiShuDTO> miShuIdList = new List<MiShuDTO>();
                if (dict.Count != 0)
                {
                    foreach (var item in dict)
                    {
                        if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._MiShuPerfix, item.Key).Result)
                        {
                            miShuIdList.Add(await RedisHelper.Hash.HashGetAsync<MiShuDTO>(RedisKeyDefine._RoleMiShuPerfix, RoleID.ToString()));
                        }
                    }
                    OperationData opData = new OperationData();
                    opData.DataMessage = Utility.Json.ToJson(miShuIdList);
                    opData.OperationCode = (byte)OperationCode.SyncPractice;
                    opData.ReturnCode = (byte)ReturnCode.Success;
                    GameEntry.RoleManager.SendMessage(RoleID, opData);
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
            OperationData opData = new OperationData();
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
                    opData.OperationCode = (byte)OperationCode.SyncPractice;
                    dataDict = new Dictionary<byte, object>();
                    dataDict.Add((byte)PracticeOpcode.GetRoleGongfa, gongFaIdList);
                    opData.DataMessage = Utility.Json.ToJson(dataDict);
                    GameEntry.RoleManager.SendMessage(RoleID, opData);
                }
                else
                {
                    opData.OperationCode = (byte)OperationCode.SyncPractice;
                    dataDict = new Dictionary<byte, object>();
                    dataDict.Add((byte)PracticeOpcode.GetRoleGongfa, null);
                    opData.DataMessage = Utility.Json.ToJson(dataDict);
                    GameEntry.RoleManager.SendMessage(RoleID, opData);
                    Utility.Debug.LogInfo("yzqjueseid发送成功");
                }
            }
            else
            {
                opData.OperationCode = (byte)OperationCode.SyncPractice;
                dataDict = new Dictionary<byte, object>();
                dataDict.Add((byte)PracticeOpcode.GetRoleGongfa, null);
                opData.DataMessage = Utility.Json.ToJson(dataDict);
                GameEntry.RoleManager.SendMessage(RoleID, opData);
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
                    opData.OperationCode = (byte)OperationCode.SyncPractice;
                    dataDict = new Dictionary<byte, object>();
                    dataDict.Add((byte)PracticeOpcode.GetRoleMiShu, mishuIdList);
                    opData.DataMessage = Utility.Json.ToJson(dataDict);
                    GameEntry.RoleManager.SendMessage(RoleID, opData);
                }
                else
                {
                    opData.OperationCode = (byte)OperationCode.SyncPractice;
                    dataDict = new Dictionary<byte, object>();
                    dataDict.Add((byte)PracticeOpcode.GetRoleMiShu, null);
                    opData.DataMessage = Utility.Json.ToJson(dataDict);
                    GameEntry.RoleManager.SendMessage(RoleID, opData);
                }
            }
            else
            {
                opData.OperationCode = (byte)OperationCode.SyncPractice;
                dataDict = new Dictionary<byte, object>();
                dataDict.Add((byte)PracticeOpcode.GetRoleMiShu, null);
                opData.DataMessage = Utility.Json.ToJson(dataDict);
                GameEntry.RoleManager.SendMessage(RoleID, opData);
    
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
        #endregion

    }
}

