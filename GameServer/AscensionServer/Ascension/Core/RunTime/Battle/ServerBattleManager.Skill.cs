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
/// 处理战斗中所有的组队情况
/// </summary>
namespace AscensionServer
{
    public partial class ServerBattleManager
    {
        ///buff  针对buff的技能id
        int buffToSkillId;
         #region 统一技能的修改

        /// <summary>
        /// 2020.11.06 20:06
        /// 统一计算技能 单段 多段
        /// </summary>
        public void SkillSingleOrStaged(BattleTransferDTO battleTransferDTOs, int roleId, int currentId, BattleSkillData battleSkillData, int special = 0)
        {
            var enemySet = _teamIdToBattleInit[roleId].enemyUnits;
            var skillDataDamageNum = battleSkillData.battleSkillDamageNumDataList;
            var  addBuffDataSet  =  battleSkillData.battleSkillAddBuffList;
            var eventDataSet = battleSkillData.battleSkillEventDataList;
            var removeBuffDataSet = battleSkillData.battleSkillRemoveBuffDataList;
            if (skillDataDamageNum.Count == 1 && battleSkillData.TargetNumber == 1)
            {
                for (int oc = 0; oc < skillDataDamageNum.Count; oc++)
                {
                    var enemySetObject = enemySet.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.ToList()[oc].Key);
                    var NumSource = skillDataDamageNum[oc].baseNumSourceDataList;
                    //换取目标打
                    if (enemySetObject.EnemyStatusDTO.EnemyHP > 0)
                    {
                        //TODO
                        for (int ox = 0; ox < NumSource.Count; ox++)
                        {
                            if (enemySetObject.EnemyStatusDTO.EnemyHP <= 0)
                                break;
                          
                            var TargetInfosSet = ServerToClientResult(new TargetInfoDTO() { TargetID = enemySetObject.EnemyStatusDTO.EnemyId, TargetHPDamage = -NumSource[ox].mulitity,  AddTargetBuff= AddBufferMethod(addBuffDataSet, roleId, currentId, enemySetObject, oc), RemoveTargetBuff = RemoveBufferMethod(removeBuffDataSet,roleId,currentId,oc) });
                            BattleSkillEventDataMethod(eventDataSet, roleId, currentId, enemySetObject, ox, TargetInfosSet);
                            enemySetObject.EnemyStatusDTO.EnemyHP -= NumSource[ox].mulitity;
                            if (NumSource.Count - 1 == ox || enemySetObject.EnemyStatusDTO.EnemyHP <= 0)
                                teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                            else
                                teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                        }
                    }
                    else
                    {
                        if (AIToHPMethod(roleId, enemySet).Count == 0)
                        {
                            Utility.Debug.LogError("AI  全部死亡");
                            break;
                        }
                        var RandomTarget = RandomManager(oc, 0, AIToHPMethod(roleId, enemySet).Count);
                        for (int ox = 0; ox < NumSource.Count; ox++)
                        {
                            if (enemySetObject.EnemyStatusDTO.EnemyHP <= 0)
                                break;
                            var TargetInfosSet = ServerToClientResult(new TargetInfoDTO() { TargetID = enemySet[RandomTarget].EnemyStatusDTO.EnemyId, TargetHPDamage = -NumSource[ox].mulitity ,  AddTargetBuff = AddBufferMethod(addBuffDataSet, roleId, currentId, enemySetObject, oc), RemoveTargetBuff = RemoveBufferMethod(removeBuffDataSet, roleId, currentId, oc) });
                            BattleSkillEventDataMethod(eventDataSet, roleId, currentId, enemySetObject, ox, TargetInfosSet);
                            enemySetObject.EnemyStatusDTO.EnemyHP -= NumSource[ox].mulitity;
                            if (NumSource.Count - 1 == ox || enemySetObject.EnemyStatusDTO.EnemyHP <= 0)
                                teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                            else
                                teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                        }
                    }
                }
            }
            ///多段伤害
            else
            {
                if (battleSkillData.attackProcessType == AttackProcess_Type.SingleUse)
                {
                    if (skillDataDamageNum[0].baseNumSourceDataList.Count == 1)
                    {
                        for (int zo = 0; zo < TargetID.Count; zo++)
                        {
                            for (int op = 0; op < skillDataDamageNum.Count; op++)
                            {
                                var servivalTarget = enemySet.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[zo]);
                                if (servivalTarget.EnemyStatusDTO.EnemyHP <= 0)
                                    break;
                                var tranfsSet = ServerToClientResult(new TargetInfoDTO() { TargetID = servivalTarget.EnemyStatusDTO.EnemyId, TargetHPDamage = -skillDataDamageNum[op].baseNumSourceDataList[zo].mulitity,  AddTargetBuff = AddBufferMethod(addBuffDataSet, roleId, currentId, servivalTarget, zo), RemoveTargetBuff= RemoveBufferMethod(removeBuffDataSet, roleId, currentId, zo) });
                                BattleSkillEventDataMethod(eventDataSet, roleId, currentId, servivalTarget, op, tranfsSet);
                                servivalTarget.EnemyStatusDTO.EnemyHP -= skillDataDamageNum[op].baseNumSourceDataList[zo].mulitity;
                                if (skillDataDamageNum.Count - 1 == op|| servivalTarget.EnemyStatusDTO.EnemyHP <= 0)
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = tranfsSet });
                                else
                                    teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = tranfsSet });
                            }
                        }
                    }
                    else
                    {
                        List<TargetInfoDTO> targetInfoDTOsSet = new List<TargetInfoDTO>();
                        for (int zo = 0; zo < TargetID.Count; zo++)
                        {
                            var servivalTarget = enemySet.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[zo]);
                            BattleSkillEventDataMethod(eventDataSet, roleId, currentId, servivalTarget, zo, targetInfoDTOsSet);
                            servivalTarget.EnemyStatusDTO.EnemyHP -= skillDataDamageNum[0].baseNumSourceDataList[zo].mulitity;
                            var tranfsSet = ServerToClientResults(new TargetInfoDTO() { TargetID = servivalTarget.EnemyStatusDTO.EnemyId, TargetHPDamage = -skillDataDamageNum[0].baseNumSourceDataList[zo].mulitity,  AddTargetBuff = AddBufferMethod(addBuffDataSet, roleId, currentId, servivalTarget, zo, targetInfoDTOsSet), RemoveTargetBuff = RemoveBufferMethod(removeBuffDataSet, roleId, currentId, zo)});
                            targetInfoDTOsSet.Add(tranfsSet);
                        }
                        teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = targetInfoDTOsSet });
                    }
                }
                else if (battleSkillData.attackProcessType == AttackProcess_Type.Staged)
                {
                    for (int n = 0; n < TargetID.Count; n++)
                    {
                        var survivalTarget = enemySet.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[n]);
                        if (survivalTarget.EnemyStatusDTO.EnemyHP <= 0)
                            break;
                        var TargetInfosSet = ServerToClientResult(new TargetInfoDTO() { TargetID = survivalTarget.EnemyStatusDTO.EnemyId, TargetHPDamage = -skillDataDamageNum[0].baseNumSourceDataList[n].mulitity,  AddTargetBuff = AddBufferMethod(addBuffDataSet, roleId, currentId, survivalTarget, n), RemoveTargetBuff = RemoveBufferMethod(removeBuffDataSet, roleId, currentId, n) });
                        BattleSkillEventDataMethod(eventDataSet, roleId, currentId, survivalTarget, n, TargetInfosSet);
                        survivalTarget.EnemyStatusDTO.EnemyHP -= skillDataDamageNum[0].baseNumSourceDataList[n].mulitity;
                        if (TargetID.Count - 1 == n || survivalTarget.EnemyStatusDTO.EnemyHP <= 0)
                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                        else
                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                    }
                }
            }
        }



        /// <summary>
        /// 技能 添加buff
        /// </summary>
        public List<BufferBattleDataDTO> AddBufferMethod(List<BattleSkillAddBuffData> addBuffDataSet,int roleId,int currentId,EnemyBattleDataDTO enemySetObject, int index, List<TargetInfoDTO> targetInfoDTOsSet = null)
        {
            RoleBattleDataDTO tempSet = null;
            EnemyBattleDataDTO enemySet = null;
            List<BufferBattleDataDTO> bufferId = new List<BufferBattleDataDTO>();
            if (addBuffDataSet.Count == 0)
                return bufferId;
            var playerSet = _teamIdToBattleInit[roleId].playerUnits;
            var bufferSet = _teamIdToBattleInit[roleId].bufferUnits;
            var playerSetObject = playerSet.Find(x => x.RoleStatusDTO.RoleID == currentId);
            tempSet = playerSetObject;
            enemySet = enemySetObject;

            for (int og = 0; og < addBuffDataSet.Count; og++)
            {
                int selfTempValue = 0;
                int targetTempValue = 0;
                switch (addBuffDataSet[og].selfAddBuffProbability.battleSkillBuffProbSource)
                {
                    case BattleSkillNumSourceType.MaxHealth:
                        if (addBuffDataSet[og].selfAddBuffProbability.addOrReduce)
                            selfTempValue = (tempSet.RoleStatusDTO.RoleMaxHP * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        else
                            selfTempValue = -(tempSet.RoleStatusDTO.RoleMaxHP * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        break;
                    case BattleSkillNumSourceType.NowHealth:
                        if (addBuffDataSet[og].selfAddBuffProbability.addOrReduce)
                            selfTempValue = (tempSet.RoleStatusDTO.RoleHP * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        else
                            selfTempValue = -(tempSet.RoleStatusDTO.RoleHP * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        break;
                    case BattleSkillNumSourceType.HasLostHealth:
                        if (addBuffDataSet[og].selfAddBuffProbability.addOrReduce)
                            selfTempValue = ((tempSet.RoleStatusDTO.RoleMaxHP - tempSet.RoleStatusDTO.RoleHP) * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        else
                            selfTempValue = -((tempSet.RoleStatusDTO.RoleMaxHP - tempSet.RoleStatusDTO.RoleHP) * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        break;
                    case BattleSkillNumSourceType.MaxZhenYuan:
                        if (addBuffDataSet[og].selfAddBuffProbability.addOrReduce)
                            selfTempValue = (tempSet.RoleStatusDTO.RoleMaxMP * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        else
                            selfTempValue = -(tempSet.RoleStatusDTO.RoleMaxMP * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        break;
                    case BattleSkillNumSourceType.NowZhenYuan:
                        if (addBuffDataSet[og].selfAddBuffProbability.addOrReduce)
                            selfTempValue = (tempSet.RoleStatusDTO.RoleMP * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        else
                            selfTempValue = -(tempSet.RoleStatusDTO.RoleMP * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        break;
                    case BattleSkillNumSourceType.MaxShenHun:
                        if (addBuffDataSet[og].selfAddBuffProbability.addOrReduce)
                            selfTempValue = (tempSet.RoleStatusDTO.RoleMaxSoul * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        else
                            selfTempValue = - (tempSet.RoleStatusDTO.RoleMaxSoul * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        break;
                    case BattleSkillNumSourceType.NowShenHun:
                        if (addBuffDataSet[og].selfAddBuffProbability.addOrReduce)
                            selfTempValue = (tempSet.RoleStatusDTO.RoleSoul * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        else
                            selfTempValue = -(tempSet.RoleStatusDTO.RoleSoul * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        break;
                    case BattleSkillNumSourceType.PhysicAttack:
                        break;
                    case BattleSkillNumSourceType.MagicAttack:
                        break;
                    case BattleSkillNumSourceType.PhysicDefense:
                        break;
                    case BattleSkillNumSourceType.MagicDefense:
                        break;
                    case BattleSkillNumSourceType.AttackSpeed:
                        if (addBuffDataSet[og].selfAddBuffProbability.addOrReduce)
                            selfTempValue = (tempSet.RoleStatusDTO.AttackSpeed * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        else
                            selfTempValue = -(tempSet.RoleStatusDTO.AttackSpeed * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        break;
                }

                switch (addBuffDataSet[og].targetAddBuffProbability.battleSkillBuffProbSource)
                {
                    case BattleSkillNumSourceType.MaxHealth:
                        if (addBuffDataSet[og].selfAddBuffProbability.addOrReduce)
                            targetTempValue = (enemySet.EnemyStatusDTO.EnemyHP * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        else
                            targetTempValue = -(enemySet.EnemyStatusDTO.EnemyHP * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        break;
                    case BattleSkillNumSourceType.NowHealth:
                        if (addBuffDataSet[og].selfAddBuffProbability.addOrReduce)
                            targetTempValue = (tempSet.RoleStatusDTO.RoleHP * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        else
                            targetTempValue = -(tempSet.RoleStatusDTO.RoleHP * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        break;
                    case BattleSkillNumSourceType.HasLostHealth:
                        if (addBuffDataSet[og].selfAddBuffProbability.addOrReduce)
                            targetTempValue = ((tempSet.RoleStatusDTO.RoleMaxHP - tempSet.RoleStatusDTO.RoleHP) * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        else
                            targetTempValue = -((tempSet.RoleStatusDTO.RoleMaxHP - tempSet.RoleStatusDTO.RoleHP) * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        break;
                    case BattleSkillNumSourceType.MaxZhenYuan:
                        if (addBuffDataSet[og].selfAddBuffProbability.addOrReduce)
                            targetTempValue = (tempSet.RoleStatusDTO.RoleMaxMP * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        else
                            targetTempValue = -(tempSet.RoleStatusDTO.RoleMaxMP * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        break;
                    case BattleSkillNumSourceType.NowZhenYuan:
                        if (addBuffDataSet[og].selfAddBuffProbability.addOrReduce)
                            targetTempValue = (tempSet.RoleStatusDTO.RoleMP * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        else
                            targetTempValue = -(tempSet.RoleStatusDTO.RoleMP * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        break;
                    case BattleSkillNumSourceType.MaxShenHun:
                        if (addBuffDataSet[og].selfAddBuffProbability.addOrReduce)
                            targetTempValue = (tempSet.RoleStatusDTO.RoleMaxSoul * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        else
                            targetTempValue = -(tempSet.RoleStatusDTO.RoleMaxSoul * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        break;
                    case BattleSkillNumSourceType.NowShenHun:
                        if (addBuffDataSet[og].selfAddBuffProbability.addOrReduce)
                            targetTempValue = (tempSet.RoleStatusDTO.RoleSoul * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        else
                            targetTempValue = -(tempSet.RoleStatusDTO.RoleSoul * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        break;
                    case BattleSkillNumSourceType.PhysicAttack:
                        break;
                    case BattleSkillNumSourceType.MagicAttack:
                        break;
                    case BattleSkillNumSourceType.PhysicDefense:
                        break;
                    case BattleSkillNumSourceType.MagicDefense:
                        break;
                    case BattleSkillNumSourceType.AttackSpeed:
                        if (addBuffDataSet[og].selfAddBuffProbability.addOrReduce)
                            targetTempValue = (tempSet.RoleStatusDTO.AttackSpeed * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        else
                            targetTempValue = -(tempSet.RoleStatusDTO.AttackSpeed * addBuffDataSet[og].selfAddBuffProbability.multiplyPropValue) / 10000;
                        break;
                }

                var tempSelect = RandomManager(og, 0, 101);
                var buffValue =  addBuffDataSet[og].basePropList[index] + (selfTempValue + targetTempValue);
                if (tempSelect <= buffValue)
                {
                    bufferSet.Add(new BufferBattleDataDTO() { RoleId = currentId, BufferData = new BufferData() { bufferId = addBuffDataSet[og].buffId, RoundNumber = addBuffDataSet[og].round } });
                    bufferId.Add(new BufferBattleDataDTO() { RoleId = currentId, BufferData = new BufferData() { bufferId = addBuffDataSet[og].buffId, RoundNumber = addBuffDataSet[og].round } });
                    //TODO
                    BuffManagerMethod(addBuffDataSet[og].buffId,roleId, currentId, playerSetObject,enemySetObject,og);
                }
                else
                    Utility.Debug.LogInfo("Buffer 添加失败====>>>>" + buffValue);
            }
            return bufferId;
        }

          /// <summary>
        /// 技能 移除buff
        /// </summary>
        public List<int> RemoveBufferMethod(List<BattleSkillRemoveBuffData> removeBuffDataSet,int roleId,int currentId,int index)
        {
       
            List<int> bufferId = new List<int>();
            if (removeBuffDataSet.Count == 0)
                return bufferId;
            var playerSet = _teamIdToBattleInit[roleId].playerUnits;
            var bufferSet = _teamIdToBattleInit[roleId].bufferUnits;
          
            for (int og = 0; og < removeBuffDataSet.Count; og++)
            {
                var tempSelect = RandomManager(og, 0, 100);
                if (removeBuffDataSet[og].probability == 100|| tempSelect >= removeBuffDataSet[og].probability)
                {
                    for (int ko = 0; ko < removeBuffDataSet[og].buffIdList.Count; ko++)
                    {
                        var bufferObject = bufferSet.Find(x => x.RoleId == currentId && x.BufferData.bufferId == removeBuffDataSet[og].buffIdList[ko]);
                        if (bufferObject != null)
                        {
                            bufferSet.Remove(bufferObject);
                            bufferId.Add(removeBuffDataSet[og].buffIdList[ko]);
                        }
                    }
                }
                else
                    Utility.Debug.LogInfo("Buffer  使用失败====>>>>" + removeBuffDataSet[og].probability);
            }
            return bufferId;
        }

        /// <summary>
        ///技能触发时机
        /// </summary>
        public void BattleSkillEventDataMethod(List<BattleSkillEventData> battleSkillEvents, int roleId, int currentId, EnemyBattleDataDTO enemySetObject,int index, List<TargetInfoDTO> targetInfoDTOsSet)
        {
            var playerSet = _teamIdToBattleInit[roleId].playerUnits;
            var playerSetObject = playerSet.Find(x => x.RoleStatusDTO.RoleID == currentId);
            if (battleSkillEvents.Count == 0)
                return;
            for (int ov = 0; ov < battleSkillEvents.Count; ov++)
            {
                switch (battleSkillEvents[ov].battleSkillEventTriggerTime)
                {
                    case BattleSkillEventTriggerTime.BeforeAttack:
                        battleSkillEventTriggerCondition(battleSkillEvents[ov], roleId, currentId, enemySetObject, playerSetObject, targetInfoDTOsSet);
                        break;
                    case BattleSkillEventTriggerTime.BehindAttack:
                        break;
                }
            }
        }

        /// <summary>
        ///  技能 触发来源
        /// </summary>
        /// <param name="battleSkillEvents"></param>
        /// <param name="roleId"></param>
        /// <param name="currentId"></param>
        /// <param name="enemySetObject"></param>
        /// <param name="targetInfoDTOsSet"></param>
        public void battleSkillEventTriggerCondition(BattleSkillEventData battleSkillEvents, int roleId, int currentId, EnemyBattleDataDTO enemySetObject,RoleBattleDataDTO playerSetObject, List<TargetInfoDTO> targetInfoDTOsSet)
        {
            switch (battleSkillEvents.battleSkillEventTriggerNumSourceType)
            {
                case BattleSkillEventTriggerNumSourceType.Health:
                    battleSkillEventTriggerNumSourceType(battleSkillEvents, roleId, currentId, enemySetObject, playerSetObject, targetInfoDTOsSet);
                    break;
                case BattleSkillEventTriggerNumSourceType.PhysicDefense:
                    battleSkillEventTriggerNumSourceTypePhysicDefense(battleSkillEvents, roleId, currentId, enemySetObject, playerSetObject, targetInfoDTOsSet);
                    break;
                case BattleSkillEventTriggerNumSourceType.MagicDefense:
                    break;
                case BattleSkillEventTriggerNumSourceType.ShenHun:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 技能触发条件
        /// </summary>
        /// <param name="battleSkillEvents"></param>
        /// <param name="roleId"></param>
        /// <param name="currentId"></param>
        /// <param name="enemySetObject"></param>
        /// <param name="targetInfoDTOsSet"></param>
        public void battleSkillEventTriggerNumSourceType(BattleSkillEventData battleSkillEvents, int roleId, int currentId, EnemyBattleDataDTO enemySetObject, RoleBattleDataDTO playerSetObject, List<TargetInfoDTO> targetInfoDTOsSet)
        {
            switch (battleSkillEvents.battleSkillEventTriggerCondition)
            {
                case BattleSkillEventTriggerCondition.None:
                    break;
                case BattleSkillEventTriggerCondition.Crit:
                    break;
                case BattleSkillEventTriggerCondition.TargetPropertyUnder:
                    break;
                case BattleSkillEventTriggerCondition.TargetPropertyOver:
                    var tempNumSourceType = ((float)enemySetObject.EnemyStatusDTO.EnemyHP / enemySetObject.EnemyStatusDTO.EnemyMaxHP) * 100;
                    if (tempNumSourceType <= battleSkillEvents.conditionPercentNum|| battleSkillEvents.conditionFixedNum != 0)
                        battleSkillTriggerEventType(battleSkillEvents, roleId, currentId, enemySetObject, playerSetObject, targetInfoDTOsSet);
                    break;
                case BattleSkillEventTriggerCondition.SelfPropertyUnder:
                    var tempNumSourcePlayer = ((float)playerSetObject.RoleStatusDTO.RoleHP / playerSetObject.RoleStatusDTO.RoleMaxHP) * 100;
                    if (tempNumSourcePlayer> battleSkillEvents.conditionPercentNum || battleSkillEvents.conditionFixedNum != 0)
                    {

                    }
                    break;
                case BattleSkillEventTriggerCondition.SelfPropertyOver:
                    break;
            }
        }


        public void battleSkillEventTriggerNumSourceTypePhysicDefense(BattleSkillEventData battleSkillEvents, int roleId, int currentId, EnemyBattleDataDTO enemySetObject, RoleBattleDataDTO playerSetObject, List<TargetInfoDTO> targetInfoDTOsSet)
        {
            switch (battleSkillEvents.battleSkillEventTriggerCondition)
            {
                case BattleSkillEventTriggerCondition.None:
                    break;
                case BattleSkillEventTriggerCondition.Crit:
                    break;
                case BattleSkillEventTriggerCondition.TargetPropertyUnder:
                    break;
                case BattleSkillEventTriggerCondition.TargetPropertyOver:
                    //var tempNumSourceType = ((float)enemySetObject.EnemyStatusDTO.EnemyDefence_Physical / enemySetObject.EnemyStatusDTO.EnemyMaxHP) * 100;
                    //if (tempNumSourceType <= battleSkillEvents.conditionPercentNum || battleSkillEvents.conditionFixedNum != 0)
                    //    battleSkillTriggerEventType(battleSkillEvents, roleId, currentId, enemySetObject, playerSetObject, targetInfoDTOsSet);
                    break;
                case BattleSkillEventTriggerCondition.SelfPropertyUnder:
                    break;
                case BattleSkillEventTriggerCondition.SelfPropertyOver:
                    break;
            }
        }


        /// <summary>
        /// 技能触发事件类型
        /// </summary>
        /// <param name="battleSkillEvents"></param>
        /// <param name="roleId"></param>
        /// <param name="currentId"></param>
        /// <param name="enemySetObject"></param>
        /// <param name="targetInfoDTOsSet"></param>
        public void battleSkillTriggerEventType(BattleSkillEventData battleSkillEvents,int roleId,int currentId, EnemyBattleDataDTO enemySetObject,RoleBattleDataDTO playerSetObject, List<TargetInfoDTO> targetInfoDTOsSet)
        {
           
            switch (battleSkillEvents.battleSkillTriggerEventType)
            {
                case BattleSkillTriggerEventType.Skill:
                    break;
                case BattleSkillTriggerEventType.Heal:
                    enemySetObject.EnemyStatusDTO.EnemyHP += battleSkillEvents.EventValue;
                    break;
                case BattleSkillTriggerEventType.SuckBlood:
                    playerSetObject.RoleStatusDTO.RoleHP += battleSkillEvents.EventValue;
                    var tranfsSet = ServerToClientResults(new TargetInfoDTO() { TargetID = currentId, TargetHPDamage = battleSkillEvents.EventValue});
                    targetInfoDTOsSet.Add(tranfsSet);
                    break;
                case BattleSkillTriggerEventType.AddCrit:
                    //enemySetObject.EnemyStatusDTO. += battleSkillEvents[ov].EventValue;
                    break;
                case BattleSkillTriggerEventType.AddDamage:
                    break;
                case BattleSkillTriggerEventType.AddPierce:
                    break;
            }
        }

        #endregion


        #region 统一技能治疗术

        /// <summary>
        /// 2020.11.06 20:45
        /// 统一技能治疗术
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="currentId"></param>
        /// <param name="battleSkillData"></param>
        public void PlayerToSkillReturnBlood(BattleTransferDTO battleTransferDTOs, int roleId, int currentId, BattleSkillData battleSkillData)
        {

            var pU = _teamIdToBattleInit[roleId].playerUnits;
            var petU = _teamIdToBattleInit[roleId].petUnits;
            var bSD = battleSkillData.battleSkillDamageNumDataList;

            ///给自己回血
            if (battleSkillData.TargetNumber == 1)
            {
                var playerObject = pU.Find(x => x.RoleStatusDTO.RoleID == currentId);
                //TODO需要计算
                if (playerObject.RoleStatusDTO.RoleHP + bSD[0].baseNumSourceDataList[0].mulitity >= playerObject.RoleStatusDTO.RoleMaxHP)
                    playerObject.RoleStatusDTO.RoleHP = playerObject.RoleStatusDTO.RoleMaxHP;
                else
                    playerObject.RoleStatusDTO.RoleHP += bSD[0].baseNumSourceDataList[0].mulitity;

                var TargetInfosSet = ServerToClientResult(new TargetInfoDTO() { TargetID = currentId, TargetHPDamage = bSD[0].baseNumSourceDataList[0].mulitity });
                teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
            }
            else
            {
                var tempSet = PlayerToPetID(roleId);
                List<TargetInfoDTO> targetInfoDTOsSet = new List<TargetInfoDTO>();
                if (battleSkillData.TargetNumber >= tempSet.Count)
                {
                    for (int ol = 0; ol < tempSet.Count; ol++)
                    {
                        var objectOwner = ReleaseToOwner(tempSet[ol], tempSet[ol], roleId);
                        var typeName = objectOwner.GetType().Name;
                        SelectToTarget(targetInfoDTOsSet, typeName, objectOwner, bSD, ol);
                    }
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = targetInfoDTOsSet });
                }
                else
                {
                    for (int ol = 0; ol < battleSkillData.TargetNumber; ol++)
                    {
                        var RandomTarget = RandomManager(ol, 0, battleSkillData.TargetNumber);
                        var objectOwner = ReleaseToOwner(tempSet[RandomTarget], tempSet[RandomTarget], roleId);
                        var typeName = objectOwner.GetType().Name;
                        SelectToTarget(targetInfoDTOsSet, typeName, objectOwner, bSD, ol);
                        tempSet.RemoveAt(RandomTarget);
                    }
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = targetInfoDTOsSet });
                }
            }
        }

        /// <summary>
        /// 2020 11.07 17:21
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<int> PlayerToPetID(int roleId)
        {
            List<int> TargetIdSet = new List<int>();
            for (int i = 0; i < _teamIdToBattleInit[roleId].playerUnits.Count
; i++)
                TargetIdSet.Add(_teamIdToBattleInit[roleId].playerUnits[i].RoleStatusDTO.RoleID);
            for (int i = 0; i < _teamIdToBattleInit[roleId].petUnits.Count
; i++)
                TargetIdSet.Add(_teamIdToBattleInit[roleId].petUnits[i].PetStatusDTO.PetID);

            return TargetIdSet;
        }

        /// <summary>
        /// 2020 11.07  18:00
        /// 选择回血的目标
        /// </summary>
        /// <param name="targetInfoDTOsSet"></param>
        /// <param name="typeName"></param>
        /// <param name="objectOwner"></param>
        /// <param name="bSD"></param>
        /// <param name="ol"></param>
        public void SelectToTarget(List<TargetInfoDTO> targetInfoDTOsSet, string typeName, object objectOwner, List<BattleSkillDamageNumData> bSD, int ol)
        {
            switch (typeName)
            {
                case "RoleStatusDTO":
                    var playerStatusDTO = objectOwner as RoleStatusDTO;
                    if (playerStatusDTO.RoleHP + bSD[0].baseNumSourceDataList[ol].mulitity >= playerStatusDTO.RoleMaxHP)
                        playerStatusDTO.RoleHP = playerStatusDTO.RoleMaxHP;
                    else
                        playerStatusDTO.RoleHP += bSD[0].baseNumSourceDataList[ol].mulitity;
                    var TargetInfosSet = ServerToClientResults(new TargetInfoDTO() { TargetID = playerStatusDTO.RoleID, TargetHPDamage = bSD[0].baseNumSourceDataList[ol].mulitity });
                    targetInfoDTOsSet.Add(TargetInfosSet);
                    break;
                case "PetStatusDTO":
                    var petStatusDTO = objectOwner as PetStatusDTO;
                    if (petStatusDTO.PetHP + bSD[0].baseNumSourceDataList[ol].mulitity >= petStatusDTO.PetMaxHP)
                        petStatusDTO.PetHP = petStatusDTO.PetMaxHP;
                    else
                        petStatusDTO.PetHP += bSD[0].baseNumSourceDataList[ol].mulitity;
                    var tempTrans = ServerToClientResults(new TargetInfoDTO() { TargetID = petStatusDTO.PetID, TargetHPDamage = bSD[0].baseNumSourceDataList[ol].mulitity });
                    targetInfoDTOsSet.Add(tempTrans);
                    break;
            }
        }
        #endregion

        #region 统一 技能分类 法宝的具体处理
        /// <summary>
        /// 针对法宝的使用
        /// </summary>
        public void PlayerToMagicWeapen(BattleTransferDTO battleTransferDTOs, int roleId, int currentId)
        {
            if (MagicWeaponFormToObject(battleTransferDTOs.ClientCmdId) == null)
                return;
            var magicOwner = MagicWeaponFormToObject(battleTransferDTOs.ClientCmdId);
            if (!IsToSkillForm(magicOwner.Magic_Skill))
                return;
            //Utility.Debug.LogInfo(" battleTransferDTOs.ClientCmdId ===》法宝指令" + battleTransferDTOs.ClientCmdId);
            battleTransferDTOs.ClientCmdId = magicOwner.Magic_Skill;
            PlayerToSKillRelease(battleTransferDTOs, roleId, currentId, magicOwner.Magic_ID);
        }
        #endregion

        #region 统一技能分类  道具的使用 符箓和丹药
        public void PlayerToPropslnstruction(BattleTransferDTO battleTransferDTOs, int roleId, int currentId)
        {
            if (PropsInstrutionFormToObject(battleTransferDTOs.ClientCmdId) == null)
                return;
            var objectOwner = PropsInstrutionFormToObject(battleTransferDTOs.ClientCmdId);
            var typeName = objectOwner.GetType().Name;
            switch (typeName)
            {
                case "DrugData":
                    var drugData = objectOwner as DrugData;
                    DrugDataToUser(battleTransferDTOs, roleId, currentId, drugData);
                    break;
                case "RunesData":
                    var runesData = objectOwner as RunesData;
                    RunesDataToUser(battleTransferDTOs, roleId, currentId, runesData);
                    break;
            }
        }


        /// <summary>
        /// 符箓的使用
        /// </summary>
        public void RunesDataToUser(BattleTransferDTO battleTransferDTOs, int roleId, int currentId, RunesData runesData)
        {
            if (!IsToSkillForm(runesData.Runes_Skill))
                return;
            battleTransferDTOs.ClientCmdId = runesData.Runes_Skill;
            PlayerToSKillRelease(battleTransferDTOs, roleId, currentId, runesData.Runes_ID);
        }

        /// <summary>
        /// 丹药的使用
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="drugData"></param>
        public void DrugDataToUser(BattleTransferDTO battleTransferDTOs, int roleId, int currentId, DrugData drugData)
        {
            switch (drugData.Drug_Type)
            {
                case DrugType.RoleHP:
                    DrugHP(battleTransferDTOs, roleId, currentId, drugData);
                    break;
                case DrugType.RoleMP:

                    break;
                case DrugType.RoleBuff:

                    break;
                case DrugType.RoleResurgence:
                    break;
            }
        }

        #region  针对丹药的HP  MP Buffer 复活
        public void DrugHP(BattleTransferDTO battleTransferDTOs, int roleId, int currentId, DrugData drugData)
        {
            List<TargetInfoDTO> targetInfoDTOsSet = new List<TargetInfoDTO>();
            var dv = drugData.Drug_Value;
            var objectOwner = ReleaseToOwner(currentId, currentId, roleId);
            var typeName = objectOwner.GetType().Name;
            //SelectToTarget(targetInfoDTOsSet, typeName, objectOwner, bSD, ol);
            switch (typeName)
            {
                case "RoleStatusDTO":
                    var playerStatusDTO = objectOwner as RoleStatusDTO;
                    if (playerStatusDTO.RoleHP + dv >= playerStatusDTO.RoleMaxHP)
                        playerStatusDTO.RoleHP = playerStatusDTO.RoleMaxHP;
                    else
                        playerStatusDTO.RoleHP += dv;
                    var TargetInfosSet = ServerToClientResults(new TargetInfoDTO() { TargetID = playerStatusDTO.RoleID, TargetHPDamage = dv });
                    targetInfoDTOsSet.Add(TargetInfosSet);
                    break;
                case "PetStatusDTO":
                    var petStatusDTO = objectOwner as PetStatusDTO;
                    if (petStatusDTO.PetHP + dv >= petStatusDTO.PetMaxHP)
                        petStatusDTO.PetHP = petStatusDTO.PetMaxHP;
                    else
                        petStatusDTO.PetHP += dv;
                    var tempTrans = ServerToClientResults(new TargetInfoDTO() { TargetID = petStatusDTO.PetID, TargetHPDamage = dv });
                    targetInfoDTOsSet.Add(tempTrans);
                    break;
            }
            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = targetInfoDTOsSet });
        }
        #endregion


        #endregion

        #region 技能分类具体处理
        /// <summary>
        /// 针对  单人逃跑的 返回计算处理
        /// </summary>
        public void PlayerToRunAway(BattleTransferDTO battleTransferDTOs, int roleId)
        {
            TargetInfoDTO tempTrans = new TargetInfoDTO();
            tempTrans.TargetID = roleId;
            tempTrans.TargetHPDamage = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP > 0 ? 1 : 0;
            if (tempTrans.TargetHPDamage == 1)
                isRunAway = true;
            List<TargetInfoDTO> TargetInfosSet = new List<TargetInfoDTO>();
            TargetInfosSet.Add(tempTrans);
            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
        }
        /// <summary>
        /// 针对 宠物逃跑的 返回计算处理
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="petId"></param>
        public void PetToRunAway(BattleTransferDTO battleTransferDTOs, int roleId, int petId)
        {
            TargetInfoDTO tempTrans = new TargetInfoDTO();
            tempTrans.TargetID = petId;
            tempTrans.TargetHPDamage = _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP > 0 ? 1 : 0;
            ///TODO 缺少同步HP
            if (tempTrans.TargetHPDamage == 1)
            {
                isPetRunAway = true;
                _teamIdToBattleInit[roleId].petUnits.RemoveAt(0);
                //_teamIdToBattleInit[roleId].battleUnits.RemoveAt(_teamIdToBattleInit[roleId].battleUnits.FindIndex(x => x.ObjectId == petId));
            }
            List<TargetInfoDTO> TargetInfosSet = new List<TargetInfoDTO>();
            TargetInfosSet.Add(tempTrans);
            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
        }


        /// <summary>
        /// 组队逃跑   可以和并成一个 和单人逃跑的   需要去队伍中标记一下 是不是存在战斗中还是中途退出啦
        /// 需要继续完善   ///TODO
        /// </summary>
        /// speed = -1 的话 代表这回合计算是宠物逃跑
        public void PlayerTeamToRunAway(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, int transfer = 0, int speed = 0)
        {
            TargetInfoDTO tempTrans = new TargetInfoDTO();
            tempTrans.TargetID = currentRole;
            if (speed == -1)
                tempTrans.TargetHPDamage = _teamIdToBattleInit[roleId].petUnits[transfer].PetStatusDTO.PetHP > 0 ? 1 : 0;
            else
                tempTrans.TargetHPDamage = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP > 0 ? 1 : 0;
            if (tempTrans.TargetHPDamage == 1)
            {
                if (speed == -1)
                {
                    isPetTeamRunAway = true;
                    _teamIdToBattleInit[roleId].petUnits.Remove(_teamIdToBattleInit[roleId].petUnits.Find(x => x.PetStatusDTO.PetID == currentRole));
                    //_teamIdToBattleInit[roleId].battleUnits.RemoveAt(_teamIdToBattleInit[roleId].battleUnits.FindIndex(x => x.ObjectId == currentRole));
                    _roomidToBattleTransfer[_teamIdToBattleInit[roleId].RoomId].RemoveAt(_roomidToBattleTransfer[_teamIdToBattleInit[roleId].RoomId].FindIndex(x => x.petBattleTransferDTO.RoleId == currentRole));
                }
                else
                {
                    isTeamRunAway++;
                    // _teamIdToBattleInit[roleId].playerUnits.RemoveAt(_teamIdToBattleInit[roleId].playerUnits.FindIndex(x => x.RoleStatusDTO.RoleID == currentRole));
                    //TODO   需要下午该一下
                    //_roomidToBattleTransfer[_teamIdToBattleInit[roleId].RoomId].RemoveAt(_roomidToBattleTransfer[_teamIdToBattleInit[roleId].RoomId].FindIndex(x => x.RoleId == currentRole));

                    //var petTempId=  _teamIdToBattleInit[roleId].petUnits.Find(x => x.RoleId == currentRole);
                    // _teamIdToBattleInit[roleId].petUnits.RemoveAt(_teamIdToBattleInit[roleId].petUnits.FindIndex(x => x.RoleId == currentRole));
                    //_teamIdToBattleInit[roleId].battleUnits.RemoveAt(_teamIdToBattleInit[roleId].battleUnits.FindIndex(x => x.ObjectId == currentRole));
                    //_teamIdToBattleInit[roleId].battleUnits.RemoveAt(_teamIdToBattleInit[roleId].battleUnits.FindIndex(x => x.ObjectId == petTempId.PetStatusDTO.PetID));
                }
            }
            List<TargetInfoDTO> TargetInfosSet = new List<TargetInfoDTO>();
            TargetInfosSet.Add(tempTrans);
            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
        }
        #endregion

        #region 统一针对 捕捉
        public void PlayerToCatchPet(BattleTransferDTO battleTransferDTOs, int roleId, int currentId, BattleSkillData battleSkillData)
        {

            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", currentId);
            var petnHCriteriaRoleID = NHibernateQuerier.CriteriaSelect<RolePet>(nHCriteriaRoleID);
            //GameManager.CustomeModule<PetStatusManager>().InitPet(battleSkillData., "", petnHCriteriaRoleID);
        }
        #endregion


    }
}
