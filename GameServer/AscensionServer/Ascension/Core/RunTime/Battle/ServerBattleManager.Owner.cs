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

            ///技能的计算TODO 需要加判断
            SkillSingleOrStaged(battleTransferDTOs, roleId, currentId, battleSkillData, special);

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


            #region TODO
            var eventData = battleSkillData.battleSkillEventDataList;
            for (int ov = 0; ov < eventData.Count; ov++)
            {
                switch (eventData[ov].battleSkillEventTriggerTime)
                {
                    case BattleSkillEventTriggerTime.BeforeAttack:

                        break;
                    case BattleSkillEventTriggerTime.BehindAttack:
                        break;
                }

                switch (eventData[ov].battleSkillEventTriggerCondition)
                {
                    case BattleSkillEventTriggerCondition.None:
                        break;
                    case BattleSkillEventTriggerCondition.Crit:
                        break;
                    case BattleSkillEventTriggerCondition.TargetPropertyUnder:
                        break;
                    case BattleSkillEventTriggerCondition.TargetPropertyOver:
                        break;
                    case BattleSkillEventTriggerCondition.SelfPropertyUnder:
                        break;
                    case BattleSkillEventTriggerCondition.SelfPropertyOver:
                        break;
                }

                switch (eventData[ov].battleSkillEventTriggerNumSourceType)
                {
                    case BattleSkillEventTriggerNumSourceType.Health:
                        break;
                    case BattleSkillEventTriggerNumSourceType.PhysicDefense:
                        break;
                    case BattleSkillEventTriggerNumSourceType.MagicDefense:
                        break;
                    case BattleSkillEventTriggerNumSourceType.ShenHun:
                        break;
                }

                switch (eventData[ov].battleSkillTriggerEventType)
                {
                    case BattleSkillTriggerEventType.Skill:
                        break;
                    case BattleSkillTriggerEventType.Heal:
                        break;
                    case BattleSkillTriggerEventType.SuckBlood:
                        break;
                    case BattleSkillTriggerEventType.AddCrit:
                        break;
                    case BattleSkillTriggerEventType.AddDamage:
                        break;
                    case BattleSkillTriggerEventType.AddPierce:
                        break;
                }
                #endregion

                ///TODO 需要完善
            }
        }

        #region 统一技能的修改

        /// <summary>
        /// 2020.11.06 20:06
        /// 统一计算技能 单段 多段
        /// </summary>
        public void SkillSingleOrStaged(BattleTransferDTO battleTransferDTOs, int roleId, int currentId, BattleSkillData battleSkillData, int special = 0)
        {
            var enemySet = _teamIdToBattleInit[roleId].enemyUnits;
            var skillDataDamageNum = battleSkillData.battleSkillDamageNumDataList;
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
                            enemySetObject.EnemyStatusDTO.EnemyHP -= NumSource[ox].mulitity;

                            var TargetInfosSet = ServerToClientResult(new BattleTransferDTO.TargetInfoDTO() { TargetID = enemySetObject.EnemyStatusDTO.EnemyId, TargetHPDamage = -NumSource[ox].mulitity });
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
                            enemySetObject.EnemyStatusDTO.EnemyHP -= NumSource[ox].mulitity;
                            var TargetInfosSet = ServerToClientResult(new BattleTransferDTO.TargetInfoDTO() { TargetID = enemySet[RandomTarget].EnemyStatusDTO.EnemyId, TargetHPDamage = -NumSource[ox].mulitity });
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
                                servivalTarget.EnemyStatusDTO.EnemyHP -= skillDataDamageNum[op].baseNumSourceDataList[zo].mulitity;
                                var tranfsSet = ServerToClientResult(new BattleTransferDTO.TargetInfoDTO() { TargetID = servivalTarget.EnemyStatusDTO.EnemyId, TargetHPDamage = -skillDataDamageNum[op].baseNumSourceDataList[zo].mulitity });
                                if (skillDataDamageNum.Count - 1 == op)
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = tranfsSet });
                                else
                                    teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = tranfsSet });
                            }
                        }
                    }
                    else
                    {
                        List<BattleTransferDTO.TargetInfoDTO> targetInfoDTOsSet = new List<BattleTransferDTO.TargetInfoDTO>();
                        for (int zo = 0; zo < TargetID.Count; zo++)
                        {
                            var servivalTarget = enemySet.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[zo]);
                            servivalTarget.EnemyStatusDTO.EnemyHP -= skillDataDamageNum[0].baseNumSourceDataList[zo].mulitity;
                            var tranfsSet = ServerToClientResults(new BattleTransferDTO.TargetInfoDTO() { TargetID = servivalTarget.EnemyStatusDTO.EnemyId, TargetHPDamage = -skillDataDamageNum[0].baseNumSourceDataList[zo].mulitity });
                            targetInfoDTOsSet.Add(tranfsSet);
                        }
                        teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = targetInfoDTOsSet });
                    }
                }
                else if (battleSkillData.attackProcessType == AttackProcess_Type.Staged)
                {
                    //Utility.Debug.LogInfo("target===>>>>Staged>>>" + TargetID.Count);
                    //for (int op = 0; op < skillDataDamageNum.Count; op++)
                    {
                        for (int n = 0; n < TargetID.Count; n++)
                        {
                            var survivalTarget = enemySet.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[n]);
                            survivalTarget.EnemyStatusDTO.EnemyHP -= skillDataDamageNum[0].baseNumSourceDataList[n].mulitity;
                            var TargetInfosSet = ServerToClientResult(new BattleTransferDTO.TargetInfoDTO() { TargetID = survivalTarget.EnemyStatusDTO.EnemyId, TargetHPDamage = -skillDataDamageNum[0].baseNumSourceDataList[n].mulitity });
                            if (TargetID.Count - 1 == n)
                                teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                            else
                                teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                        }
                    }
                }
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

                var TargetInfosSet = ServerToClientResult(new BattleTransferDTO.TargetInfoDTO() { TargetID = currentId, TargetHPDamage = bSD[0].baseNumSourceDataList[0].mulitity });
                teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
            }
            else
            {
                var tempSet = PlayerToPetID(roleId);
                List<BattleTransferDTO.TargetInfoDTO> targetInfoDTOsSet = new List<BattleTransferDTO.TargetInfoDTO>();
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
        public void SelectToTarget(List<BattleTransferDTO.TargetInfoDTO> targetInfoDTOsSet, string typeName, object objectOwner, List<BattleSkillDamageNumData> bSD, int ol)
        {
            switch (typeName)
            {
                case "RoleStatusDTO":
                    var playerStatusDTO = objectOwner as RoleStatusDTO;
                    if (playerStatusDTO.RoleHP + bSD[0].baseNumSourceDataList[ol].mulitity >= playerStatusDTO.RoleMaxHP)
                        playerStatusDTO.RoleHP = playerStatusDTO.RoleMaxHP;
                    else
                        playerStatusDTO.RoleHP += bSD[0].baseNumSourceDataList[ol].mulitity;
                    var TargetInfosSet = ServerToClientResults(new BattleTransferDTO.TargetInfoDTO() { TargetID = playerStatusDTO.RoleID, TargetHPDamage = bSD[0].baseNumSourceDataList[ol].mulitity });
                    targetInfoDTOsSet.Add(TargetInfosSet);
                    break;
                case "PetStatusDTO":
                    var petStatusDTO = objectOwner as PetStatusDTO;
                    if (petStatusDTO.PetHP + bSD[0].baseNumSourceDataList[ol].mulitity >= petStatusDTO.PetMaxHP)
                        petStatusDTO.PetHP = petStatusDTO.PetMaxHP;
                    else
                        petStatusDTO.PetHP += bSD[0].baseNumSourceDataList[ol].mulitity;
                    var tempTrans = ServerToClientResults(new BattleTransferDTO.TargetInfoDTO() { TargetID = petStatusDTO.PetID, TargetHPDamage = bSD[0].baseNumSourceDataList[ol].mulitity });
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
        public void DrugDataToUser(BattleTransferDTO battleTransferDTOs, int roleId, int currentId,DrugData drugData)
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
            List<BattleTransferDTO.TargetInfoDTO> targetInfoDTOsSet = new List<BattleTransferDTO.TargetInfoDTO>();
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
                    var TargetInfosSet = ServerToClientResults(new BattleTransferDTO.TargetInfoDTO() { TargetID = playerStatusDTO.RoleID, TargetHPDamage = dv });
                    targetInfoDTOsSet.Add(TargetInfosSet);
                    break;
                case "PetStatusDTO":
                    var petStatusDTO = objectOwner as PetStatusDTO;
                    if (petStatusDTO.PetHP + dv >= petStatusDTO.PetMaxHP)
                        petStatusDTO.PetHP = petStatusDTO.PetMaxHP;
                    else
                        petStatusDTO.PetHP += dv;
                    var tempTrans = ServerToClientResults(new BattleTransferDTO.TargetInfoDTO() { TargetID = petStatusDTO.PetID, TargetHPDamage = dv });
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
            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
            tempTrans.TargetID = roleId;
            tempTrans.TargetHPDamage = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP > 0 ? 1 : 0;
            if (tempTrans.TargetHPDamage == 1)
                isRunAway = true;
            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
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
            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
            tempTrans.TargetID = petId;
            tempTrans.TargetHPDamage = _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP > 0 ? 1 : 0;
            ///TODO 缺少同步HP
            if (tempTrans.TargetHPDamage == 1)
            {
                isPetRunAway = true;
                _teamIdToBattleInit[roleId].petUnits.RemoveAt(0);
                //_teamIdToBattleInit[roleId].battleUnits.RemoveAt(_teamIdToBattleInit[roleId].battleUnits.FindIndex(x => x.ObjectId == petId));
            }
            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
            TargetInfosSet.Add(tempTrans);
            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
        }

        #endregion

        #endregion
    }
}
