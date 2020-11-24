using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using NHibernate.Linq.Clauses;
using NHibernate.Util;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// 处理 所有的buff事件
/// </summary>
namespace AscensionServer
{
    public partial class ServerBattleManager
    {
        /// <summary>
        /// buff的入口
        /// </summary>
        /// <param name="buffId"></param>
        /// <param name="roleId"></param>
        /// <param name="playerSetObject"></param>
        /// <param name="enemySetObject"></param>
        public void BuffManagerMethod(int buffId,int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject,int og)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, BattleBuffData>>(out var buffDict);
            if (!buffDict.ContainsKey(buffId))
                return;
            ///buff  时机
            switch (buffDict[buffId].battleBuffTriggerTime)
            {
                case BattleBuffTriggerTime.BuffAdd:
                    break;
                case BattleBuffTriggerTime.RoundStart:
                    BuffConditionMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict);
                    break;
                case BattleBuffTriggerTime.RoleAttack:

                    break;
                case BattleBuffTriggerTime.RoleOnHit:
                    break;
                case BattleBuffTriggerTime.RoleBeforeDie:
                    break;
                case BattleBuffTriggerTime.RoleAfterDie:
                    break;
                case BattleBuffTriggerTime.RoundEnd:
                    var tempSelect = RandomManager(og, 0, 100);
                    if (buffDict[buffId].probability == 100 || buffDict[buffId].probability>= tempSelect)
                        BuffConditionMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict);
                    break;
                case BattleBuffTriggerTime.BuffRemove:
                    break;
            }
        }


        /// <summary>
        /// 先判断buff 触发的条件
        /// </summary>
        public void BuffConditionMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject,Dictionary<int , BattleBuffData> buffDict)
        {
            ///buff 触发判断条件
            if (buffDict[buffId].battleBuffTriggerConditionList.Count == 0)
            {
                BuffEventMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict);
                return;
            }
            ///TODO  buff 触发条件
            for (int oc = 0; oc < buffDict[buffId].battleBuffTriggerConditionList.Count; oc++)
            {
                switch (buffDict[buffId].battleBuffTriggerConditionList[oc].battleBuffConditionType)
                {
                    case BattleBuffConditionType.None:
                        break;
                    case BattleBuffConditionType.UseDesignatedSkill:
                        var userSkill = buffDict[buffId].battleBuffTriggerConditionList[oc].idList.Find(x => x == buffToSkillId);
                        if (userSkill != 0)
                        {

                        }
                        break;
                    case BattleBuffConditionType.HaveDesignatedSkill:
                        ///自己拥有的技能列表
                        break;
                    case BattleBuffConditionType.NotHaveDesignatedSkill:
                        /// /不拥有的技能列表
                        break;
                    case BattleBuffConditionType.LimitSkillTargetNum:
                        /// 未知
                        break;
                    case BattleBuffConditionType.DesignatedDamageType:
                        ///受击时  
                        break;
                    case BattleBuffConditionType.DamageCrit:
                        ///暴击
                        break;
                    case BattleBuffConditionType.DesignatedPropertyLimit:
                        break;
                    case BattleBuffConditionType.BothDesignatedPropertyCompare:
                        break;
                    case BattleBuffConditionType.TargetHaveDesignatedBuff:
                        break;
                    case BattleBuffConditionType.CharacterTypeLimit:
                        break;
                    case BattleBuffConditionType.CloseOrRangeAttack:
                        break;
                }
            }
        }

       /// <summary>
       /// 判断buff 触发事件
       /// </summary>
       /// <param name="buffId"></param>
       /// <param name="roleId"></param>
       /// <param name="currentId"></param>
       /// <param name="playerSetObject"></param>
       /// <param name="enemySetObject"></param>
       /// <param name="buffDict"></param>
        public void BuffEventMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict)
        {
            ///buff  触发事件
            var buffEventSet = buffDict[buffId].battleBuffEventDataList;
            for (int i = 0; i < buffEventSet.Count; i++)
            {
                /// 触发类型
                switch (buffEventSet[i].battleBuffEventType)
                {
                    case BattleBuffEventType.RolePropertyChange:
                        break;
                    case BattleBuffEventType.BuffPropertyChange:
                        break;
                    case BattleBuffEventType.ForbiddenBuff:
                        break;
                    case BattleBuffEventType.RoleStateChange:
                        break;
                    case BattleBuffEventType.UseDesignateSkill:
                        break;
                    case BattleBuffEventType.DamageOrHeal:
                        BuffEventDamageOrHealMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict,buffEventSet,i);
                        break;
                    case BattleBuffEventType.Shield:
                        break;
                    case BattleBuffEventType.DamageReduce:
                        break;
                    case BattleBuffEventType.TakeHurtForOther:
                        break;
                    case BattleBuffEventType.AddBuff:
                        break;
                    case BattleBuffEventType.DispelBuff:
                        break;
                    case BattleBuffEventType.NotResurgence:
                        break;
                    default:
                        break;
                }
            }
        }





        /// <summary>
        /// 伤害 或者治疗
        /// </summary>
        /// <param name="buffId"></param>
        /// <param name="roleId"></param>
        /// <param name="currentId"></param>
        /// <param name="playerSetObject"></param>
        /// <param name="enemySetObject"></param>
        /// <param name="buffDict"></param>
         
        public void BuffEventDamageOrHealMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict,List<BattleBuffEventData> buffEventSet , int i)
        {
            /// 伤害还是治疗
            switch (buffEventSet[i].flag)
            {
                case true:
                    ///伤害的类型
                    switch (buffEventSet[i].BattleSkillDamageType)
                    {
                        case BattleSkillDamageType.Physic:
                            break;
                        case BattleSkillDamageType.Magic:
                            break;
                        case BattleSkillDamageType.ShenHun:
                            break;
                        case BattleSkillDamageType.Reality:
                            break;
                    }
                    break;
                case false:
                    ///治疗数值来源
                    switch (buffEventSet[i].buffEvent_DamageOrHeal_SourceDataType)
                    {
                        case BuffEvent_DamageOrHeal_SourceDataType.MaxHealth:
                            ///来源目标 自己还是目标
                            switch (buffEventSet[i].flag_2)
                            {
                                case true:
                                    BuffEventResultsMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, buffEventSet, i);
                                    break;
                                case false:
                                    BuffEventResultsMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, buffEventSet, i);
                                    break;
                            }
                            break;
                        case BuffEvent_DamageOrHeal_SourceDataType.MaxZhenYuan:
                            break;
                        case BuffEvent_DamageOrHeal_SourceDataType.MaxShenHun:
                            break;
                        case BuffEvent_DamageOrHeal_SourceDataType.TakeDamageNum:
                            break;
                        case BuffEvent_DamageOrHeal_SourceDataType.ReceiveDamageNum:
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// buff作用于的目标具体计算
        /// </summary>
        /// <param name="buffId"></param>
        /// <param name="roleId"></param>
        /// <param name="currentId"></param>
        /// <param name="playerSetObject"></param>
        /// <param name="enemySetObject"></param>
        /// <param name="buffDict"></param>
        /// <param name="buffEventSet"></param>
        /// <param name="i"></param>
        public void BuffEventResultsMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i)
        {
            var damgeValue = (buffEventSet[i].percentValue * enemySetObject.EnemyStatusDTO.EnemyMaxHP) / 100 + buffEventSet[i].fixedValue;
            switch (buffEventSet[i].flag_3)
            {
                case true:
                    switch (buffEventSet[i].buffEvent_DamageOrHeal_EffectTargetType)
                    {
                        case BuffEvent_DamageOrHeal_EffectTargetType.Health:
                            playerSetObject.RoleStatusDTO.RoleHP += damgeValue;
                            _buffToRoomIdAfter.Add(new BattleBuffDTO() { TargetId = enemySetObject.EnemyStatusDTO.EnemyId, TriggerId = enemySetObject.EnemyStatusDTO.EnemyId, bufferId = buffId, BuffValue = damgeValue });
                            #region ob
                            /*
                            List<BattleBuffDTO> battleBuffDTOs = new List<BattleBuffDTO>();
                            battleBuffDTOs.Add(new BattleBuffDTO() { TargetId = enemySetObject.EnemyStatusDTO.EnemyId, TriggerId = roleId, bufferId = buffId, BuffValue = damgeValue });
                            var infoSet = ServerToClientResults(new TargetInfoDTO() { TargetID = enemySetObject.EnemyStatusDTO.EnemyId, TargetHPDamage = damgeValue, battleBuffDTOs = battleBuffDTOs });
                            targetInfoDTOsSet.Add(infoSet);*/
                            #endregion
                            break;
                        case BuffEvent_DamageOrHeal_EffectTargetType.ShenHun:
                            break;
                        case BuffEvent_DamageOrHeal_EffectTargetType.ZhenYuan:
                            break;
                    }
                    break;
                case false:
                  
                    break;
            }
        }

    }
}
