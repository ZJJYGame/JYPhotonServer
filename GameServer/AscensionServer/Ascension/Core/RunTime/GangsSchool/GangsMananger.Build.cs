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
            GameEntry.DataManager.TryGetValue<Dictionary<byte, AllianceConstructionData>>(out var construction);
            var roleAllianceExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AllianceConstructionPerfix, ID.ToString()).Result;
            var allianceExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlliancePerfix, ID.ToString()).Result;
            if (roleAllianceExist&& allianceExist)
            {
                Utility.Debug.LogInfo("YZQ升级宗门建设成员2"+Utility.Json.ToJson(construction));
                var Construction = RedisHelper.Hash.HashGetAsync<AllianceConstructionDTO>(RedisKeyDefine._AllianceConstructionPerfix, ID.ToString()).Result;
                var alliance = RedisHelper.Hash.HashGetAsync<AllianceStatusDTO>(RedisKeyDefine._AlliancePerfix, ID.ToString()).Result;
                if (Construction != null&& alliance!=null)
                {
                    if (constructionDTO.AllianceAlchemyStorage == 1)
                    {
                        var assets = construction[3].AllianceBuildingData.Find((x) => x.BuildingLevel == Construction.AllianceAlchemyStorage);
                        if (Construction.AllianceAssets>= assets.NeedAllianceSpiritStones)
                        {
                            Construction.AllianceAssets -= assets.NeedAllianceSpiritStones;
                            Construction.AllianceAlchemyStorage++;
                        }
                    }
                    else if (constructionDTO.AllianceArmsDrillSite == 1)
                    {
                        var assets = construction[2].AllianceBuildingData.Find((x) => x.BuildingLevel == Construction.AllianceArmsDrillSite);
                        if (Construction.AllianceAssets >= assets.NeedAllianceSpiritStones)
                        {
                            Construction.AllianceAssets -= assets.NeedAllianceSpiritStones;
                            Construction.AllianceArmsDrillSite++;
                        }
                    }
                    else if (constructionDTO.AllianceChamber == 1)
                    {

                        if (Construction.AllianceScripturesPlatform == Construction.AllianceChamber && Construction.AllianceChamber == Construction.AllianceArmsDrillSite && Construction.AllianceChamber == Construction.AllianceAlchemyStorage)
                        {
                            var assets = construction[1].AllianceBuildingData.Find((x) => x.BuildingLevel == Construction.AllianceChamber);
                            if (Construction.AllianceAssets >= assets.NeedAllianceSpiritStones)
                            {
                                Construction.AllianceAssets -= assets.NeedAllianceSpiritStones;
                                Construction.AllianceChamber++;
                                alliance.AllianceLevel = Construction.AllianceChamber;
                            }
                        }
                    }
                    else if (constructionDTO.AllianceScripturesPlatform == 1)
                    {
                        var assets = construction[4].AllianceBuildingData.Find((x) => x.BuildingLevel == Construction.AllianceScripturesPlatform);
                        if (Construction.AllianceAssets >= assets.NeedAllianceSpiritStones)
                        {
                            Construction.AllianceAssets -= assets.NeedAllianceSpiritStones;
                            Construction.AllianceScripturesPlatform++;
                        }
                    }
                    Dictionary<byte, object> dict = new Dictionary<byte, object>();
                    dict.Add((byte)ParameterCode.AllianceStatus, alliance);
                    dict.Add((byte)ParameterCode.AllianceConstruction, Construction);
                    RoleStatusSuccessS2C(roleID, AllianceOpCode.BuildAlliance, dict);
                    await  RedisHelper.Hash.HashSetAsync<AllianceConstructionDTO>(RedisKeyDefine._AllianceConstructionPerfix,ID.ToString(),Construction);
                    await NHibernateQuerier.UpdateAsync(Construction);
                } else
                    BuildAllianceConstructionMySql(ID, roleID, constructionDTO);
            }
            else
                BuildAllianceConstructionMySql(ID, roleID, constructionDTO);
        }
        /// <summary>
        /// 升级宗门技能等级
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="skillDTO"></param>
        async void UpdateAllianceSkillS2C(int roleID,RoleAllianceSkillDTO skillDTO)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<string, AllianceSkillsData>>(out var SkillDict);
            var skillexit = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAllianceSkillPerfix,roleID.ToString()).Result;
            var assetsexit = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAssetsPerfix, roleID.ToString()).Result;
            if (skillexit && assetsexit)
            {
                var skillObj = RedisHelper.Hash.HashGetAsync<RoleAllianceSkill>(RedisKeyDefine._RoleAllianceSkillPerfix, roleID.ToString()).Result;
                var assetsObj = RedisHelper.Hash.HashGetAsync<RoleAssets>(RedisKeyDefine._RoleAssetsPerfix, roleID.ToString()).Result;
                if (skillObj != null && assetsObj != null)
                {
                    if (skillDTO.SkillInsight == 1)
                    {
                        var obj = SkillDict["Insight"].AllianceSkillData.Find((x) => x.SkillLevel == skillObj.SkillInsight);
                        if (obj.SpiritStones <= assetsObj.SpiritStonesLow && skillObj.SkillInsight < 150)
                        {
                            assetsObj.SpiritStonesLow -= obj.SpiritStones;
                            skillObj.SkillInsight++;
                        }
                        else
                        {
                            RoleStatusFailS2C(roleID, AllianceOpCode.UpdateAllianceSkill);
                            return;
                        }
                    }
                    else if (skillDTO.SkillMeditation == 1)
                    {
                        var obj = SkillDict["Meditation"].AllianceSkillData.Find((x) => x.SkillLevel == skillObj.SkillMeditation);
                        if (obj.SpiritStones <= assetsObj.SpiritStonesLow && skillObj.SkillMeditation < 150)
                        {
                            assetsObj.SpiritStonesLow -= obj.SpiritStones;
                            skillObj.SkillMeditation++;
                        }
                        else
                        {
                            RoleStatusFailS2C(roleID, AllianceOpCode.UpdateAllianceSkill);
                            return;
                        }
                    }
                    else if (skillDTO.SkillRapid == 1)
                    {
                        var obj = SkillDict["Rapid"].AllianceSkillData.Find((x) => x.SkillLevel == skillObj.SkillRapid);
                        if (obj.SpiritStones <= assetsObj.SpiritStonesLow && skillObj.SkillRapid < 150)
                        {
                            assetsObj.SpiritStonesLow -= obj.SpiritStones;
                            skillObj.SkillRapid++;
                        }
                        else
                        {
                            RoleStatusFailS2C(roleID, AllianceOpCode.UpdateAllianceSkill);
                            return;
                        }
                    }
                    else if (skillDTO.SkillStrong == 1)
                    {
                        var obj = SkillDict["StrongBody"].AllianceSkillData.Find((x) => x.SkillLevel == skillObj.SkillStrong);
                        if (obj.SpiritStones <= assetsObj.SpiritStonesLow && skillObj.SkillStrong < 150)
                        {
                            assetsObj.SpiritStonesLow -= obj.SpiritStones;
                            skillObj.SkillStrong++;
                        }
                        else
                        {
                            RoleStatusFailS2C(roleID, AllianceOpCode.UpdateAllianceSkill);
                            return;
                        }
                    }
                    RedisHelper.Hash.HashSet<RoleAllianceSkill>(RedisKeyDefine._RoleAllianceSkillPerfix, roleID.ToString(), skillObj);
                    RedisHelper.Hash.HashSet<RoleAssets>(RedisKeyDefine._RoleAllianceSkillPerfix, roleID.ToString(), assetsObj);
                    await NHibernateQuerier.UpdateAsync(skillObj);
                    await NHibernateQuerier.UpdateAsync(assetsObj);

                    Dictionary<byte, object> dict = new Dictionary<byte, object>();
                    dict.Add((byte)ParameterCode.RoleAssets, assetsObj);
                    dict.Add((byte)ParameterCode.RoleAllianceSkill, skillObj);
                    RoleStatusSuccessS2C(roleID, AllianceOpCode.UpdateAllianceSkill, dict);
                }
                else
                {
                    UpdateAllianceSkillMySql(roleID, skillDTO);
                }
            }
            else
            {
                UpdateAllianceSkillMySql(roleID, skillDTO);
            }

        }
        /// <summary>
        /// 获得宗门技能
        /// </summary>
         void GetAllianceSkillS2C(int roleID)
        {
            var skillExit = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAllianceSkillPerfix,roleID.ToString()).Result;
            if (skillExit)
            {
                var skillObj = RedisHelper.Hash.HashGetAsync<RoleAllianceSkillDTO>(RedisKeyDefine._RoleAllianceSkillPerfix, roleID.ToString()).Result;
                if (skillObj != null)
                {
                    Utility.Debug.LogInfo("获得角色宗門技能" + roleID);
                    RoleStatusSuccessS2C(roleID, AllianceOpCode.GetAllianceSkill, skillObj);
                }
                else
                    GetAllianceSkillMySql(roleID);
            }
            else
                GetAllianceSkillMySql(roleID);
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

        async void BuildAllianceConstructionMySql(int ID, int roleID, AllianceConstructionDTO constructionDTO)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<byte, AllianceConstructionData>>(out var construction);
            NHCriteria nHCriteriaAlliance = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", ID);
            NHCriteria nHCriteriaAlliancestatus = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", ID);
            var Construction = NHibernateQuerier.CriteriaSelect<AllianceConstruction>(nHCriteriaAlliance);
            var alliance = NHibernateQuerier.CriteriaSelect<AllianceStatus>(nHCriteriaAlliancestatus);
            if (Construction != null&& alliance!=null)
            {
                Utility.Debug.LogInfo("YZQ升级宗门建设成员3");
                if (constructionDTO.AllianceAlchemyStorage == 1)
                {
                    var assets = construction[3].AllianceBuildingData.Find((x) => x.BuildingLevel == Construction.AllianceAlchemyStorage);
                    if (Construction.AllianceAssets >= assets.NeedAllianceSpiritStones)
                    {
                        Construction.AllianceAssets -= assets.NeedAllianceSpiritStones;
                        Construction.AllianceAlchemyStorage++;
                    }
                }
                else if (constructionDTO.AllianceArmsDrillSite == 1)
                {
                    var assets = construction[2].AllianceBuildingData.Find((x) => x.BuildingLevel == Construction.AllianceArmsDrillSite);
                    if (Construction.AllianceAssets >= assets.NeedAllianceSpiritStones)
                    {
                        Construction.AllianceAssets -= assets.NeedAllianceSpiritStones;
                        Construction.AllianceArmsDrillSite++;
                    }
                }
                else if (constructionDTO.AllianceChamber == 1)
                {

                    if (Construction.AllianceScripturesPlatform == Construction.AllianceChamber && Construction.AllianceChamber == Construction.AllianceArmsDrillSite && Construction.AllianceChamber == Construction.AllianceAlchemyStorage)
                    {
                        var assets = construction[1].AllianceBuildingData.Find((x) => x.BuildingLevel == Construction.AllianceChamber);
                        if (Construction.AllianceAssets >= assets.NeedAllianceSpiritStones)
                        {
                            Construction.AllianceAssets -= assets.NeedAllianceSpiritStones;
                            Construction.AllianceChamber++;
                            alliance.AllianceLevel = Construction.AllianceChamber;
                        }
                    }
                }
                else if (constructionDTO.AllianceScripturesPlatform == 1)
                {
                    var assets = construction[4].AllianceBuildingData.Find((x) => x.BuildingLevel == Construction.AllianceScripturesPlatform);
                    if (Construction.AllianceAssets >= assets.NeedAllianceSpiritStones)
                    {
                        Construction.AllianceAssets -= assets.NeedAllianceSpiritStones;
                        Construction.AllianceScripturesPlatform++;
                    }
                }

                Dictionary<byte, object> dict = new Dictionary<byte, object>();
                dict.Add((byte)ParameterCode.AllianceStatus, alliance);
                dict.Add((byte)ParameterCode.AllianceConstruction, Construction);
                RoleStatusSuccessS2C(roleID, AllianceOpCode.BuildAlliance, dict);
            }
            else
                RoleStatusFailS2C(roleID,AllianceOpCode.BuildAlliance);
        }
        /// <summary>
        /// 获得角色宗门技能
        /// </summary>
        /// <param name="roleID"></param>
        void GetAllianceSkillMySql(int roleID)
        {
            Utility.Debug.LogInfo("获得角色宗門技能" + roleID);
            NHCriteria nHCriteriarole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            var skillObj = NHibernateQuerier.CriteriaSelect<RoleAllianceSkill>(nHCriteriarole);
            Utility.Debug.LogInfo("获得角色宗門技能" + Utility.Json.ToJson(skillObj));
            if (skillObj != null)
            {
                RoleStatusSuccessS2C(roleID, AllianceOpCode.GetAllianceSkill, skillObj);
            }
            else
                RoleStatusFailS2C(roleID, AllianceOpCode.GetAllianceSkill);
        }

        async void UpdateAllianceSkillMySql(int roleID, RoleAllianceSkillDTO skillDTO)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<string, AllianceSkillsData>>(out var SkillDict);
            Utility.Debug.LogInfo("角色宗門技能升级2"+Utility.Json.ToJson(SkillDict));
            NHCriteria nHCriteriarole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            var skillObj = NHibernateQuerier.CriteriaSelect<RoleAllianceSkill>(nHCriteriarole);
            var assetsObj = NHibernateQuerier.CriteriaSelect<RoleAssets>(nHCriteriarole);
            if (skillObj != null && assetsObj != null)
            {
                if (skillDTO.SkillInsight == 1)
                {
                    var obj = SkillDict["Insight"].AllianceSkillData.Find((x) => x.SkillLevel == skillObj.SkillInsight);
                    if (obj.SpiritStones <= assetsObj.SpiritStonesLow && skillObj.SkillInsight < 150)
                    {
                        assetsObj.SpiritStonesLow -= obj.SpiritStones;
                        skillObj.SkillInsight++;
                    }
                    else
                    {
                        RoleStatusFailS2C(roleID, AllianceOpCode.UpdateAllianceSkill);
                        return;
                    }
                }
                else if (skillDTO.SkillMeditation == 1)
                {
                    var obj = SkillDict["Meditation"].AllianceSkillData.Find((x) => x.SkillLevel == skillObj.SkillMeditation);
                    if (obj.SpiritStones <= assetsObj.SpiritStonesLow && skillObj.SkillMeditation < 150)
                    {
                        assetsObj.SpiritStonesLow -= obj.SpiritStones;
                        skillObj.SkillMeditation++;
                    }
                    else
                    {
                        RoleStatusFailS2C(roleID, AllianceOpCode.UpdateAllianceSkill);
                        return;
                    }
                }
                else if (skillDTO.SkillRapid == 1)
                {
                    var obj = SkillDict["Rapid"].AllianceSkillData.Find((x) => x.SkillLevel == skillObj.SkillRapid);
                    if (obj.SpiritStones <= assetsObj.SpiritStonesLow && skillObj.SkillRapid < 150)
                    {
                        assetsObj.SpiritStonesLow -= obj.SpiritStones;
                        skillObj.SkillRapid++;
                    }
                    else
                    {
                        RoleStatusFailS2C(roleID, AllianceOpCode.UpdateAllianceSkill);
                        return;
                    }
                }
                else if (skillDTO.SkillStrong == 1)
                {
                    var obj = SkillDict["StrongBody"].AllianceSkillData.Find((x) => x.SkillLevel == skillObj.SkillStrong);
                    if (obj.SpiritStones <= assetsObj.SpiritStonesLow && skillObj.SkillStrong < 150)
                    {
                        assetsObj.SpiritStonesLow -= obj.SpiritStones;
                        skillObj.SkillStrong++;
                    }
                    else
                    {
                        RoleStatusFailS2C(roleID, AllianceOpCode.UpdateAllianceSkill);
                        return;
                    }
                }
                Utility.Debug.LogInfo("角色宗門技能升级2");
                RedisHelper.Hash.HashSet<RoleAllianceSkill>(RedisKeyDefine._RoleAllianceSkillPerfix, roleID.ToString(), skillObj);
                RedisHelper.Hash.HashSet<RoleAssets>(RedisKeyDefine._RoleAllianceSkillPerfix, roleID.ToString(), assetsObj);
                await NHibernateQuerier.UpdateAsync(skillObj);
                await NHibernateQuerier.UpdateAsync(assetsObj);

                Dictionary<byte, object> dict = new Dictionary<byte, object>();
                dict.Add((byte)ParameterCode.RoleAssets, assetsObj);
                dict.Add((byte)ParameterCode.RoleAllianceSkill, skillObj);
                RoleStatusSuccessS2C(roleID,AllianceOpCode.UpdateAllianceSkill,dict);
            }
            else
            {
                RoleStatusFailS2C(roleID, AllianceOpCode.UpdateAllianceSkill);
            }
        }
        #endregion
    }
}
