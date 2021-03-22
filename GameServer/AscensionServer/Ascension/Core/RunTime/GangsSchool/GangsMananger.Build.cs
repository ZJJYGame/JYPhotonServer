using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using RedisDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public partial class GangsMananger
    {
        #region Redis模块
        /// <summary>
        /// 获得宗门属性及建设
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="roleID"></param>
        async void GetAllianceConstructionS2C(int ID, int roleID)
        {
            var roleAllianceExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AllianceConstructionPerfix, ID.ToString()).Result;
            var AllianceExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlliancePerfix, ID.ToString()).Result;
            if (roleAllianceExist && AllianceExist)
            {
                var Construction = RedisHelper.Hash.HashGetAsync<AllianceConstructionDTO>(RedisKeyDefine._AllianceConstructionPerfix, ID.ToString()).Result;
                var Alliance = RedisHelper.Hash.HashGetAsync<AllianceStatusDTO>(RedisKeyDefine._AlliancePerfix, ID.ToString()).Result;
                if (Construction != null && Alliance != null)
                {
                    Dictionary<byte, object> dict = new Dictionary<byte, object>();
                    dict.Add((byte)ParameterCode.AllianceStatus, Alliance);
                    dict.Add((byte)ParameterCode.AllianceConstruction, Construction);
                    RoleStatusSuccessS2C(roleID, AllianceOpCode.GetAllianceStatus, dict);
                }
                else
                    GetRoleAliianceConstructionMySql(ID, roleID);
            }
            else
                GetRoleAliianceConstructionMySql(ID, roleID);
        }
        /// <summary>
        /// 宗门建筑升级
        /// </summary>
        async void BuildAllianceConstructionS2C(int ID, int roleID, AllianceConstructionDTO constructionDTO)
        {
            //GameEntry.DataManager.TryGetValue<Dictionary<int, >>(out var set);
            var roleAllianceExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AllianceConstructionPerfix, ID.ToString()).Result;
            var AllianceExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlliancePerfix, ID.ToString()).Result;
            if (roleAllianceExist && AllianceExist)
            {
                var Construction = RedisHelper.Hash.HashGetAsync<AllianceConstructionDTO>(RedisKeyDefine._AllianceConstructionPerfix, ID.ToString()).Result;
                var Alliance = RedisHelper.Hash.HashGetAsync<AllianceStatusDTO>(RedisKeyDefine._AlliancePerfix, ID.ToString()).Result;
                if (Construction != null && Alliance != null)
                {
                    if (constructionDTO.AllianceAlchemyStorage == 1)
                    {

                    }
                    else if (constructionDTO.AllianceArmsDrillSite == 1)
                    {
                    }
                    else if (constructionDTO.AllianceChamber == 1)
                    {
                        if (Construction.AllianceScripturesPlatform == Construction.AllianceChamber && Construction.AllianceChamber == Construction.AllianceArmsDrillSite && Construction.AllianceChamber == Construction.AllianceAlchemyStorage && Construction.AllianceAssets >= 10000)
                        {//补充宗门灵石的判断
                            Construction.AllianceChamber++;
                        }
                    }
                    else if (constructionDTO.AllianceScripturesPlatform == 1)
                    {
                    }
                }
            }
            else
            {
                //TODOMySql逻辑
            }
        }
        #endregion

        #region MySql模块
        /// <summary>
        /// 获取仙盟建筑信息
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="roleID"></param>
        void GetRoleAliianceConstructionMySql(int ID, int roleID)
        {
            NHCriteria nHCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", ID);
            NHCriteria nHCriteriaAlliance = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", ID);
            var Construction = NHibernateQuerier.CriteriaSelect<AllianceConstruction>(nHCriteria);
            var Alliance = NHibernateQuerier.CriteriaSelect<AllianceStatus>(nHCriteriaAlliance);
            Utility.Debug.LogInfo("获得数据库宗門建设数据" + Utility.Json.ToJson(Construction));
            if (Alliance != null && Alliance != null)
            {
                Dictionary<byte, object> dict = new Dictionary<byte, object>();
                dict.Add((byte)ParameterCode.AllianceStatus, Alliance);
                dict.Add((byte)ParameterCode.AllianceConstruction, Construction);
                RoleStatusSuccessS2C(roleID, AllianceOpCode.GetAllianceStatus, dict);
            }
            else
                RoleStatusFailS2C(roleID, AllianceOpCode.GetAllianceStatus);
        }
        #endregion
    }
}
