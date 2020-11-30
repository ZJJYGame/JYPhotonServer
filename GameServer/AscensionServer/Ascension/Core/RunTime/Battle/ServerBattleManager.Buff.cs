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
        public void BuffManagerMethod(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, int og, bool isSelf)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, BattleBuffData>>(out var buffDict);
            if (!buffDict.ContainsKey(buffId))
                return;
            var tempSelect = RandomManager(og, 0, 100);
            if (buffDict[buffId].probability == 100 || buffDict[buffId].probability >= tempSelect)
            {
                ///buff  时机
                switch (buffDict[buffId].battleBuffTriggerTime)
                {
                    case BattleBuffTriggerTime.BuffAdd:
                        BuffConditionMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, isSelf);
                        break;
                    case BattleBuffTriggerTime.RoundStart:
                        BuffConditionMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, isSelf);
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
                        BuffConditionMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, isSelf);
                        break;
                    case BattleBuffTriggerTime.BuffRemove:
                        break;
                }
            }
        }


        /// <summary>
        /// 先判断buff 触发的条件
        /// </summary>
        public void BuffConditionMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, bool isSelf)
        {
            ///buff 触发判断条件
            if (buffDict[buffId].battleBuffTriggerConditionList.Count == 0)
            {
                BuffEventMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, isSelf);
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
        public void BuffEventMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, bool isSelf)
        {
            ///buff  触发事件
            var buffEventSet = buffDict[buffId].battleBuffEventDataList;
            for (int i = 0; i < buffEventSet.Count; i++)
            {
                /// 触发类型
                switch (buffEventSet[i].battleBuffEventType)
                {
                    case BattleBuffEventType.RolePropertyChange:
                        BuffEventRolePropertyChangeMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, buffEventSet, i, isSelf);
                        break;
                    case BattleBuffEventType.BuffPropertyChange:
                        BuffEventBuffPropertyChangeMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, buffEventSet, i, isSelf);
                        break;
                    case BattleBuffEventType.ForbiddenBuff:
                        BuffEventForbiddenBuffMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, buffEventSet, i, isSelf);
                        break;
                    case BattleBuffEventType.RoleStateChange:
                        BuffEventRoleStateChangeMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, buffEventSet, i, isSelf);
                        break;
                    case BattleBuffEventType.UseDesignateSkill:
                        BuffEventUseDesignateSkillMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, buffEventSet, i, isSelf);
                        break;
                    case BattleBuffEventType.DamageOrHeal:
                        BuffEventDamageOrHealMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, buffEventSet, i, isSelf);
                        break;
                    case BattleBuffEventType.Shield:
                        BuffEventShieldMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, buffEventSet, i, isSelf);
                        break;
                    case BattleBuffEventType.DamageReduce:
                        BuffEventDamageReduceMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, buffEventSet, i, isSelf);
                        break;
                    case BattleBuffEventType.TakeHurtForOther:
                        BuffEventTakeHurtForOtherMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, buffEventSet, i, isSelf);
                        break;
                    case BattleBuffEventType.AddBuff:
                        BuffEventAddBuffMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, buffEventSet, i, isSelf);
                        break;
                    case BattleBuffEventType.DispelBuff:
                        BuffEventDispelBuffMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, buffEventSet, i, isSelf);
                        break;
                    case BattleBuffEventType.NotResurgence:
                        BuffEventNotResurgenceMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, buffEventSet, i, isSelf);
                        break;
                }
            }
        }


        #region RolePropertyChange  角色属性改变

        /// <summary>
        ///变动属性类型
        /// </summary>
        /// <param name="buffId"></param>
        /// <param name="roleId"></param>
        /// <param name="currentId"></param>
        /// <param name="playerSetObject"></param>
        /// <param name="enemySetObject"></param>
        /// <param name="buffDict"></param>
        /// <param name="buffEventSet"></param>
        /// <param name="i"></param>
        public void BuffEventRolePropertyChangeMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf)
        {
            switch (buffEventSet[i].battleBuffEventType_RolePropertyChange)
            {
                case BattleBuffEventType_RolePropertyChange.Health:
                    BuffEventSourceDataTypeMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, buffEventSet, i, isSelf);
                    break;
                case BattleBuffEventType_RolePropertyChange.ZhenYuan:
                    break;
                case BattleBuffEventType_RolePropertyChange.ShenHun:
                    break;
                case BattleBuffEventType_RolePropertyChange.JingXue:
                    break;
                case BattleBuffEventType_RolePropertyChange.PhysicAttack:
                    break;
                case BattleBuffEventType_RolePropertyChange.PhysicDefend:
                    BuffEventSourceDataTypeMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, buffEventSet, i, isSelf);
                    break;
                case BattleBuffEventType_RolePropertyChange.MagicAttack:
                    break;
                case BattleBuffEventType_RolePropertyChange.MagicDefend:
                    break;
                case BattleBuffEventType_RolePropertyChange.AttackSpeed:
                    break;
                case BattleBuffEventType_RolePropertyChange.HitRate:
                    break;
                case BattleBuffEventType_RolePropertyChange.BasicDodgeRate:
                    break;
                case BattleBuffEventType_RolePropertyChange.PhysicDodgeRate:
                    break;
                case BattleBuffEventType_RolePropertyChange.MagicDodgeRate:
                    break;
                case BattleBuffEventType_RolePropertyChange.PhysicCritRate:
                    break;
                case BattleBuffEventType_RolePropertyChange.MagicCritRate:
                    break;
                case BattleBuffEventType_RolePropertyChange.ReduceCritRate:
                    break;
                case BattleBuffEventType_RolePropertyChange.PhysicCritDamage:
                    break;
                case BattleBuffEventType_RolePropertyChange.MagicCritDamage:
                    break;
                case BattleBuffEventType_RolePropertyChange.ReduceCritDamage:
                    break;
                case BattleBuffEventType_RolePropertyChange.TakeDamage:
                    break;
                case BattleBuffEventType_RolePropertyChange.ReceiveDamage:
                    break;
                case BattleBuffEventType_RolePropertyChange.IgnoreDefend:
                    break;
                case BattleBuffEventType_RolePropertyChange.DamageNumFluctuations:
                    break;
                case BattleBuffEventType_RolePropertyChange.HealEffect:
                    break;
            }
        }

        /// <summary>
        /// 变动的数值来源
        /// </summary>
        /// <param name="buffId"></param>
        /// <param name="roleId"></param>
        /// <param name="currentId"></param>
        /// <param name="playerSetObject"></param>
        /// <param name="enemySetObject"></param>
        /// <param name="buffDict"></param>
        /// <param name="buffEventSet"></param>
        /// <param name="i"></param>
        public void BuffEventSourceDataTypeMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf)
        {
            switch (buffEventSet[i].buffRolePropertyChange_SourceDataType)
            {
                case BuffEvent_RolePropertyChange_SourceDataType.MaxHealth:
                    switch (isSelf)
                    {
                        case true:
                            if (enemySetObject.EnemyStatusDTO.EnemyMaxHP <= 0)
                                return;
                            enemySetObject.EnemyStatusDTO.EnemyMaxHP += (enemySetObject.EnemyStatusDTO.EnemyMaxHP * buffEventSet[i].percentValue) / 100 + buffEventSet[i].fixedValue;
                            ///TODO
                            break;
                        case false:
                            if (playerSetObject.RoleStatusDTO.RoleMaxHP <= 0)
                                return;
                            playerSetObject.RoleStatusDTO.RoleMaxHP += (playerSetObject.RoleStatusDTO.RoleMaxHP * buffEventSet[i].percentValue) / 100 + buffEventSet[i].fixedValue;
                            ///TODO
                            break;
                    }
                    break;
                case BuffEvent_RolePropertyChange_SourceDataType.MaxZhenYuan:
                    break;
                case BuffEvent_RolePropertyChange_SourceDataType.MaxShenHun:
                    break;
                case BuffEvent_RolePropertyChange_SourceDataType.PhysicAttack:
                    break;
                case BuffEvent_RolePropertyChange_SourceDataType.MagicAttack:
                    break;
                case BuffEvent_RolePropertyChange_SourceDataType.PhysicDefend:
                    switch (isSelf)
                    {
                        case true:
                            if (enemySetObject.EnemyStatusDTO.EnemyDefence_Physical <= 0)
                                return;
                            enemySetObject.EnemyStatusDTO.EnemyDefence_Physical += (enemySetObject.EnemyStatusDTO.EnemyDefence_Physical * buffEventSet[i].percentValue) / 100 + buffEventSet[i].fixedValue;
                            ///TODO
                            break;
                        case false:
                            if (playerSetObject.RoleStatusDTO.DefendPhysical <= 0)
                                return;
                            playerSetObject.RoleStatusDTO.DefendPhysical += (playerSetObject.RoleStatusDTO.DefendPhysical * buffEventSet[i].percentValue) / 100 + buffEventSet[i].fixedValue;
                            ///TODO
                            break;
                    }
                    break;
                case BuffEvent_RolePropertyChange_SourceDataType.MagicDefend:
                    break;
                case BuffEvent_RolePropertyChange_SourceDataType.TakeDamage:
                    break;
                case BuffEvent_RolePropertyChange_SourceDataType.SkillDamage:
                    break;
            }
        }

        #endregion

        #region BuffPropertyChange buff属性变动   TODO
        public void BuffEventBuffPropertyChangeMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf)
        {
            switch (buffEventSet[i].buffPropertyChangeType)
            {
                case BuffEvent_PropertyChangeType.TakeDamage:
                    break;
                case BuffEvent_PropertyChangeType.ReceiveDamage:
                    break;
                case BuffEvent_PropertyChangeType.IgnoreDefend:
                    break;
                case BuffEvent_PropertyChangeType.BasicDodgeRate:
                    break;
            }
        }
        #endregion

        #region ForbiddenBuff  禁用buff列表   TODO
        public void BuffEventForbiddenBuffMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf)
        {

        }
        #endregion

        #region RoleStateChange 角色状态改变  TODO
        public void BuffEventRoleStateChangeMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf)
        {
            switch (buffEventSet[i].buffRoleStateChangeType)
            {
                case BuffEvent_RoleStateChangeType.Dizziness:
                    break;
                case BuffEvent_RoleStateChangeType.Frozen:
                    break;
                case BuffEvent_RoleStateChangeType.Chaos:
                    break;
                case BuffEvent_RoleStateChangeType.LostHeart:
                    break;
                case BuffEvent_RoleStateChangeType.Hide:
                    break;
            }
        }
        #endregion
            
        #region UseDesignateSkill 使用指定技能 TODO
        public void BuffEventUseDesignateSkillMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf)
        {

        }
        #endregion

        #region DamageOrHeal 伤害或者治疗

        /// <summary>
        /// 伤害 或者治疗
        /// </summary>
        /// <param name="buffId"></param>
        /// <param name="roleId"></param>
        /// <param name="currentId"></param>
        /// <param name="playerSetObject"></param>
        /// <param name="enemySetObject"></param>
        /// <param name="buffDict"></param>

        public void BuffEventDamageOrHealMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf)
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
                                    BuffEventResultsMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, buffEventSet, i, isSelf);
                                    break;
                                case false:
                                    BuffEventResultsMothed(buffId, roleId, currentId, playerSetObject, enemySetObject, buffDict, buffEventSet, i, isSelf);
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
        public void BuffEventResultsMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf)
        {
            int damgeValue = 0;
            switch (buffEventSet[i].flag_3)
            {
                case true:
                    damgeValue = (buffEventSet[i].percentValue * playerSetObject.RoleStatusDTO.RoleMaxHP) / 100 + buffEventSet[i].fixedValue;
                    switch (buffEventSet[i].buffEvent_DamageOrHeal_EffectTargetType)
                    {
                        case BuffEvent_DamageOrHeal_EffectTargetType.Health:
                            List<BattleBuffEventDTO> battleBuffEvents = new List<BattleBuffEventDTO>();
                            battleBuffEvents.Add(new BattleBuffEventDTO() { index = i, TargetId = currentId, TriggerId = currentId, BuffValue = damgeValue });
                            playerSetObject.RoleStatusDTO.RoleHP += damgeValue;
                            _buffToRoomIdAfter.Add(new BattleBuffDTO() { bufferId = buffId, battleBuffDTOs = battleBuffEvents });
                            break;
                        case BuffEvent_DamageOrHeal_EffectTargetType.ShenHun:
                            break;
                        case BuffEvent_DamageOrHeal_EffectTargetType.ZhenYuan:
                            break;
                    }
                    break;
                case false:
                    damgeValue = (buffEventSet[i].percentValue * enemySetObject.EnemyStatusDTO.EnemyMaxHP) / 100 + buffEventSet[i].fixedValue;
                    switch (buffEventSet[i].buffEvent_DamageOrHeal_EffectTargetType)
                    {
                        case BuffEvent_DamageOrHeal_EffectTargetType.Health:
                            List<BattleBuffEventDTO> battleBuffEvents = new List<BattleBuffEventDTO>();
                            battleBuffEvents.Add(new BattleBuffEventDTO() { index = i, TargetId = enemySetObject.EnemyStatusDTO.EnemyId, TriggerId = enemySetObject.EnemyStatusDTO.EnemyId, BuffValue = damgeValue });
                            enemySetObject.EnemyStatusDTO.EnemyHP += damgeValue;
                            _buffToRoomIdAfter.Add(new BattleBuffDTO() { bufferId = buffId, battleBuffDTOs = battleBuffEvents });

                            #region ob
                            /*
                             *   _buffToRoomIdAfter.Add(new BattleBuffDTO() { TargetId = enemySetObject.EnemyStatusDTO.EnemyId, TriggerId = enemySetObject.EnemyStatusDTO.EnemyId, bufferId = buffId, BuffValue = damgeValue });
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
            }
        }
        #endregion

        #region Shield 护盾 TODO
        public void BuffEventShieldMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf)
        {

        }
        #endregion

        #region DamageReduce 该次伤害减免 TODO
        public void BuffEventDamageReduceMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf)
        {

        }
        #endregion

        #region TakeHurtForOther 替他人承担伤害 TODO
        public void BuffEventTakeHurtForOtherMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf)
        {

        }
        #endregion

        #region AddBuff 替他人承担伤害 TODO
        public void BuffEventAddBuffMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf)
        {

        }
        #endregion

        #region DispelBuff 驱散 TODO
        public void BuffEventDispelBuffMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf)
        {

        }
        #endregion

        #region NotResurgence 无法复活 TODO
        public void BuffEventNotResurgenceMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf)
        {

        }
        #endregion

    }
}
