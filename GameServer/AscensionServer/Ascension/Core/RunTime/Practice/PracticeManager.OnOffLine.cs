using System;
using System.Collections.Generic;
using System.Linq;
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
        /// 切换修炼秘书功法
        /// </summary>
        /// <param name="onOffLineDTO"></param>
        async void SwitchPracticeTypeS2C(OnOffLineDTO onOffLineDTO)
        {
            if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleOnOffLinePostfix, onOffLineDTO.RoleID.ToString()).Result)
            {
                var redisOnOffLine = await RedisHelper.Hash.HashGetAsync<OnOffLineDTO>(RedisKeyDefine._RoleOnOffLinePostfix, onOffLineDTO.RoleID.ToString());
                if (redisOnOffLine != null)
                {
                    if (onOffLineDTO.ExpType != 1 && onOffLineDTO.ExpType != 2)
                    {
                        ResultFailS2C(onOffLineDTO.RoleID, PracticeOpcode.SwitchPracticeType);
                        return;//发送返回失败
                    }
                    redisOnOffLine.ExpType = onOffLineDTO.ExpType;
                    redisOnOffLine.MsGfID = onOffLineDTO.MsGfID;
                    ResultSuccseS2C(onOffLineDTO.RoleID, PracticeOpcode.SwitchPracticeType, redisOnOffLine);
                    //发送
                }
                else
                    SwitchPracticeTypeMySql(onOffLineDTO);
            }else
                SwitchPracticeTypeMySql(onOffLineDTO);
        }
        /// <summary>
        ///计算离线经验发送给客户端
        /// </summary>
        async void GetOffLineExpS2C(int roleID)
        {
            if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleOnOffLinePostfix, roleID.ToString()).Result)
            {
                var exp = 0;
                var redisOnOffLine = await RedisHelper.Hash.HashGetAsync<OnOffLineDTO>(RedisKeyDefine._RoleOnOffLinePostfix, roleID.ToString());
                var redisRoleStatus = await RedisHelper.Hash.HashGetAsync<RoleStatus>(RedisKeyDefine._RoleStatsuPerfix, roleID.ToString());
                TimeSpan interval;
                if (redisOnOffLine!=null&&redisRoleStatus!=null)
                {
                    var redisGongfa = await RedisHelper.Hash.HashGetAsync<CultivationMethod>(RedisKeyDefine._GongfaPerfix, redisOnOffLine.MsGfID.ToString());
                    var redisMiShu = await RedisHelper.Hash.HashGetAsync<MiShu>(RedisKeyDefine._MiShuPerfix, redisOnOffLine.MsGfID.ToString());
                    interval = (DateTime.Now).Subtract(Convert.ToDateTime(redisOnOffLine.OffTime));
                    switch (redisOnOffLine.ExpType)
                    {
                        case 1:
                            exp = (int)interval.TotalSeconds / 5* redisRoleStatus.GongfaLearnSpeed;
                          var gongfaObj= AddGongFaExp(redisGongfa, exp,out CultivationMethod method);
                            ResultSuccseS2C(roleID, PracticeOpcode.GetOffLineExp, gongfaObj);
                            await RedisHelper.Hash.HashSetAsync<CultivationMethod>(RedisKeyDefine._GongfaPerfix, redisOnOffLine.MsGfID.ToString(), method);
                            break;
                        case 2:                        
                            interval = (DateTime.Now).Subtract(Convert.ToDateTime(redisOnOffLine.OffTime));
                            exp = (int)interval.TotalSeconds / 5* redisRoleStatus.MishuLearnSpeed;
                         var mishuObj =AddMiShu(redisMiShu,exp,out var miShuData);
                            ResultSuccseS2C(roleID, PracticeOpcode.GetOffLineExp, mishuObj);
                            await RedisHelper.Hash.HashSetAsync<MiShu>(RedisKeyDefine._GongfaPerfix, redisOnOffLine.MsGfID.ToString(), miShuData);
                            break;
                        default:
                            break;
                    }
                }
            }

        }
        #endregion

        #region MySql模块
        async void SwitchPracticeTypeMySql(OnOffLineDTO onOffLineDTO)
        {
            NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", onOffLineDTO.RoleID);
            var onOffLineObj= NHibernateQuerier.CriteriaSelectAsync<OnOffLine>(nHCriteriaRole).Result;
            if (onOffLineObj != null)
            {
                if (onOffLineDTO.ExpType != 1 && onOffLineDTO.ExpType != 2)
                {
                    ResultFailS2C(onOffLineDTO.RoleID, PracticeOpcode.SwitchPracticeType);
                    return;//发送返回失败
                }
                onOffLineObj.ExpType = onOffLineDTO.ExpType;
                onOffLineObj.MsGfID = onOffLineDTO.MsGfID;
                await NHibernateQuerier.UpdateAsync(onOffLineObj);
                ResultSuccseS2C(onOffLineDTO.RoleID, PracticeOpcode.SwitchPracticeType, onOffLineObj);
            }
            else
            {
                ResultFailS2C(onOffLineDTO.RoleID,PracticeOpcode.SwitchPracticeType);
            }
        }
        /// <summary>
        /// j计算离线经验
        /// </summary>
        /// <param name="roleID"></param>
        async void GetOffLineExpMySql(int roleID)
        {
            NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            var onOffLineObj = NHibernateQuerier.CriteriaSelectAsync<OnOffLine>(nHCriteriaRole).Result;
            var redisRoleStatus = NHibernateQuerier.CriteriaSelectAsync<RoleStatus>(nHCriteriaRole).Result;
            TimeSpan interval;
            if (onOffLineObj!=null&& redisRoleStatus!=null)
            {
                NHCriteria nHCriteriaid = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", onOffLineObj.MsGfID);
                var gongfa = NHibernateQuerier.CriteriaSelectAsync<CultivationMethod>(nHCriteriaid).Result;
                interval = (DateTime.Now).Subtract(Convert.ToDateTime(onOffLineObj.OffTime));

            }
        }
        
        #endregion

        /// <summary>
        /// 添加功法方法
        /// </summary>
        CultivationMethodDTO AddGongFaExp(CultivationMethod cultivation,int exp,out CultivationMethod obj)
        {
            GameEntry. DataManager.TryGetValue<Dictionary<int, RoleLevelData>>(out var roleDict);
            CultivationMethodDTO cultivationDTO = new CultivationMethodDTO();
            var result = roleDict.TryGetValue(cultivation.CultivationMethodLevel,out var roleData);
            if (result)
            {
                if (roleData.ExpLevelUp <= cultivation.CultivationMethodExp + exp)
                {
                    cultivation.CultivationMethodExp = cultivation.CultivationMethodExp + exp - roleData.ExpLevelUp;
                    cultivation.CultivationMethodLevel++;
                    //TODO添加升级的逻辑
                    AddGongFaExp(cultivation, 0,out obj);
                }
            }
            obj = cultivation;
            cultivationDTO.CultivationMethodExp = cultivation.CultivationMethodExp;
            cultivationDTO.CultivationMethodID = cultivation.CultivationMethodID;
            cultivationDTO.ID = cultivation.ID;
            cultivationDTO.CultivationMethodLevel = cultivation.CultivationMethodLevel;
            cultivationDTO.CultivationMethodLevelSkillArray =Utility.Json.ToObject<List<int>>(cultivation.CultivationMethodLevelSkillArray);
            return cultivationDTO;
        }
        /// <summary>
        /// 添加秘术
        /// </summary>
        /// <param name="miShu"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        MiShuDTO AddMiShu(MiShu miShu,int exp,out MiShu miShudata )
        {
            MiShuDTO miShuDTO = new MiShuDTO();
            GameEntry.DataManager.TryGetValue<Dictionary<int, RoleLevelData>>(out var roleDict);
            var result = roleDict.TryGetValue(miShu.MiShuLevel, out var roleData);
            if (result)
            {
                if (roleData.ExpLevelUp <= miShu.MiShuLevel + exp)
                {
                    miShu.MiShuExp = miShu.MiShuLevel + exp - roleData.ExpLevelUp;
                    miShu.MiShuLevel++;
                    //TODO添加升级的逻辑
                    AddMiShu(miShu, 0,out miShudata);
                }
            }
            miShudata = miShu;
            miShuDTO.ID = miShu.ID;
            miShuDTO.MiShuID = miShu.MiShuID;
            miShuDTO.MiShuLevel = miShu.MiShuLevel;
            miShuDTO.MiShuExp = miShu.MiShuExp;
            miShuDTO.MiShuSkillArry =Utility.Json.ToObject<List<int>>(miShu.MiShuSkillArry);  
            return miShuDTO;
        }
    }
}
