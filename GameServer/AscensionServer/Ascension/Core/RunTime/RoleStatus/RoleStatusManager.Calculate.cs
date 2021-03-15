using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedisDotNet;
namespace AscensionServer
{
   public partial class RoleStatusManager
    {
        public  void CalculateRoleStatus(List<GongFa> gongfaList, List<MishuSkillData> mishuList, RoleStatusDatas roleStatusDatas, out RoleStatus roleStatus)
        {
            var roleStatustemp = CosmosEntry.ReferencePoolManager.Spawn<RoleStatus>();
            roleStatustemp.Clear();
            roleStatus = CosmosEntry.ReferencePoolManager.Spawn<RoleStatus>();
            if (gongfaList.Count > 0)
            {
                for (int i = 0; i < gongfaList.Count; i++)
                {

                }
            }
            if (mishuList.Count > 0)
            {
                for (int i = 0; i < mishuList.Count; i++)
                {

                }
            }
            OutRolestatus(roleStatus, roleStatusDatas, roleStatustemp, out var tempstatus);
            roleStatus = tempstatus;
            // CosmosEntry.ReferencePoolManager.Despawns(roleStatustemp, roleStatus);
        }

        public  void OutRolestatus(RoleStatus roleStatus, RoleStatusDatas roleStatusDatas, RoleStatus roleStatusTemp, out RoleStatus tempstatus)
        {
            tempstatus = new RoleStatus();
           

        }

        /// <summary>
        ///获取数据库功法秘术数值
        /// </summary>
        /// <param name="gongfaList"></param>
        /// <param name="mishuList"></param>
        /// <param name="roleStatusDatas"></param>
        public void GetRoleStatus(int roleID)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, List<MishuSkillData>>>(out var mishuDict);

            GameEntry.DataManager.TryGetValue<Dictionary<int, GongFa>>(out var gongfaDict);
            List<int> gongfaList = new List<int>();
            List<MishuSkillData> mishuList = new List<MishuSkillData>();
            #region 获取角色所有功法
            if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleGongfaPerfix, roleID.ToString()).Result)
            {
                var roleGongfa = RedisHelper.Hash.HashGetAsync<RoleGongFaDTO>(RedisKeyDefine._RoleGongfaPerfix, roleID.ToString()).Result;
                if (roleGongfa != null)
                {
                    gongfaList = roleGongfa.GongFaIDArray.Values.ToList();
                }
                else
                {
                    NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
                    var rolegongfa = NHibernateQuerier.CriteriaSelect<RoleGongFa>(nHCriteriaRole);
                    if (rolegongfa != null)
                    {
                        var dict = Utility.Json.ToObject<Dictionary<int, int>>(rolegongfa.GongFaIDArray);
                        gongfaList = dict.Values.ToList();
                    }
                }

            }
            else
            {
                NHCriteria nHCriteriaRole= CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
                var rolegongfa = NHibernateQuerier.CriteriaSelect<RoleGongFa>(nHCriteriaRole);
                if (rolegongfa!=null)
                {
                   var dict= Utility.Json.ToObject<Dictionary<int,int>>(rolegongfa.GongFaIDArray);
                    gongfaList = dict.Values.ToList();
                }
            }
            #endregion

            #region 获取角色所有功法
            if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleMiShuPerfix, roleID.ToString()).Result)
            {
                var roleMiShu = RedisHelper.Hash.HashGetAsync<RoleMiShuDTO>(RedisKeyDefine._RoleMiShuPerfix, roleID.ToString()).Result;
                if (roleMiShu != null)
                {
                    foreach (var item in roleMiShu.MiShuIDArray)
                    {
                        NHCriteria nHCriteriamishu = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", item.Key);
                        var mishuObj = NHibernateQuerier.CriteriaSelect<MiShu>(nHCriteriamishu);
                        var temp = mishuDict[item.Value].Find(t => t.Mishu_Floor == mishuObj.MiShuLevel);
                        if (temp != null)
                            mishuList.Add(temp);
                    }
                }
                else
                {
                    NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
                    var rolemishu = NHibernateQuerier.CriteriaSelect<RoleMiShu>(nHCriteriaRole);
                    if (rolemishu != null)
                    {
                        var dict = Utility.Json.ToObject<Dictionary<int, int>>(rolemishu.MiShuIDArray);
                        foreach (var item in dict)
                        {
                            NHCriteria nHCriteriamishu = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", item.Key);
                            var mishuObj = NHibernateQuerier.CriteriaSelect<MiShu>(nHCriteriamishu);
                            var temp = mishuDict[item.Value].Find(t => t.Mishu_Floor == mishuObj.MiShuLevel);
                            if (temp != null)
                                mishuList.Add(temp);
                        }
                    }
                }

            }
            else
            {
                NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
                var rolemishu = NHibernateQuerier.CriteriaSelect<RoleMiShu>(nHCriteriaRole);
                if (rolemishu != null)
                {
                    var dict = Utility.Json.ToObject<Dictionary<int, int>>(rolemishu.MiShuIDArray);
                    foreach (var item in dict)
                    {
                        NHCriteria nHCriteriamishu = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", item.Key);
                        var mishuObj = NHibernateQuerier.CriteriaSelect<MiShu>(nHCriteriamishu);
                        var temp = mishuDict[item.Value].Find(t => t.Mishu_Floor == mishuObj.MiShuLevel);
                        if (temp != null)
                            mishuList.Add(temp);
                    }
                }
            }

            #endregion
        }

        public void SumRolestatus(int flyID,RoleStatusPointDTO roleStatusPoint)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, RoleStatusDatas>>(out var roleDataDict);
        }
    }
}
