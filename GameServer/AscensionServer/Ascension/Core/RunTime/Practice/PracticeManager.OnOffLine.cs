using System;
using System.Collections.Generic;
using System.Linq;
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
                   await RedisHelper.Hash.HashSetAsync<OnOffLineDTO>(RedisKeyDefine._RoleOnOffLinePostfix, onOffLineDTO.RoleID.ToString(), redisOnOffLine);
                    UpdateOnOffLine(redisOnOffLine);
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
            Dictionary<byte, object> dict;
            var OnOffLineresult= RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleOnOffLinePostfix, roleID.ToString()).Result;
            var BottleneckResult = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleBottleneckPostfix, roleID.ToString()).Result;
            var RoleStatusResult = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatsuPerfix, roleID.ToString()).Result;
            if (!OnOffLineresult||!BottleneckResult||!RoleStatusResult)
            {
                GetOffLineExpMySql(roleID);
                return;
            }

            var exp = 0;
            var redisOnOffLine = await RedisHelper.Hash.HashGetAsync<OnOffLineDTO>(RedisKeyDefine._RoleOnOffLinePostfix, roleID.ToString());
            var redisBottleneck = await RedisHelper.Hash.HashGetAsync<BottleneckDTO>(RedisKeyDefine._RoleBottleneckPostfix, roleID.ToString());
            var redisRoleStatus = await RedisHelper.Hash.HashGetAsync<RoleStatus>(RedisKeyDefine._RoleStatsuPerfix, roleID.ToString());

            var redisGongfaexist = await RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleGongfaPerfix, roleID.ToString());
            var redisMiShuexist = await RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleMiShuPerfix, roleID.ToString());
            if (redisGongfaexist&& redisMiShuexist)
            {
                TimeSpan interval;
                if (redisOnOffLine != null && redisRoleStatus != null)
                {
                    Utility.Debug.LogInfo("YZQ获取离线经验进来了1");
                    var redisGongfa = await RedisHelper.Hash.HashGetAsync<RoleGongFaDTO>(RedisKeyDefine._RoleGongfaPerfix, roleID.ToString());
                    var redisMiShu = await RedisHelper.Hash.HashGetAsync<RoleMiShuDTO>(RedisKeyDefine._RoleMiShuPerfix, roleID.ToString());
                    interval = (DateTime.Now).Subtract(Convert.ToDateTime(redisOnOffLine.OffTime));
                    switch (redisOnOffLine.ExpType)
                    {
                        case 1:
                            if (redisBottleneck != null&& redisGongfa == null)
                            {
                                Utility.Debug.LogInfo("YZQ获取离线经验进来了2");
                                if (redisBottleneck.IsBottleneck || redisBottleneck.IsDemon || redisBottleneck.IsThunder)
                                {
                                    dict = new Dictionary<byte, object>();
                                    dict.Add((byte)PracticeOpcode.TriggerBottleneck, redisBottleneck);
                                    dict.Add((byte)PracticeOpcode.GetOffLineExp, redisOnOffLine);
                                    dict.Add((byte)PracticeOpcode.GetRoleGongfa, redisGongfa.GongFaIDDict[redisOnOffLine.MsGfID]);
                                    ResultSuccseS2C(roleID, PracticeOpcode.GetOffLineExp, dict);
                                    return;
                                }
                                else
                                {
                                    exp = (int)interval.TotalSeconds / 5 * redisRoleStatus.GongfaLearnSpeed;
                                    var bottleneckObj = AddGongFaExp(roleID, redisGongfa.GongFaIDDict[redisOnOffLine.MsGfID], exp, out CultivationMethodDTO method);
                                    redisGongfa.GongFaIDDict[method.CultivationMethodID] = method;
                                    dict = new Dictionary<byte, object>();
                                    dict.Add((byte)PracticeOpcode.TriggerBottleneck, bottleneckObj);
                                    dict.Add((byte)PracticeOpcode.GetOffLineExp, redisOnOffLine);
                                    dict.Add((byte)PracticeOpcode.GetRoleGongfa, method);
                                    ResultSuccseS2C(roleID, PracticeOpcode.GetOffLineExp, dict);

                                    #region 更新数据至数据库 redis
                                    await RedisHelper.Hash.HashSetAsync<Bottleneck>(RedisKeyDefine._RoleBottleneckPostfix, roleID.ToString(), bottleneckObj);
                                    await RedisHelper.Hash.HashSetAsync<RoleGongFaDTO>(RedisKeyDefine._RoleGongfaPerfix, roleID.ToString(), redisGongfa);
                                    await NHibernateQuerier.UpdateAsync(bottleneckObj);
                                    await NHibernateQuerier.UpdateAsync(ChangeDataType(redisGongfa));
                                    #endregion
                                }
                            }
                            else
                            {
                                GetOffLineExpMySql(roleID);
                                return;
                            }


                            break;
                        case 2:
                            if (redisMiShu == null)
                            {
                                GetOffLineExpMySql(roleID);
                                return;
                            }
                            interval = (DateTime.Now).Subtract(Convert.ToDateTime(redisOnOffLine.OffTime));
                            exp = (int)interval.TotalSeconds / 5 * redisRoleStatus.MishuLearnSpeed;
                            var mishuObj = AddMiShu(redisMiShu.MiShuIDDict[redisOnOffLine.MsGfID], exp);
                            ResultSuccseS2C(roleID, PracticeOpcode.GetOffLineExp, mishuObj);
                            redisMiShu.MiShuIDDict[redisOnOffLine.MsGfID] = mishuObj;

                            await RedisHelper.Hash.HashSetAsync<RoleMiShuDTO>(RedisKeyDefine._GongfaPerfix, redisOnOffLine.MsGfID.ToString(), redisMiShu);
                            await  NHibernateQuerier.UpdateAsync(ChangeDataType(redisMiShu));
                            break;
                        default:
                            break;
                    }
                }
                else
                    GetOffLineExpMySql(roleID);
            }
            else
                GetOffLineExpMySql(roleID);

        }
        /// <summary>
        /// 自动加经验
        /// </summary>
       async void UploadingExpS2C(OnOffLineDTO onOff)
        {
            Utility.Debug.LogInfo("YZQ自动加经验进来了");
            Dictionary<byte, object> dict;
            //升级自动更新属性返回
            var rolestatus = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatsuPerfix, onOff.RoleID.ToString()).Result;
            var role = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RolePostfix, onOff.RoleID.ToString()).Result;
            var gongfa = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleGongfaPerfix, onOff.MsGfID.ToString()).Result;
            var mishu = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatusMSPerfix, onOff.MsGfID.ToString()).Result;
            var bottleneck = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleBottleneckPostfix, onOff.RoleID.ToString()).Result;
            if (rolestatus && role&& gongfa&&bottleneck)
            {
                var rolestatusObj = RedisHelper.Hash.HashGetAsync<RoleStatusDTO>(RedisKeyDefine._RoleStatsuPerfix, onOff.RoleID.ToString()).Result;
                var gongfaObj = RedisHelper.Hash.HashGetAsync<RoleGongFaDTO>(RedisKeyDefine._RoleGongfaPerfix, onOff.RoleID.ToString()).Result;
                var mishuObj = RedisHelper.Hash.HashGetAsync<RoleMiShuDTO>(RedisKeyDefine._RoleMiShuPerfix, onOff.RoleID.ToString()).Result;
                var bottleneckObj = RedisHelper.Hash.HashGetAsync<BottleneckDTO>(RedisKeyDefine._RoleBottleneckPostfix, onOff.RoleID.ToString()).Result;
                switch (onOff.MsGfID)
                {
                    case 1://1是功法

                        if (bottleneckObj != null && rolestatusObj != null)
                        {
                            if (!bottleneckObj.IsBottleneck || !bottleneckObj.IsDemon || !bottleneckObj.IsThunder)
                            {
                                var bottleneckData=   AddGongFaExp(onOff.RoleID, gongfaObj.GongFaIDDict[onOff.MsGfID], rolestatusObj.GongfaLearnSpeed, out CultivationMethodDTO methodDTO);

                                gongfaObj.GongFaIDDict[methodDTO.CultivationMethodID] = methodDTO;

                                dict = new Dictionary<byte, object>();
                                dict.Add((byte)PracticeOpcode.TriggerBottleneck, bottleneckData);
                                dict.Add((byte)PracticeOpcode.GetRoleGongfa, methodDTO);
                                ResultSuccseS2C(onOff.RoleID, PracticeOpcode.UploadingExp, dict);

                               await  NHibernateQuerier.UpdateAsync(ChangeDataType(gongfaObj));
                                await NHibernateQuerier.UpdateAsync(bottleneckData);

                                await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleBottleneckPostfix, onOff.RoleID.ToString(), bottleneckData);
                                await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleGongfaPerfix, onOff.RoleID.ToString(), gongfaObj);
                            }
                            else
                                ResultFailS2C(onOff.RoleID, PracticeOpcode.UploadingExp);
                        }
                        else
                            UpLoadingExpMySql(onOff);//Redis數據獲取失敗
                        break;
                    case 2://2是秘術
                        if (mishuObj == null)
                        {
                            ResultFailS2C(onOff.MsGfID, PracticeOpcode.GetOffLineExp);
                            return;
                        }
                        var mishuTemp = AddMiShu(mishuObj.MiShuIDDict[onOff.MsGfID], rolestatusObj.MishuLearnSpeed);
                        dict = new Dictionary<byte, object>();
                        dict.Add((byte)PracticeOpcode.GetOffLineExp, mishuObj);
                        ResultSuccseS2C(onOff.RoleID, PracticeOpcode.GetRoleMiShu, dict);
                        mishuObj.MiShuIDDict[onOff.MsGfID] = mishuTemp;

                        await NHibernateQuerier.UpdateAsync(ChangeDataType(mishuObj));
                        await RedisHelper.Hash.HashSetAsync<RoleMiShuDTO>(RedisKeyDefine._MiShuPerfix, onOff.MsGfID.ToString(), mishuObj);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //TODO切换MySql查找
                UpLoadingExpMySql(onOff);//Redis數據獲取失敗
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
                await RedisHelper.Hash.HashSetAsync<OnOffLine>(RedisKeyDefine._RoleOnOffLinePostfix, onOffLineDTO.RoleID.ToString(), onOffLineObj);

                ResultSuccseS2C(onOffLineDTO.RoleID, PracticeOpcode.SwitchPracticeType, onOffLineObj);
            }
            else
            {
                ResultFailS2C(onOffLineDTO.RoleID,PracticeOpcode.SwitchPracticeType);
            }
        }
        /// <summary>
        /// 计算离线经验
        /// </summary>
        /// <param name="roleID"></param>
        async void GetOffLineExpMySql(int roleID)
        {
            Utility.Debug.LogInfo("YZQ获取离线经验进来了3");
            var exp = 0;
            Dictionary<byte, object> dict;
            NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            var onOffLineObj = NHibernateQuerier.CriteriaSelectAsync<OnOffLine>(nHCriteriaRole).Result;

            var bottleneck= NHibernateQuerier.CriteriaSelectAsync<Bottleneck>(nHCriteriaRole).Result;

            var redisRoleStatus = NHibernateQuerier.CriteriaSelectAsync<RoleStatus>(nHCriteriaRole).Result;
            TimeSpan interval;
            if (onOffLineObj!=null&& redisRoleStatus!=null)
            {
                Utility.Debug.LogInfo("YZQ得到的功法ID为" + onOffLineObj.MsGfID);
                NHCriteria nHCriteriaid = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
                var gongfa = NHibernateQuerier.CriteriaSelectAsync<RoleGongFa>(nHCriteriaid).Result;
                var mishu = NHibernateQuerier.CriteriaSelectAsync<RoleMiShu >(nHCriteriaid).Result;
                interval = (DateTime.Now).Subtract(Convert.ToDateTime(onOffLineObj.OffTime));
                switch (onOffLineObj.ExpType)
                {
                    case 1:
                        if (bottleneck != null&& gongfa == null)
                        {
                            var rolegongfaObj = ChangeDataType(gongfa);

                            Utility.Debug.LogInfo("YZQ派发功法经验");
                            if (bottleneck.IsBottleneck || bottleneck.IsDemon || bottleneck.IsThunder)
                            {
                                Utility.Debug.LogInfo("YZQ派发功法经验2");
                                dict = new Dictionary<byte, object>();
                                dict.Add((byte)PracticeOpcode.TriggerBottleneck, bottleneck);
                                dict.Add((byte)PracticeOpcode.GetOffLineExp, onOffLineObj);
                                dict.Add((byte)PracticeOpcode.GetRoleGongfa, rolegongfaObj.GongFaIDDict[onOffLineObj.MsGfID]);
                                ResultSuccseS2C(roleID, PracticeOpcode.GetOffLineExp, dict);
                                return;
                            }else
                            {
                                exp = (int)interval.TotalSeconds / 5 * redisRoleStatus.GongfaLearnSpeed;
                                Utility.Debug.LogInfo("YZQonoffLine計算的功法经验為" + exp);
                                var bottleneckObj = AddGongFaExp(roleID, rolegongfaObj.GongFaIDDict[onOffLineObj.MsGfID], exp, out CultivationMethodDTO method);
                                rolegongfaObj.GongFaIDDict[method.CultivationMethodID] = method;

                                dict = new Dictionary<byte, object>();
                                dict.Add((byte)PracticeOpcode.TriggerBottleneck, bottleneckObj);
                                dict.Add((byte)PracticeOpcode.GetOffLineExp, onOffLineObj);
                                dict.Add((byte)PracticeOpcode.GetRoleGongfa, method);
                                //TODO添加onoffline到字典中
                                ResultSuccseS2C(roleID, PracticeOpcode.GetOffLineExp, dict);

                                Utility.Debug.LogInfo("YZQ派发功法经验" + Utility.Json.ToJson(method));
                                #region 更新数据至数据库 redis
                                await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleBottleneckPostfix, roleID.ToString(), bottleneckObj);
                                await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._GongfaPerfix, roleID.ToString(), rolegongfaObj);
                                await NHibernateQuerier.UpdateAsync(bottleneckObj);
                                await NHibernateQuerier.UpdateAsync(ChangeDataType(rolegongfaObj));
                                #endregion
                            }
                        }
                        else
                            ResultFailS2C(roleID, PracticeOpcode.GetOffLineExp);
                        break;
                    case 2:
                        if (mishu == null)
                        {
                            ResultFailS2C(roleID, PracticeOpcode.GetOffLineExp);
                            return;
                        }

                        var mishuTemp = ChangeDataType(mishu);

                        exp = (int)interval.TotalSeconds / 5 * redisRoleStatus.GongfaLearnSpeed;
                        var mishuObj = AddMiShu(mishuTemp.MiShuIDDict[onOffLineObj.MsGfID], exp);
                        dict = new Dictionary<byte, object>();
                        dict.Add((byte)PracticeOpcode.GetOffLineExp, mishuObj);
                        ResultSuccseS2C(roleID, PracticeOpcode.UploadingExp, dict);
                        mishuTemp.MiShuIDDict[onOffLineObj.MsGfID] = mishuObj;

                        await NHibernateQuerier.UpdateAsync(ChangeDataType(mishuTemp));
                        await RedisHelper.Hash.HashSetAsync<RoleMiShuDTO>(RedisKeyDefine._MiShuPerfix, onOffLineObj.MsGfID.ToString(), mishuTemp);
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// 挂機經驗的結算
        /// </summary>
        /// <param name="onOffLine"></param>
        async  void UpLoadingExpMySql(OnOffLineDTO onOffLine)
        {
            Utility.Debug.LogInfo("YZQ自动加经验MYSQL进来了"+Utility.Json.ToJson(onOffLine));
            Utility.Debug.LogInfo("YZQ自动加经验MYSQL进来了" + onOffLine.RoleID);
            Dictionary<byte, object>dict;
            NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", onOffLine.RoleID);
            NHCriteria nHCriteriagf = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", onOffLine.MsGfID);
            var roleStatusObj = NHibernateQuerier.CriteriaSelectAsync<RoleStatus>(nHCriteriaRole).Result;
            var rolegongfaObj = NHibernateQuerier.CriteriaSelectAsync<RoleGongFa>(nHCriteriaRole).Result;
            var bottleneckObj = NHibernateQuerier.CriteriaSelectAsync<Bottleneck>(nHCriteriaRole).Result;
            var mishu = NHibernateQuerier.CriteriaSelectAsync<RoleMiShu>(nHCriteriaRole).Result;
            if (roleStatusObj != null && rolegongfaObj != null && bottleneckObj != null&& mishu!=null)
            {
                Utility.Debug.LogInfo("YZQ自动加经验MYSQL进来了1" + (bottleneckObj.IsBottleneck) + ">>" + (bottleneckObj.IsDemon) + ">>" + (bottleneckObj.IsThunder));
                var rolegongfa = ChangeDataType(rolegongfaObj);
                switch (onOffLine.ExpType)
                {
                    case 1:
                        if (!bottleneckObj.IsBottleneck || !bottleneckObj.IsDemon || !bottleneckObj.IsThunder)
                        {
                            Utility.Debug.LogInfo("YZQonoffLineMYSQL自动加经验的數值為" + roleStatusObj.GongfaLearnSpeed);
                            var bottleneckData = AddGongFaExp(onOffLine.RoleID, rolegongfa.GongFaIDDict[onOffLine.MsGfID], roleStatusObj.GongfaLearnSpeed, out var methodDTO);
                            rolegongfa.GongFaIDDict[onOffLine.MsGfID] = methodDTO;
                            dict = new Dictionary<byte, object>();
                            dict.Add((byte)PracticeOpcode.TriggerBottleneck, bottleneckData);
                            dict.Add((byte)PracticeOpcode.GetRoleGongfa, methodDTO);
                            ResultSuccseS2C(onOffLine.RoleID, PracticeOpcode.UploadingExp, dict);

                            await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleGongfaPerfix, onOffLine.RoleID.ToString(), rolegongfa);
                            await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleBottleneckPostfix, onOffLine.RoleID.ToString(), bottleneckData);

                            await NHibernateQuerier.UpdateAsync(ChangeDataType(rolegongfa));
                            await NHibernateQuerier.UpdateAsync(bottleneckData);

                        }
                        else
                        {
                          //var bottleneck=  AddGongFaExp(onOffLine.RoleID, cultivationObj, roleStatusObj.GongfaLearnSpeed,out var methodDTO);
                            dict = new Dictionary<byte, object>();
                            dict.Add((byte)PracticeOpcode.TriggerBottleneck, bottleneckObj);
                            dict.Add((byte)PracticeOpcode.GetRoleGongfa, rolegongfa.GongFaIDDict[onOffLine.MsGfID]);
                            //TODO添加onoffline到字典中
                            ResultSuccseS2C(onOffLine.RoleID, PracticeOpcode.UploadingExp, dict);
                        }
                        break;
                    case 2:
                        if (mishu == null)
                        {
                            ResultFailS2C(onOffLine.MsGfID, PracticeOpcode.GetOffLineExp);
                            return;
                        }

                        var mishuTemp = ChangeDataType(mishu);
                        var mishuObj = AddMiShu(mishuTemp.MiShuIDDict[onOffLine.MsGfID], roleStatusObj.MishuLearnSpeed);
                        dict = new Dictionary<byte, object>();
                        dict.Add((byte)PracticeOpcode.GetRoleMiShu, mishuObj);
                        ResultSuccseS2C(onOffLine.RoleID, PracticeOpcode.UploadingExp, dict);
                        mishuTemp.MiShuIDDict[onOffLine.MsGfID] = mishuObj;

                        await NHibernateQuerier.UpdateAsync(ChangeDataType(mishuTemp));
                        await RedisHelper.Hash.HashSetAsync<RoleMiShuDTO>(RedisKeyDefine._MiShuPerfix, onOffLine.MsGfID.ToString(), mishuTemp);
                        break;
                    default:
                        break;
                }
            }
            else
                ResultFailS2C(onOffLine.RoleID,PracticeOpcode.UploadingExp);
        }
        #endregion

        /// <summary>
        /// 添加功法经验方法
        /// </summary>
        Bottleneck AddGongFaExp(int roleID,CultivationMethodDTO cultivation,int exp,out CultivationMethodDTO obj)
        {
            Bottleneck bottleneck = new Bottleneck() ;
            bool isbottleneck;
            GameEntry. DataManager.TryGetValue<Dictionary<int, RoleLevelData>>(out var roleDict);
            CultivationMethodDTO cultivationDTO = new CultivationMethodDTO();
            var result = roleDict.TryGetValue(cultivation.CultivationMethodLevel,out var roleData);
            if (result)
            {
                if (roleData.ExpLevelUp <= cultivation.CultivationMethodExp + exp)
                {
                    cultivation.CultivationMethodExp = cultivation.CultivationMethodExp + exp - roleData.ExpLevelUp;
                    cultivation.CultivationMethodLevel++;
                     bottleneck = TriggerBottleneckS2C(roleID, cultivation.CultivationMethodLevel, out  isbottleneck);
                    if (bottleneck != null)
                    {
                        if (!isbottleneck)
                        {
                            cultivationDTO.CultivationMethodExp = cultivation.CultivationMethodExp;
                            cultivationDTO.CultivationMethodID = cultivation.CultivationMethodID;
                            cultivationDTO.CultivationMethodLevel = cultivation.CultivationMethodLevel;
                            cultivationDTO.CultivationMethodLevelSkillArray = cultivation.CultivationMethodLevelSkillArray;
                            obj = cultivationDTO;
                            return bottleneck;
                        }
                    }
                    AddGongFaExp(roleID,cultivation, 0,out obj);
                }
            }
             bottleneck = TriggerBottleneckS2C(roleID, cultivation.CultivationMethodLevel, out  isbottleneck);
            cultivationDTO.CultivationMethodExp = cultivation.CultivationMethodExp+exp;
            cultivationDTO.CultivationMethodID = cultivation.CultivationMethodID;
            cultivationDTO.CultivationMethodLevel = cultivation.CultivationMethodLevel;
            cultivationDTO.CultivationMethodLevelSkillArray = cultivation.CultivationMethodLevelSkillArray;
            obj = cultivationDTO;
            return bottleneck;
        }
        /// <summary>
        /// 添加秘术
        /// </summary>
        /// <param name="miShu"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        MiShuDTO AddMiShu(MiShuDTO miShu,int exp )
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
                    //TODO添加升级的逻辑,先判断瓶颈在升级
                    
                    AddMiShu(miShu, 0);
                }
            }
            miShuDTO.MiShuID = miShu.MiShuID;
            miShuDTO.MiShuLevel = miShu.MiShuLevel;
            miShuDTO.MiShuExp = miShu.MiShuExp;
            miShuDTO.MiShuSkillArry = miShu.MiShuSkillArry;  
            return miShuDTO;
        }

        /// <summary>
        /// 更新数据至数据库
        /// </summary>
        /// <param name="onOffLineDTO"></param>
        async void UpdateOnOffLine(OnOffLineDTO onOffLineDTO)
        {
            OnOffLine onOffLine = new OnOffLine();
            onOffLine.ExpType = onOffLineDTO.ExpType;
            onOffLine.MsGfID = onOffLineDTO.MsGfID;
            onOffLine.OffTime = onOffLineDTO.OffTime;
            onOffLine.RoleID = onOffLineDTO.RoleID;
            await NHibernateQuerier.UpdateAsync(onOffLine);
        }
        /// <summary>
        /// 更新离线经验相关的数据
        /// </summary>

        CultivationMethod ChangeDataType(CultivationMethodDTO methodDTO)
        {
            CultivationMethod cultivation = new CultivationMethod();
            cultivation.CultivationMethodExp = methodDTO.CultivationMethodExp;
            cultivation.CultivationMethodID = methodDTO.CultivationMethodID;
            cultivation.CultivationMethodLevel = methodDTO.CultivationMethodLevel;
            cultivation.CultivationMethodLevelSkillArray =Utility.Json.ToJson(methodDTO.CultivationMethodLevelSkillArray);
            return cultivation;
        }
    }
}
