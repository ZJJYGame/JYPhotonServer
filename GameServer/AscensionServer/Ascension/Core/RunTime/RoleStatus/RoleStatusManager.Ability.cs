using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using RedisDotNet;
using Cosmos;
namespace AscensionServer
{
    public partial class  RoleStatusManager
    {
        /// <summary>
        /// 获取人物加点方案
        /// </summary>
        /// <param name="pointDTO"></param>
        async void GetRolePointAbilityS2C(RoleStatusPointDTO pointDTO)
        {
            if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAbilityPointPostfix, pointDTO.RoleID.ToString()).Result)
            {
                var AbilityPoint = RedisHelper.Hash.HashGetAsync<RoleStatusPointDTO>(RedisKeyDefine._RoleAbilityPointPostfix, pointDTO.RoleID.ToString());
                if (AbilityPoint != null)
                {
                   
                }
                else
                {
                    
                }
            }

        }
        /// <summary>
        ///设置人物加点
        /// </summary>
        async void SetRolePointS2C(RoleStatusPointDTO pointDTO)
        {
            if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAbilityPointPostfix, pointDTO.RoleID.ToString()).Result)
            {
                var pointObj = RedisHelper.Hash.HashGetAsync<RoleStatusPointDTO>(RedisKeyDefine._RoleAbilityPointPostfix, pointDTO.RoleID.ToString()).Result;

                var obj = RolePointCalculate(pointObj, pointDTO);
                if (obj != null)
                {
                    RoleStatusSuccessS2C(pointDTO.RoleID, RoleStatusOpCode.Rename, obj);
                    await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleAbilityPointPostfix, pointDTO.RoleID.ToString(), obj);
                    await NHibernateQuerier.UpdateAsync(ChangeRoleStatusPointType(obj));
                }
                else
                    SetRolePointMySql(pointDTO);
            }else
                SetRolePointMySql(pointDTO);
        }
        /// <summary>
        /// 设置加点方案名称
        /// </summary>
       async  void SetRoleSlnNameS2C(RoleStatusPointDTO pointDTO)
        {
            NHCriteria nHCriteriaRoleStatue = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", pointDTO.RoleID);
            var rolePoint = NHibernateQuerier.CriteriaSelect<RoleStatusPoint>(nHCriteriaRoleStatue);
            if (rolePoint!=null)
            {
                //TODO记得补充名称合法验证
                var obj = ChangeRoleStatusPointType(rolePoint);
                var result=  obj.AbilityPointSln.TryGetValue(pointDTO.SlnNow,out var ability);
                var data = pointDTO.AbilityPointSln.TryGetValue(pointDTO.SlnNow, out var abilitydto);
                if (result&& data)
                {
                    ability.SlnName = abilitydto.SlnName;
                    obj.AbilityPointSln[pointDTO.SlnNow] = ability;
                    RoleStatusSuccessS2C(pointDTO.RoleID,RoleStatusOpCode.Rename, obj);

                    await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleAbilityPointPostfix, pointDTO.RoleID.ToString(), obj);
                    await NHibernateQuerier.UpdateAsync(ChangeRoleStatusPointType(obj));
                }
                else
                    RoleStatusFailS2C(pointDTO.RoleID, RoleStatusOpCode.SetAddPoint);
            }
            else
                RoleStatusFailS2C(pointDTO.RoleID, RoleStatusOpCode.SetAddPoint);
        }
        /// <summary>
        /// 重置加点
        /// </summary>
       async void RestartPointS2C(RoleStatusPointDTO pointDTO)
        {
            var exist =GameEntry. DataManager.TryGetValue<Dictionary<int, RoleStatusDatas>>(out var roleStatudict);
            Utility.Debug.LogInfo("YZQ加点数据进来了");
            NHCriteria nHCriteriaRoleStatue = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", pointDTO.RoleID);
            int point = 0;
            var rolePoint = NHibernateQuerier.CriteriaSelect<RoleStatusPoint>(nHCriteriaRoleStatue);
            var role = NHibernateQuerier.CriteriaSelect<Role>(nHCriteriaRoleStatue);
            if (rolePoint != null && role != null)
            {
                Utility.Debug.LogInfo("YZQ加点数据进来了1");
                //TODO记得补充名称合法验证
                var obj = ChangeRoleStatusPointType(rolePoint);
                var result = obj.AbilityPointSln.TryGetValue(pointDTO.SlnNow, out var ability);
                var data = pointDTO.AbilityPointSln.TryGetValue(pointDTO.SlnNow, out var abilitydto);
                if (result && data)
                {
                    Utility.Debug.LogInfo("YZQ加点数据进来了2");
                    if (exist)
                    {
                        for (int i = 0; i < role.RoleLevel; i++)
                        {
                            point += roleStatudict[i].FreeAttributes;
                        }

                    }
                    ability.SurplusAptitudePoint = point;
                    ability.SurplusAptitudePoint = 0;
                    ability.Agility = 0;
                    ability.Corporeity = 0;
                    ability.Power = 0;
                    ability.Soul = 0;
                    ability.Stamina = 0;
                    ability.Strength = 0;
                    ability.Agility = 0;
                    obj.AbilityPointSln[pointDTO.SlnNow] = ability;

                    RoleStatusSuccessS2C(pointDTO.RoleID, RoleStatusOpCode.Rename, obj);
                    await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleAbilityPointPostfix, pointDTO.RoleID.ToString(), obj);
                    await NHibernateQuerier.UpdateAsync(ChangeRoleStatusPointType(obj));
                }
                else
                    RoleStatusFailS2C(pointDTO.RoleID, RoleStatusOpCode.SetAddPoint);
            }
            else
            RoleStatusFailS2C(pointDTO.RoleID, RoleStatusOpCode.SetAddPoint);

        }
        
        
        #region MySql模块
       async  void SetRolePointMySql(RoleStatusPointDTO pointDTO)
        {
            NHCriteria nHCriteriaRoleStatue = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", pointDTO.RoleID);
            var rolePoint = NHibernateQuerier.CriteriaSelect<RoleStatusPoint>(nHCriteriaRoleStatue);
            if (rolePoint != null)
            {
                var obj = ChangeRoleStatusPointType(rolePoint);
                var pointObj = RolePointCalculate(obj, pointDTO);
                if (pointObj != null)
                {
                    RoleStatusSuccessS2C(pointDTO.RoleID, RoleStatusOpCode.Rename, pointObj);
                    await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleAbilityPointPostfix, pointDTO.RoleID.ToString(), pointObj);
                    await NHibernateQuerier.UpdateAsync(ChangeRoleStatusPointType(pointObj));
                }
                else
                    RoleStatusFailS2C(pointDTO.RoleID, RoleStatusOpCode.SetAddPoint);
            }else
                RoleStatusFailS2C(pointDTO.RoleID, RoleStatusOpCode.SetAddPoint);
        }
        #endregion


        /// <summary>
        /// 人物加点计算验证
        /// </summary>
        /// <param name="pointObj">数据库</param>
        /// <param name="pointDTO">客户端接收数据</param>
        /// <returns></returns>
        RoleStatusPointDTO RolePointCalculate(RoleStatusPointDTO pointObj, RoleStatusPointDTO pointDTO)
        {
            if (pointObj != null)
            {
                var result = pointObj.AbilityPointSln.TryGetValue(pointDTO.SlnNow, out var ability);
                var exist = pointDTO.AbilityPointSln.TryGetValue(pointDTO.SlnNow, out var abilityDTO);
                if (result && exist)
                {
                    var num = abilityDTO.Agility + abilityDTO.Corporeity + abilityDTO.Power + abilityDTO.Soul + abilityDTO.Stamina + abilityDTO.Strength;
                    if (ability.SurplusAptitudePoint >= num)
                    {
                        ability.SurplusAptitudePoint -= num;
                        ability.Agility += abilityDTO.Agility;
                        ability.Corporeity += abilityDTO.Corporeity;
                        ability.Power += abilityDTO.Power;
                        ability.Soul += abilityDTO.Soul;
                        ability.Stamina += abilityDTO.Stamina;
                        ability.Strength += abilityDTO.Strength;
                        pointObj.AbilityPointSln[pointDTO.SlnNow] = ability;
                        pointObj.SlnNow = pointDTO.SlnNow;
                        return pointObj;
                    }
                    else
                        return null;
                }else
                    return null;
            }
            else
                return null;
        }
        /// <summary>
        /// 数据类型转换
        /// </summary>
        /// <param name="pointDTO"></param>
        /// <returns></returns>
        RoleStatusPoint ChangeRoleStatusPointType(RoleStatusPointDTO pointDTO)
        {
            RoleStatusPoint roleStatusPoint = new RoleStatusPoint();
            roleStatusPoint.AbilityPointSln = Utility.Json.ToJson(pointDTO.AbilityPointSln);
            roleStatusPoint.RoleID = pointDTO.RoleID;
            roleStatusPoint.SlnNow = pointDTO.SlnNow;
            return roleStatusPoint;
        }
        /// <summary>
        /// 数据类型转换
        /// </summary>
        /// <param name="pointDTO"></param>
        /// <returns></returns>
        RoleStatusPointDTO ChangeRoleStatusPointType(RoleStatusPoint point)
        {
            RoleStatusPointDTO roleStatusPoint = new RoleStatusPointDTO();
            roleStatusPoint.AbilityPointSln = Utility.Json.ToObject<Dictionary<int, AbilityDTO>>(point.AbilityPointSln);
            roleStatusPoint.RoleID = point.RoleID;
            roleStatusPoint.SlnNow = point.SlnNow;
            return roleStatusPoint;
        }

    }
}
