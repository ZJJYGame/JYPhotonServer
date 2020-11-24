using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using NHibernate.Linq.Clauses;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// 处理战斗中所有单人的情况
/// </summary>
namespace AscensionServer
{
    public partial class ServerBattleManager
    {
        #region 2020. 11. 06  11:43

        /// <summary>
        /// 2020.11.06 12:00
        /// 筛选出来存活的Ai
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="info"></param>
        /// <param name="skillGongFa"></param>
        public void AlToSurvival(BattleTransferDTO battleTransferDTOs, int roleId, int info, BattleSkillData battleSkillData)
        {
            var indexNumber = 0; //_teamIdToBattleInit[roleId].enemyUnits.Count;
            int survivalNumber = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count;
            if (_teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == battleTransferDTOs.TargetInfos[info].TargetID).EnemyStatusDTO.EnemyHP > 0)
                TargetID.Add(battleTransferDTOs.TargetInfos[info].TargetID, battleTransferDTOs.TargetInfos[info].GlobalId);
            while (TargetID.Count != battleSkillData.TargetNumber)
            {
                if (TargetID.Count == survivalNumber || survivalNumber == 0)
                    break;
                //TODO 缺少判断   数量不够 的情况下
                indexNumber++;
                //var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);
                if (AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count == indexNumber - 1)
                    break;
                if (TargetID.ContainsKey(AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[indexNumber - 1].EnemyStatusDTO.EnemyId))
                    continue;

                TargetID.Add(AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[indexNumber - 1].EnemyStatusDTO.EnemyId, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[indexNumber - 1].GlobalId);
            }
        }

        /// <summary>
        ///玩家出手 释放技能
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="special"></param>
        /// <param name="petId"></param>
        public void PlayerToSKillRelease(BattleTransferDTO battleTransferDTOs, int roleId, int currentId, int special = 0)
        {
            TargetID.Clear();

            ///传输的目标
            for (int info = 0; info < battleTransferDTOs.TargetInfos.Count; info++)
            {
                if (SkillFormToObject(battleTransferDTOs.ClientCmdId) != null)
                {
                    //Utility.Debug.LogInfo("survivalNumber================ =>" + survivalNumber);
                    //TODO  需要细节的处理
                    var objectOwner = SkillFormToObject(battleTransferDTOs.ClientCmdId);

                    switch ((BattleSkillActionType)objectOwner.battleSkillActionType)
                    {
                        case BattleSkillActionType.Damage:
                            AlToSurvival(battleTransferDTOs, roleId, info, objectOwner);
                            PlayerToSkillDamage(battleTransferDTOs, roleId, currentId, objectOwner, special);
                            break;
                        case BattleSkillActionType.Heal:
                            PlayerToSkillReturnBlood(battleTransferDTOs, roleId, currentId, objectOwner);
                            ; break;
                        case BattleSkillActionType.Resurrection:
                            break;
                        case BattleSkillActionType.Summon:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 2020 . 11 . 06 13：17
        /// 针对功法  玩家释放  不同技能类型的技能计算伤害
        /// </summary>
        public void PlayerToSkillDamage(BattleTransferDTO battleTransferDTOs, int roleId, int currentId, BattleSkillData battleSkillData, int special = 0)
        {
            battleTransferDTOs.ClientCmdId = battleTransferDTOs.BattleCmd == BattleCmd.PropsInstruction || battleTransferDTOs.BattleCmd == BattleCmd.MagicWeapon ? special : battleTransferDTOs.ClientCmdId;
            buffToSkillId = battleSkillData.id;

            #region ob  TODO
            switch (battleSkillData.battleSkillTargetType)
            {
                case BattleSkillTargetType.All:
                    break;
                case BattleSkillTargetType.player:
                    break;
                case BattleSkillTargetType.Pet:
                    break;
                case BattleSkillTargetType.Self:
                    break;
                case BattleSkillTargetType.Summon:
                    break;
                default:
                    break;
            }
            #endregion
            ///判断技能触发时机
            ///技能的计算TODO 需要加判断
            ///伤害系数列表
            SkillSingleOrStaged(battleTransferDTOs, roleId, currentId, battleSkillData, special);
        }
        #endregion
    }
}
