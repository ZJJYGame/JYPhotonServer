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

        #region 单人技能分类具体处理

        /// <summary>
        /// 针对单人的功法秘术   处理玩家  判断Al出手之前 是不是死亡 换取目标
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        public void PlayerToRelease(BattleTransferDTO battleTransferDTOs, int roleId, int special = 0, int petId = 0)
        {
            TargetID.Clear();
            ///传输的目标
            for (int info = 0; info < battleTransferDTOs.TargetInfos.Count; info++)
            {
                if (IsToSkillForm(battleTransferDTOs.ClientCmdId))
                {
                    //Utility.Debug.LogInfo("survivalNumber================ =>" + survivalNumber);
                    //TODO  需要细节的处理
                    var objectOwner = SkillFormToSkillObject(battleTransferDTOs.ClientCmdId);
                    var typeName = objectOwner.GetType().Name;
                    switch (typeName)
                    {
                        case "SkillGongFaDatas":
                            var skillGongFa = objectOwner as SkillGongFaDatas;
                            switch ((Skill_Type)skillGongFa.Skill_Type - 1)
                            {
                                case Skill_Type.Attact:
                                    AlToSurvival(battleTransferDTOs, roleId, info, skillGongFa);
                                    if (petId == 0)
                                        PlayerToSkillDamage(battleTransferDTOs, roleId, skillGongFa, special);
                                    else
                                        PlayerToSkillDamage(battleTransferDTOs, roleId, petId, skillGongFa, special);
                                    break;
                                case Skill_Type.ReturnBlood:
                                    PlayerToSkillReturnBlood(battleTransferDTOs, roleId, petId, skillGongFa);
                                    break;
                                case Skill_Type.Shield:
                                    break;
                                case Skill_Type.Buffer:
                                    break;
                                case Skill_Type.Resurgence:
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "SkillMiShuDatas":
                            var skillMiShu = objectOwner as SkillMiShuDatas;
                            AlToSurvival(battleTransferDTOs, roleId, info, skillMiShu);
                            PlayerToSkillDamage(battleTransferDTOs, roleId, skillMiShu);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 针对法宝的使用
        /// </summary>
        public void PlayerToMagicWeapen(BattleTransferDTO battleTransferDTOs, int roleId)
        {
            if (MagicWeaponFormToObject(battleTransferDTOs.ClientCmdId) == null)
                return;
            var magicOwner = MagicWeaponFormToObject(battleTransferDTOs.ClientCmdId);
            if (!IsToSkillForm(magicOwner.Magic_Skill))
                return;
            //Utility.Debug.LogInfo(" battleTransferDTOs.ClientCmdId ===》法宝指令" + battleTransferDTOs.ClientCmdId);
            battleTransferDTOs.ClientCmdId = magicOwner.Magic_Skill;
            PlayerToRelease(battleTransferDTOs, roleId, magicOwner.Magic_ID);
        }

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

        /// <summary>
        /// 针对  单人的道具的使用 返回计算处理 符箓和丹药
        /// </summary>
        public void PlayerToPropslnstruction(BattleTransferDTO battleTransferDTOs, int roleId, int petId = 0)
        {
            if (PropsInstrutionFormToObject(battleTransferDTOs.ClientCmdId) == null)
                return;
            var objectOwner = PropsInstrutionFormToObject(battleTransferDTOs.ClientCmdId);
            var typeName = objectOwner.GetType().Name;
            switch (typeName)
            {
                case "DrugData":
                    var drugData = objectOwner as DrugData;
                    DrugDataToUser(battleTransferDTOs, roleId, drugData, petId);
                    break;
                case "RunesData":
                    var runesData = objectOwner as RunesData;
                    RunesDataToUser(battleTransferDTOs, roleId, runesData, petId);
                    break;
            }
        }

        /// <summary>
        /// 丹药的使用
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="drugData"></param>
        public void DrugDataToUser(BattleTransferDTO battleTransferDTOs, int roleId, DrugData drugData, int petId = 0)
        {
            switch (drugData.Drug_Type)
            {
                case DrugType.RoleHP:
                    DrugHP(battleTransferDTOs, roleId, drugData, petId);
                    break;
                case DrugType.RoleMP:

                    break;
                case DrugType.RoleBuff:

                    break;

                case DrugType.RoleResurgence:
                    break;
            }
        }
        #region 单人 针对丹药的HP  MP Buffer 复活
        public void DrugHP(BattleTransferDTO battleTransferDTOs, int roleId, DrugData drugData, int petId = 0)
        {
            if (drugData.Drug_Use_Target == 1) // 需要加宠物
            {
                if (petId == 0)
                {
                    if (_teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP + drugData.Drug_Value >= _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleMaxHP)
                        _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleMaxHP;
                    else
                        _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP += drugData.Drug_Value;
                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                    tempTrans.TargetID = roleId;
                    tempTrans.TargetHPDamage = drugData.Drug_Value;
                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    TargetInfosSet.Add(tempTrans);
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                }
                else
                {
                    if (_teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP + drugData.Drug_Value >= _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetMaxHP)
                        _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP = _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetMaxHP;
                    else
                        _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP += drugData.Drug_Value;
                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                    tempTrans.TargetID = petId;
                    tempTrans.TargetHPDamage = drugData.Drug_Value;
                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    TargetInfosSet.Add(tempTrans);
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                }
            }
            else
            {
                if (petId == 0)
                {
                    if (_teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP + drugData.Drug_Value >= _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleMaxHP)
                        _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleMaxHP;
                    else
                        _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP += drugData.Drug_Value;
                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                    tempTrans.TargetID = roleId;
                    tempTrans.TargetHPDamage = drugData.Drug_Value;
                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    TargetInfosSet.Add(tempTrans);
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                }
                else
                {
                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    for (int ov = 0; ov < 2; ov++)
                    {
                        if (_teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP > 0 && ov == 0)
                        {
                            if (_teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP + drugData.Drug_Value >= _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleMaxHP)
                                _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleMaxHP;
                            else
                                _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP += drugData.Drug_Value;
                            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                            tempTrans.TargetID = roleId;
                            tempTrans.TargetHPDamage = drugData.Drug_Value;
                            TargetInfosSet.Add(tempTrans);
                        }
                        if (_teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP > 0 && ov == 1)
                        {
                            if (_teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP + drugData.Drug_Value >= _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetMaxHP)
                                _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP = _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetMaxHP;
                            else
                                _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP += drugData.Drug_Value;
                            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                            tempTrans.TargetID = petId;
                            tempTrans.TargetHPDamage = drugData.Drug_Value;

                            TargetInfosSet.Add(tempTrans);

                        }
                    }
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                }
            }
        }
        #endregion


        /// <summary>
        /// 符箓的使用
        /// </summary>
        public void RunesDataToUser(BattleTransferDTO battleTransferDTOs, int roleId, RunesData runesData, int petId = 0)
        {
            if (!IsToSkillForm(runesData.Runes_Skill))
                return;
            battleTransferDTOs.ClientCmdId = runesData.Runes_Skill;
            PlayerToRelease(battleTransferDTOs, roleId, runesData.Runes_ID, petId);
        }

        #endregion

        #region   单人 技能指令使用功法的技能
        /// <summary>
        /// 针对功法  筛选出来每回合的存活的Al
        /// </summary>
        public void AlToSurvival(BattleTransferDTO battleTransferDTOs, int roleId, int info, SkillGongFaDatas skillGongFa)
        {
            var indexNumber = 0; //_teamIdToBattleInit[roleId].enemyUnits.Count;
            int survivalNumber = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count;
            TargetID.Add(battleTransferDTOs.TargetInfos[info].TargetID, battleTransferDTOs.TargetInfos[info].GlobalId);
            while (TargetID.Count != skillGongFa.Attack_Number)
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
        /// 针对功法  玩家释放  不同技能类型的技能计算伤害
        /// </summary>
        public void PlayerToSkillDamage(BattleTransferDTO battleTransferDTOs, int roleId, SkillGongFaDatas skillGongFa, int special = 0)
        {
            battleTransferDTOs.ClientCmdId = battleTransferDTOs.BattleCmd == BattleCmd.PropsInstruction || battleTransferDTOs.BattleCmd == BattleCmd.MagicWeapon ? special : battleTransferDTOs.ClientCmdId;
            ///一段伤害     先判断数量  在判断攻击模式 最后是伤害系数
            ///单人 单段和多段伤害
            if (skillGongFa.Attack_Number == 1)
            {
                for (int k = 0; k < TargetID.Count; k++)
                {
                    //Utility.Debug.LogInfo("老陆 ，TargetID=>" + TargetID.Count);
                    for (int n = 0; n < _teamIdToBattleInit[roleId].enemyUnits.Count; n++)
                    {
                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId == TargetID.ToList()[k].Key)
                        {
                            ///换取目标打
                            if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP > 0)
                            {
                                ///判断技能的伤害系数是一个还是多个
                                if (skillGongFa.Attack_Factor.Count != 1)
                                {
                                    for (int op = 0; op < skillGongFa.Attack_Factor.Count; op++)
                                    {
                                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            break;
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[op];

                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[op];
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                        TargetInfosSet.Add(tempTrans);

                                        if (skillGongFa.Attack_Factor.Count - 1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }
                                }
                                else
                                {
                                    //需要判断 当前血量是不是满足条件
                                    _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[0];
                                    if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP = 0;
                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }
                            }
                            else
                            {
                                ///TODO  应该不完整
                                if (AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count == 0)
                                {
                                    Utility.Debug.LogError("AI  全部死亡");
                                    //BattleEnd()
                                    return;
                                }
                                ///判断技能的伤害系数是一个还是多个
                                if (skillGongFa.Attack_Factor.Count != 1)
                                {

                                    for (int op = 0; op < skillGongFa.Attack_Factor.Count; op++)
                                    {
                                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            break;
                                        var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);

                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[op];
                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[op];
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                        TargetInfosSet.Add(tempTrans);
                                        if (skillGongFa.Attack_Factor.Count - 1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }

                                }
                                else
                                {
                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);
                                    tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }

                            }
                        }
                    }
                }
            }
            ///多段伤害
            else if (skillGongFa.Attack_Number > 1)
            {
                //Utility.Debug.LogInfo("单人多个数量攻击伤害");
                ///TargetId 需要对目标出手的数量
                ///TargetID
                for (int k = 0; k < skillGongFa.Attack_Factor.Count; k++)
                {
                    if (skillGongFa.AttackProcess_Type == AttackProcess_Type.SingleUse)
                    {
                        ///判断技能伤害系数是一个还是多个
                        if (skillGongFa.Attack_Factor.Count != 1)
                        {
                            #region ob  TODO

                            /*
                                for (int ko = 0; ko < skillGongFa.Attack_Factor.Count; ko++)
                                {
                                    if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        break;
                                    _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[ko];

                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[ko];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    if (skillGongFa.Attack_Factor.Count - 1 == ko || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = RoleDTO.battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    else
                                        teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = RoleDTO.battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });

                                }*/
                            #endregion
                        }
                        else
                        {
                            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                            for (int op = 0; op < TargetID.Count; op++)
                            {
                                var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[op]);
                                survivalTarget.EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[0];
                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                tempTrans.TargetID = TargetID.Keys.ToList()[op];
                                tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                TargetInfosSet.Add(tempTrans);
                            }
                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                        }

                    }
                    else if (skillGongFa.AttackProcess_Type == AttackProcess_Type.Staged)
                    {
                        //Utility.Debug.LogInfo("单人多个数量攻击伤害" + TargetID.Count);
                        for (int n = 0; n < TargetID.Count; n++)
                        {
                            var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[n]);
                            ///判断技能伤害系数是一个还是多个
                            if (skillGongFa.Attack_Factor.Count != 1) { }
                            else
                            {
                                survivalTarget.EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[0];
                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                tempTrans.TargetID = survivalTarget.EnemyStatusDTO.EnemyId;
                                tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                TargetInfosSet.Add(tempTrans);
                                if (TargetID.Count - 1 == n)
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                else
                                    teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 针对宠物的技能计算
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="skillGongFa"></param>
        /// <param name="special"></param>
        public void PlayerToSkillDamage(BattleTransferDTO battleTransferDTOs, int roleId, int petId, SkillGongFaDatas skillGongFa, int special = 0)
        {
            battleTransferDTOs.ClientCmdId = battleTransferDTOs.BattleCmd == BattleCmd.PropsInstruction || battleTransferDTOs.BattleCmd == BattleCmd.MagicWeapon ? special : battleTransferDTOs.ClientCmdId;
            ///一段伤害     先判断数量  在判断攻击模式 最后是伤害系数
            ///单人 单段和多段伤害
            if (skillGongFa.Attack_Number == 1)
            {
                for (int k = 0; k < TargetID.Count; k++)
                {
                    //Utility.Debug.LogInfo("老陆 ，TargetID=>" + TargetID.Count);
                    for (int n = 0; n < _teamIdToBattleInit[roleId].enemyUnits.Count; n++)
                    {
                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId == TargetID.ToList()[k].Key)
                        {
                            ///换取目标打
                            if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP > 0)
                            {
                                ///判断技能的伤害系数是一个还是多个
                                if (skillGongFa.Attack_Factor.Count != 1)
                                {
                                    for (int op = 0; op < skillGongFa.Attack_Factor.Count; op++)
                                    {
                                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            break;
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[op];

                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[op];
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                        TargetInfosSet.Add(tempTrans);

                                        if (skillGongFa.Attack_Factor.Count - 1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }
                                }
                                else
                                {
                                    //需要判断 当前血量是不是满足条件
                                    _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[0];
                                    if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP = 0;
                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }
                            }
                            else
                            {
                                ///TODO  应该不完整
                                if (AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count == 0)
                                {
                                    Utility.Debug.LogError("AI  全部死亡");
                                    //BattleEnd()
                                    return;
                                }
                                ///判断技能的伤害系数是一个还是多个
                                if (skillGongFa.Attack_Factor.Count != 1)
                                {

                                    for (int op = 0; op < skillGongFa.Attack_Factor.Count; op++)
                                    {
                                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            break;
                                        var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);

                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[op];
                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[op];
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                        TargetInfosSet.Add(tempTrans);
                                        if (skillGongFa.Attack_Factor.Count - 1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }

                                }
                                else
                                {
                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);
                                    tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }

                            }
                        }
                    }
                }
            }
            ///多段伤害
            else if (skillGongFa.Attack_Number > 1)
            {
                //Utility.Debug.LogInfo("单人多个数量攻击伤害");
                ///TargetId 需要对目标出手的数量
                ///TargetID
                for (int k = 0; k < skillGongFa.Attack_Factor.Count; k++)
                {
                    if (skillGongFa.AttackProcess_Type == AttackProcess_Type.SingleUse)
                    {
                        ///判断技能伤害系数是一个还是多个
                        if (skillGongFa.Attack_Factor.Count != 1)
                        {
                            #region ob  TODO

                            /*
                                for (int ko = 0; ko < skillGongFa.Attack_Factor.Count; ko++)
                                {
                                    if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        break;
                                    _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[ko];

                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[ko];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    if (skillGongFa.Attack_Factor.Count - 1 == ko || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = RoleDTO.battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    else
                                        teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = RoleDTO.battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });

                                }*/
                            #endregion
                        }
                        else
                        {
                            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                            for (int op = 0; op < TargetID.Count; op++)
                            {
                                var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[op]);
                                survivalTarget.EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[0];
                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                tempTrans.TargetID = TargetID.Keys.ToList()[op];
                                tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                TargetInfosSet.Add(tempTrans);
                            }
                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                        }

                    }
                    else if (skillGongFa.AttackProcess_Type == AttackProcess_Type.Staged)
                    {
                        //Utility.Debug.LogInfo("单人多个数量攻击伤害" + TargetID.Count);
                        for (int n = 0; n < TargetID.Count; n++)
                        {
                            var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[n]);
                            ///判断技能伤害系数是一个还是多个
                            if (skillGongFa.Attack_Factor.Count != 1) { }
                            else
                            {
                                survivalTarget.EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[0];
                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                tempTrans.TargetID = survivalTarget.EnemyStatusDTO.EnemyId;
                                tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                TargetInfosSet.Add(tempTrans);
                                if (TargetID.Count - 1 == n)
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                else
                                    teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 技能类型治疗术   TODO 存在多个目标时加血的目标
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="skillGongFa"></param>
        public void PlayerToSkillReturnBlood(BattleTransferDTO battleTransferDTOs, int roleId, int petId, SkillGongFaDatas skillGongFa)
        {
            ///给自己回血
            if (skillGongFa.Attack_Number == 1 || petId == 0)
            {
                if (_teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP + skillGongFa.Attack_Factor[0] >= _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleMaxHP)
                    _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleMaxHP;
                else
                    _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP += skillGongFa.Attack_Factor[0];
                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                tempTrans.TargetID = roleId;
                tempTrans.TargetHPDamage = skillGongFa.Attack_Factor[0];
                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                TargetInfosSet.Add(tempTrans);
                teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });

            }///多目标回血   ??? TODO  缺少多个数量
            else if (skillGongFa.Attack_Number > 1)
            {
                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                for (int ov = 0; ov < 2; ov++)
                {
                    if (_teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP > 0 && ov == 0)
                    {
                        if (_teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP + skillGongFa.Attack_Factor[0] >= _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleMaxHP)
                            _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleMaxHP;
                        else
                            _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP += skillGongFa.Attack_Factor[0];
                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                        tempTrans.TargetID = roleId;
                        tempTrans.TargetHPDamage = skillGongFa.Attack_Factor[0];
                        TargetInfosSet.Add(tempTrans);
                    }
                    if (_teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP > 0 && ov == 1)
                    {
                        if (_teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP + skillGongFa.Attack_Factor[0] >= _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetMaxHP)
                            _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP = _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetMaxHP;
                        else
                            _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP += skillGongFa.Attack_Factor[0];
                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                        tempTrans.TargetID = petId;
                        tempTrans.TargetHPDamage = skillGongFa.Attack_Factor[0];
                        TargetInfosSet.Add(tempTrans);
                    }
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId == 0 ? roleId : petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                }
            }
        }
        #endregion

        #region 单人技能指令使用秘术的技能
        public void AlToSurvival(BattleTransferDTO battleTransferDTOs, int roleId, int info, SkillMiShuDatas skillMiShu)
        {
            var indexNumber = 0; //_teamIdToBattleInit[roleId].enemyUnits.Count;
            int survivalNumber = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count;
            TargetID.Add(battleTransferDTOs.TargetInfos[info].TargetID, battleTransferDTOs.TargetInfos[info].GlobalId);
            while (TargetID.Count != skillMiShu.Attack_Number)
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

        public void PlayerToSkillDamage(BattleTransferDTO battleTransferDTOs, int roleId, SkillMiShuDatas skillMiShu)
        {
            ///一段伤害     先判断数量  在判断攻击模式 最后是伤害系数
            ///单人 单段和多段伤害
            if (skillMiShu.Attack_Number == 1)
            {
                for (int k = 0; k < TargetID.Count; k++)
                {
                    //Utility.Debug.LogInfo("老陆 ，TargetID=>" + TargetID.Count);
                    for (int n = 0; n < _teamIdToBattleInit[roleId].enemyUnits.Count; n++)
                    {
                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId == TargetID.ToList()[k].Key)
                        {
                            ///换取目标打
                            if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP > 0)
                            {
                                ///判断技能的伤害系数是一个还是多个
                                if (skillMiShu.Attack_Factor.Count != 1)
                                {
                                    for (int op = 0; op < skillMiShu.Attack_Factor.Count; op++)
                                    {
                                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            break;
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillMiShu.Attack_Factor[op];

                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[op];
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                        TargetInfosSet.Add(tempTrans);
                                        if (skillMiShu.Attack_Factor.Count - 1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }
                                }
                                else
                                {
                                    //需要判断 当前血量是不是满足条件
                                    _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillMiShu.Attack_Factor[0];
                                    if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP = 0;
                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[0];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }
                            }
                            else
                            {
                                ///TODO  应该不完整
                                if (AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count == 0)
                                {
                                    Utility.Debug.LogError("AI  全部死亡");
                                    //BattleEnd()
                                    return;
                                }
                                ///判断技能的伤害系数是一个还是多个
                                if (skillMiShu.Attack_Factor.Count != 1)
                                {

                                    for (int op = 0; op < skillMiShu.Attack_Factor.Count; op++)
                                    {
                                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            break;
                                        var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);

                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillMiShu.Attack_Factor[op];
                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[op];
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                        TargetInfosSet.Add(tempTrans);
                                        if (skillMiShu.Attack_Factor.Count - 1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }

                                }
                                else
                                {
                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);
                                    tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[0];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }

                            }
                        }
                    }
                }
            }
            ///多段伤害
            else if (skillMiShu.Attack_Number > 1)
            {
                //Utility.Debug.LogInfo("单人多个数量攻击伤害");
                //var allEnemyHP = GameManager.CustomeModule<ServerBattleManager>()._teamIdToBattleInit[tempRoleId].enemyUnits.Find(q => q.EnemyStatusDTO.EnemyHP > 0);
                ///TargetId 需要对目标出手的数量
                ///TargetID
                for (int k = 0; k < skillMiShu.Attack_Factor.Count; k++)
                {
                    if (skillMiShu.AttackProcess_Type == AttackProcess_Type.SingleUse)
                    {
                        ///判断技能伤害系数是一个还是多个
                        if (skillMiShu.Attack_Factor.Count != 1)
                        {
                            #region ob  TODO

                            /*
                                for (int ko = 0; ko < skillMiShu.Attack_Factor.Count; ko++)
                                {
                                    if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        break;
                                    _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillMiShu.Attack_Factor[ko];

                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[ko];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    if (skillMiShu.Attack_Factor.Count - 1 == ko || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = RoleDTO.battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    else
                                        teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = RoleDTO.battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });

                                }*/
                            #endregion

                        }
                        else
                        {
                            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                            for (int op = 0; op < TargetID.Count; op++)
                            {
                                var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[op]);
                                survivalTarget.EnemyStatusDTO.EnemyHP -= skillMiShu.Attack_Factor[0];
                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                tempTrans.TargetID = TargetID.Keys.ToList()[op];
                                tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[0];
                                TargetInfosSet.Add(tempTrans);
                            }
                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                        }

                    }
                    else if (skillMiShu.AttackProcess_Type == AttackProcess_Type.Staged)
                    {
                        Utility.Debug.LogInfo("单人多个数量攻击伤害" + TargetID.Count);
                        for (int n = 0; n < TargetID.Count; n++)
                        {
                            var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[n]);
                            ///判断技能伤害系数是一个还是多个
                            if (skillMiShu.Attack_Factor.Count != 1)
                            {

                            }
                            else
                            {
                                survivalTarget.EnemyStatusDTO.EnemyHP -= skillMiShu.Attack_Factor[0];
                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                tempTrans.TargetID = survivalTarget.EnemyStatusDTO.EnemyId;
                                tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[0];
                                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                TargetInfosSet.Add(tempTrans);
                                if (TargetID.Count - 1 == n)
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                else
                                    teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                            }
                        }
                    }

                }
            }
        }
        #endregion
    }
}
