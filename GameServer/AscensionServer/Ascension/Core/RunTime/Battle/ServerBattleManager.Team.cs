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
        /// <summary>
        /// 针对 技能组队 计算  and 玩家出手
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="currentRole"></param>
        /// <param name="transfer"></param>
        /// 如果special = -1 的时候代表有宠物
        public void PlayerTeamToRelease(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, int transfer = 0, int special = 0)
        {
            //Utility.Debug.LogInfo("老陆 ，roleId =>" + battleTransferDTOs.ClientCmdId);
            TargetID.Clear();
            ///传输的目标
            for (int info = 0; info < battleTransferDTOs.TargetInfos.Count; info++)
            {
                if (IsToSkillForm(battleTransferDTOs.ClientCmdId))
                {
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
                                    PlayerTeamToSkillDamage(battleTransferDTOs, roleId, currentRole, skillGongFa, special);
                                    break;
                                case Skill_Type.ReturnBlood:
                                    PlayerTeamToSkillReturnBlood(battleTransferDTOs, roleId, currentRole, skillGongFa, transfer);
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
                            PlayerTeamToSkillDamage(battleTransferDTOs, roleId, currentRole, skillMiShu);
                            break;
                    }
                }
            }
        }

        #region 组队技能指令使用功法的技能
        /// <summary>
        /// 玩家组队技能伤害计算
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="currentRole"></param>
        /// <param name="skillGongFa"></param>
        public void PlayerTeamToSkillDamage(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, SkillGongFaDatas skillGongFa, int special = 0)
        {
            battleTransferDTOs.ClientCmdId = battleTransferDTOs.BattleCmd == BattleCmd.PropsInstruction || battleTransferDTOs.BattleCmd == BattleCmd.MagicWeapon ? special : battleTransferDTOs.ClientCmdId;
            ///一段伤害
            if (skillGongFa.Attack_Number == 1)
            {
                for (int k = 0; k < TargetID.Count; k++)
                {
                    //Utility.Debug.LogInfo("老陆 ，TargetID=>" + TargetID.Count);
                    for (int n = 0; n < _teamIdToBattleInit[roleId].enemyUnits.Count; n++)
                    {
                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId == TargetID.ToList()[k].Key)
                        {
                            //TODO
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
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }
                                }
                                else
                                {
                                    //需要判断 当前血量是不是满足条件
                                    _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[0];
                                    if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP = 0;
                                    //ProcessDamageSet.Add(skillGongFa.Attack_Factor[p]);
                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }
                            }
                            else
                            {
                                ///TODO  应该不完整
                                if (AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count == 0)
                                {
                                    Utility.Debug.LogError("AI  全部死亡");
                                    //BattleEnd()
                                    break;
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
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
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
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }
                            }
                        }
                    }
                }
            }
            ///多段伤害
            else if (skillGongFa.Attack_Number > 1)
            {
                for (int k = 0; k < skillGongFa.Attack_Factor.Count; k++)
                {
                    if (skillGongFa.AttackProcess_Type == AttackProcess_Type.SingleUse)
                    {
                        Utility.Debug.LogInfo("AI 老陆" + TargetID.Count);

                        ///判断技能伤害系数是一个还是多个
                        if (skillGongFa.Attack_Factor.Count != 1) { }
                        else
                        {
                            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                            for (int op = 0; op < TargetID.Count; op++)
                            {
                                var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[op]);
                                survivalTarget.EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[0];
                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                tempTrans.TargetID = survivalTarget.EnemyStatusDTO.EnemyId;
                                tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                TargetInfosSet.Add(tempTrans);
                            }
                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                        }
                    }
                    else if (skillGongFa.AttackProcess_Type == AttackProcess_Type.Staged)
                    {
                        Utility.Debug.LogInfo("AI 目标id" + TargetID.Count);
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
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                else
                                    teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                            }
                        }
                    }
                }
            }
        }

        public void PlayerTeamToSkillDamage(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, SkillMiShuDatas skillMiShu)
        {
            ///一段伤害
            if (skillMiShu.Attack_Number == 1)
            {
                for (int k = 0; k < TargetID.Count; k++)
                {
                    //Utility.Debug.LogInfo("老陆 ，TargetID=>" + TargetID.Count);
                    for (int n = 0; n < _teamIdToBattleInit[roleId].enemyUnits.Count; n++)
                    {
                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId == TargetID.ToList()[k].Key)
                        {
                            //TODO
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
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }
                                }
                                else
                                {
                                    //需要判断 当前血量是不是满足条件
                                    _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillMiShu.Attack_Factor[0];
                                    if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP = 0;
                                    //ProcessDamageSet.Add(skillMiShu.Attack_Factor[p]);
                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[0];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
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
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
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
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }
                            }
                        }
                    }
                }
            }
            ///多段伤害
            else if (skillMiShu.Attack_Number > 1)
            {
                for (int k = 0; k < skillMiShu.Attack_Factor.Count; k++)
                {
                    if (skillMiShu.AttackProcess_Type == AttackProcess_Type.SingleUse)
                    {
                        ///判断技能伤害系数是一个还是多个
                        if (skillMiShu.Attack_Factor.Count != 1) { }
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
                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                        }
                    }
                    else if (skillMiShu.AttackProcess_Type == AttackProcess_Type.Staged)
                    {
                        for (int n = 0; n < TargetID.Count; n++)
                        {
                            var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[n]);
                            ///判断技能伤害系数是一个还是多个
                            if (skillMiShu.Attack_Factor.Count != 1) { }
                            else
                            {
                                survivalTarget.EnemyStatusDTO.EnemyHP -= skillMiShu.Attack_Factor[0];
                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                tempTrans.TargetID = survivalTarget.EnemyStatusDTO.EnemyId;
                                tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[0];
                                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                TargetInfosSet.Add(tempTrans);
                                if (TargetID.Count - 1 == n)
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                else
                                    teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 组队回血
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="skillGongFa"></param>
        public void PlayerTeamToSkillReturnBlood(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, SkillGongFaDatas skillGongFa, int transfer)
        {
            if (skillGongFa.Attack_Number == 1)
            {
                if (_teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP + skillGongFa.Attack_Factor[0] >= _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP)
                    _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP;
                else
                    _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP += skillGongFa.Attack_Factor[0];

                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                tempTrans.TargetID = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleID;
                tempTrans.TargetHPDamage = skillGongFa.Attack_Factor[0];
                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                TargetInfosSet.Add(tempTrans);
                teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
            }
            else
            {
                if (skillGongFa.Attack_Number - 1 > PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits).Count)
                {
                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    if (_teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP + skillGongFa.Attack_Factor[0] >= _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP)
                        _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP;
                    else
                        _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP += skillGongFa.Attack_Factor[0];
                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                    tempTrans.TargetID = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleID;
                    tempTrans.TargetHPDamage = skillGongFa.Attack_Factor[0];
                    TargetInfosSet.Add(tempTrans);
                    for (int ol = 0; ol < PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits).Count; ol++)
                    {
                        var tempRoleObject = _teamIdToBattleInit[roleId].playerUnits.Find(x => x.RoleStatusDTO.RoleID == PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits)[ol].RoleStatusDTO.RoleID);
                        if (tempRoleObject.RoleStatusDTO.RoleHP + skillGongFa.Attack_Factor[0] >= tempRoleObject.RoleStatusDTO.RoleMaxHP)
                            tempRoleObject.RoleStatusDTO.RoleHP = tempRoleObject.RoleStatusDTO.RoleMaxHP;
                        else
                            tempRoleObject.RoleStatusDTO.RoleHP += skillGongFa.Attack_Factor[0];
                        BattleTransferDTO.TargetInfoDTO tempTransOl = new BattleTransferDTO.TargetInfoDTO();
                        tempTransOl.TargetID = tempRoleObject.RoleStatusDTO.RoleID;
                        tempTransOl.TargetHPDamage = skillGongFa.Attack_Factor[0];
                        TargetInfosSet.Add(tempTransOl);
                    }
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                }
                else
                {
                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    if (_teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP + skillGongFa.Attack_Factor[0] >= _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP)
                        _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP;
                    else
                        _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP += skillGongFa.Attack_Factor[0];
                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                    tempTrans.TargetID = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleID;
                    tempTrans.TargetHPDamage = skillGongFa.Attack_Factor[0];
                    TargetInfosSet.Add(tempTrans);
                    for (int ol = 0; ol < skillGongFa.Attack_Number - 1; ol++)
                    {
                        var IndexOl = new Random().Next(0, PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits).Count);
                        var tempRoleObject = _teamIdToBattleInit[roleId].playerUnits.Find(x => x.RoleStatusDTO.RoleID == PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits)[IndexOl].RoleStatusDTO.RoleID);
                        if (tempRoleObject.RoleStatusDTO.RoleHP + skillGongFa.Attack_Factor[0] >= tempRoleObject.RoleStatusDTO.RoleMaxHP)
                            tempRoleObject.RoleStatusDTO.RoleHP = tempRoleObject.RoleStatusDTO.RoleMaxHP;
                        else
                            tempRoleObject.RoleStatusDTO.RoleHP += skillGongFa.Attack_Factor[0];
                        BattleTransferDTO.TargetInfoDTO tempTransOl = new BattleTransferDTO.TargetInfoDTO();
                        tempTransOl.TargetID = tempRoleObject.RoleStatusDTO.RoleID;
                        tempTransOl.TargetHPDamage = skillGongFa.Attack_Factor[0];
                        TargetInfosSet.Add(tempTransOl);
                    }
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                }
            }
        }


        /// <summary>
        /// 组队逃跑   可以和并成一个 和单人逃跑的   需要去队伍中标记一下 是不是存在战斗中还是中途退出啦
        /// 需要继续完善
        /// </summary>
        /// speed = -1 的话 代表这回合计算是宠物逃跑
        public void PlayerTeamToRunAway(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, int transfer = 0, int speed = 0)
        {
            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
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
                }
                else
                {
                    isTeamRunAway++;
                    //TODO   需要下午该一下
                   _roomidToBattleTransfer[_teamIdToBattleInit[roleId].RoomId].RemoveAt(_roomidToBattleTransfer[_teamIdToBattleInit[roleId].RoomId].FindIndex(x => x.RoleId == currentRole));
                    _teamIdToBattleInit[roleId].playerUnits.RemoveAt(_teamIdToBattleInit[roleId].playerUnits.FindIndex(x => x.RoleStatusDTO.RoleID == currentRole));
                   var petTempId=  _teamIdToBattleInit[roleId].petUnits.Find(x => x.RoleId == currentRole);
                    _teamIdToBattleInit[roleId].petUnits.RemoveAt(_teamIdToBattleInit[roleId].petUnits.FindIndex(x => x.RoleId == currentRole));
                    //_teamIdToBattleInit[roleId].battleUnits.RemoveAt(_teamIdToBattleInit[roleId].battleUnits.FindIndex(x => x.ObjectId == currentRole));
                    //_teamIdToBattleInit[roleId].battleUnits.RemoveAt(_teamIdToBattleInit[roleId].battleUnits.FindIndex(x => x.ObjectId == petTempId.PetStatusDTO.PetID));
                }
            }
            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
            TargetInfosSet.Add(tempTrans);
            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
        }

        /// <summary>
        /// 针对 组队的法宝使用
        /// </summary>
        public void PlayerTeamToMagicWeapon(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, int transfer)
        {
            if (MagicWeaponFormToObject(battleTransferDTOs.ClientCmdId) == null)
                return;
            var magicOwner = MagicWeaponFormToObject(battleTransferDTOs.ClientCmdId);
            if (!IsToSkillForm(magicOwner.Magic_Skill))
                return;
            battleTransferDTOs.ClientCmdId = magicOwner.Magic_Skill;
            PlayerTeamToRelease(battleTransferDTOs, roleId, currentRole, transfer, magicOwner.Magic_ID);
        }

        /// <summary>
        /// 针对 组队道具 符箓和丹药
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        public void PlayerTeamToPropslnstruction(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, int transfer)
        {
            if (PropsInstrutionFormToObject(battleTransferDTOs.ClientCmdId) == null)
                return;
            var objectOwner = PropsInstrutionFormToObject(battleTransferDTOs.ClientCmdId);
            var typeName = objectOwner.GetType().Name;
            switch (typeName)
            {
                case "DrugData":
                    var drugData = objectOwner as DrugData;
                    DrugDataTeamToUser(battleTransferDTOs, roleId, currentRole, transfer, drugData);
                    break;
                case "RunesData":
                    var runesData = objectOwner as RunesData;
                    RunesDataTeamToUser(battleTransferDTOs, roleId, currentRole, runesData, transfer);
                    break;
            }
        }

        /// <summary>
        /// 针对组队的符箓的使用
        /// </summary>
        public void RunesDataTeamToUser(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, RunesData runesData, int transfer)
        {
            if (!IsToSkillForm(runesData.Runes_Skill))
                return;
            battleTransferDTOs.ClientCmdId = runesData.Runes_Skill;
            PlayerTeamToRelease(battleTransferDTOs, roleId, currentRole, transfer, runesData.Runes_ID);
        }


        /// <summary>
        /// 针对组队的丹药的使用
        /// </summary>
        public void DrugDataTeamToUser(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, int transfer, DrugData drugData)
        {
            switch (drugData.Drug_Type)
            {
                case DrugType.RoleHP:
                    DrugTeamHP(battleTransferDTOs, roleId, currentRole, transfer, drugData);
                    break;
                case DrugType.RoleMP:

                    break;
                case DrugType.RoleBuff:

                    break;

                case DrugType.RoleResurgence:
                    break;
            }
        }
        /// <summary>
        /// 针对丹药的HP  回血    //缺少宠物的判断
        /// </summary>
        public void DrugTeamHP(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, int transfer, DrugData drugData)
        {
            if (drugData.Drug_Use_Target == 1)
            {
                if (_teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP + drugData.Drug_Value >= _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP)
                    _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP;
                else
                    _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP += drugData.Drug_Value;
                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                tempTrans.TargetID = currentRole;
                tempTrans.TargetHPDamage = drugData.Drug_Value;
                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                TargetInfosSet.Add(tempTrans);
                teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
            }
            else
            {
                if (drugData.Drug_Use_Target >= PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits).Count)
                {
                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();

                    for (int ot = 0; ot < _teamIdToBattleInit[roleId].playerUnits.Count; ot++)
                    {
                        if (_teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleHP > 0)
                        {
                            if (_teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleHP + drugData.Drug_Value >= _teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleMaxHP)
                                _teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleMaxHP;
                            else
                                _teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleHP += drugData.Drug_Value;
                            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                            tempTrans.TargetID = _teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleID;
                            tempTrans.TargetHPDamage = drugData.Drug_Value;
                            TargetInfosSet.Add(tempTrans);
                        }
                    }
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                }
                else
                {
                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();

                    if (_teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP + drugData.Drug_Value >= _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP)
                        _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP;
                    else
                        _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP += drugData.Drug_Value;
                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                    tempTrans.TargetID = currentRole;
                    tempTrans.TargetHPDamage = drugData.Drug_Value;
                    TargetInfosSet.Add(tempTrans);
                    for (int oc = 0; oc < drugData.Drug_Use_Target - 1; oc++)
                    {
                        //PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits).Count
                        var IndexOl = new Random().Next(0, PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits).Count);
                        var tempRoleObject = _teamIdToBattleInit[roleId].playerUnits.Find(x => x.RoleStatusDTO.RoleID == PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits)[IndexOl].RoleStatusDTO.RoleID);
                        if (tempRoleObject.RoleStatusDTO.RoleHP + drugData.Drug_Value >= tempRoleObject.RoleStatusDTO.RoleMaxHP)
                            tempRoleObject.RoleStatusDTO.RoleHP = tempRoleObject.RoleStatusDTO.RoleMaxHP;
                        else
                            tempRoleObject.RoleStatusDTO.RoleHP += drugData.Drug_Value;
                        BattleTransferDTO.TargetInfoDTO tempTransOl = new BattleTransferDTO.TargetInfoDTO();
                        tempTransOl.TargetID = tempRoleObject.RoleStatusDTO.RoleID;
                        tempTransOl.TargetHPDamage = drugData.Drug_Value;
                        TargetInfosSet.Add(tempTransOl);
                    }
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                }
            }
        }

        #endregion
    }
}
