﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;

namespace AscensionServer
{
    /// <summary>
    /// 战斗角色技能控制器,每个角色实体持有，用来管理角色技能
    /// </summary>
    public class BattleSkillController
    {
        //控制器拥有者
        BattleCharacterEntity owner;
        //角色技能对象缓存,key=>技能id,value=>技能对象
        Dictionary<int, BattleSkillBase> skillDict;
        //角色拥有的技能id集合
        HashSet<int> roleHasSkillHash;

        public List<BattleTransferDTO> UseSkill()
        {
            List<BattleTransferDTO> battleTransferDTOList = new List<BattleTransferDTO>();
            int skillID = owner.ActionID;
            if (!roleHasSkillHash.Contains(skillID))
            {
                Utility.Debug.LogError("角色不包含此技能=>"+skillID);
                return battleTransferDTOList; ;
            }
            if (!skillDict.ContainsKey(skillID))
                skillDict[skillID] = new BattleSkillBase(skillID, owner.CharacterBattleData);
            BattleSkillBase battleSkill = skillDict[skillID];
            //获取技能释放目标的实体
            List<BattleCharacterEntity> targetCharacterList = new List<BattleCharacterEntity>();
            for (int i = 0; i < owner.TargetIDList.Count; i++)
            {
                targetCharacterList.Add(GameEntry.BattleCharacterManager.GetCharacterEntity(owner.TargetIDList[i]));
            }

            if (battleSkill.AttackProcess_Type == AttackProcess_Type.SingleUse)//所有目标的伤害一次性打完
            {
                for (int i = 0; i < battleSkill.AttackSectionNumber; i++)
                {
                    List<BattleDamageData> battleDamageDataList = new List<BattleDamageData>();
                    BattleTransferDTO battleTransferDTO = new BattleTransferDTO();
                    for (int j = 0; j < targetCharacterList.Count; j++)
                    {
                        BattleDamageData battleDamageData = battleSkill.GetDamageData(i, j, targetCharacterList[j]);
                        if (battleDamageData != null)
                            battleDamageDataList.Add(battleDamageData);
                    }
                    //todo伤害结算
                    for (int j = 0; j < battleDamageDataList.Count; j++)
                    {
                        GameEntry.BattleCharacterManager.GetCharacterEntity(battleDamageDataList[j].TargetID).OnActionEffect(battleDamageDataList[j]);
                    }
                    battleTransferDTO.TargetInfos= GetTargetInfoDTOList(battleDamageDataList);
                    battleTransferDTO.RoleId = owner.UniqueID;
                    battleTransferDTO.BattleCmd = BattleCmd.SkillInstruction;
                    battleTransferDTO.ClientCmdId = battleSkill.SkillID;
                    if (battleTransferDTO.TargetInfos != null)
                        battleTransferDTOList.Add(battleTransferDTO);
                }
                //todo受击后反应
                if (battleTransferDTOList.Count > 1)
                    battleTransferDTOList[battleTransferDTOList.Count - 1].isFinish = true;
                else if (battleTransferDTOList.Count == 1)
                    battleTransferDTOList[0].isFinish = true;
                return battleTransferDTOList;
            }
            else//所有目标的伤害分阶段打完
            {
                for (int i = 0; i < battleSkill.AttackSectionNumber; i++)
                {
                    List<BattleDamageData> battleDamageDataList = new List<BattleDamageData>();
                   
                    for (int j = 0; j < targetCharacterList.Count; j++)
                    {
                        battleDamageDataList.Clear();
                        BattleTransferDTO battleTransferDTO = new BattleTransferDTO();
                        BattleDamageData battleDamageData = battleSkill.GetDamageData(i, j, targetCharacterList[j]);
                        Utility.Debug.LogError(battleDamageData.baseDamageTargetProperty);
                        if (battleDamageData != null)
                            battleDamageDataList.Add(battleDamageData);
                        GameEntry.BattleCharacterManager.GetCharacterEntity(battleDamageData.TargetID).OnActionEffect(battleDamageData);
                        battleTransferDTO.TargetInfos = GetTargetInfoDTOList(battleDamageDataList);
                        battleTransferDTO.RoleId = owner.UniqueID;
                        battleTransferDTO.BattleCmd = BattleCmd.SkillInstruction;
                        battleTransferDTO.ClientCmdId = battleSkill.SkillID;
                        if (battleTransferDTO.TargetInfos != null)
                            battleTransferDTOList.Add(battleTransferDTO);
                    }


                }

                return battleTransferDTOList;
            }
        }

        //将伤害数据转换为发送数据
        List<TargetInfoDTO> GetTargetInfoDTOList(List<BattleDamageData> battleDamageDataList)
        {
            List<TargetInfoDTO> targetInfoDTOList = new List<TargetInfoDTO>();
            TargetInfoDTO targetInfoDTO;
            for (int i = 0; i < battleDamageDataList.Count; i++)
            {
                targetInfoDTO = new TargetInfoDTO();
                targetInfoDTO.TargetID = battleDamageDataList[i].TargetID;
                switch (battleDamageDataList[i].baseDamageTargetProperty)
                {
                    case BattleSkillDamageTargetProperty.Health:
                        targetInfoDTO.TargetHPDamage = battleDamageDataList[i].damageNum;
                        break;
                    case BattleSkillDamageTargetProperty.ShenHun:
                        targetInfoDTO.TargetMPDamage = battleDamageDataList[i].damageNum;
                        break;
                    case BattleSkillDamageTargetProperty.ZhenYuan:
                        targetInfoDTO.TargetHPDamage = battleDamageDataList[i].damageNum;
                        break;
                }
                targetInfoDTOList.Add(targetInfoDTO);
            }
            if (targetInfoDTOList.Count != 0)
                return targetInfoDTOList;
            else return null;
        }

        public BattleSkillController(BattleCharacterEntity owner)
        {
            this.owner = owner;
            skillDict = new Dictionary<int, BattleSkillBase>();
            roleHasSkillHash = new HashSet<int>() { 21001,21005,21006, 21402 };
        }
    }
}
