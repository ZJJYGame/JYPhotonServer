using System;
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

        public bool HasSkill(int skillID)
        {
            return roleHasSkillHash.Contains(skillID);
        }

        public List<BattleTransferDTO> UseSkill(int skillID,List<int> targetIdList)
        {
            List<BattleTransferDTO> battleTransferDTOList = new List<BattleTransferDTO>();

            if (!skillDict.ContainsKey(skillID))
                skillDict[skillID] = new BattleSkillBase(skillID, owner);
            BattleSkillBase battleSkill = skillDict[skillID];
            //todo释放条件是否满足
            //todo消耗的判断与扣除

            //获取技能释放目标的实体
            List<BattleCharacterEntity> targetCharacterList = new List<BattleCharacterEntity>();
            for (int i = 0; i < targetIdList.Count; i++)
            {
                targetCharacterList.Add(GameEntry.BattleCharacterManager.GetCharacterEntity(targetIdList[i]));
            }
            //使用技能前buff事件处理
            owner.BattleBuffController.beforeUseSkill?.Invoke();

            if (battleSkill.AttackProcess_Type == AttackProcess_Type.SingleUse)//所有目标的伤害一次性打完
            {
                for (int i = 0; i < battleSkill.AttackSectionNumber; i++)
                {
                    //判断所有目标是否全部死亡
                    bool flag = false;
                    for (int j = 0; j < targetCharacterList.Count; j++)
                    {
                        flag = battleSkill.CanUseSkill(targetCharacterList[j]) || flag;
                    }
                    if (!flag)
                        continue;

                    List<BattleDamageData> battleDamageDataList = new List<BattleDamageData>();
                    BattleTransferDTO battleTransferDTO = new BattleTransferDTO();
                    battleTransferDTOList.Add(battleTransferDTO);
                    //每段攻击前buff事件处理
                    owner.BattleBuffController.beforeAttackEvent?.Invoke();
                    for (int j = 0; j < targetCharacterList.Count; j++)
                    {
                        if (!battleSkill.CanUseSkill(targetCharacterList[j]))
                            continue;
                        //技能加成重置
                        battleSkill.ClearSkillAddition();
                        //暴击判断
                        BattleDamageData battleDamageData = battleSkill.IsCrit(i, targetCharacterList[j]);
                        //攻击前技能事件触发
                        battleSkill.TriggerSkillEventBeforeAttack(battleTransferDTOList, battleDamageData);
                        //获得伤害数据
                        battleDamageData = battleSkill.GetDamageData(i, j, targetCharacterList[j], battleDamageData);
                        if (battleDamageData != null)
                            battleDamageDataList.Add(battleDamageData);
                    }

                    //受击前buff事件处理
                    for (int j = 0; j < targetCharacterList.Count; j++)
                    {
                        targetCharacterList[j].BattleBuffController.beforeOnHitEvent?.Invoke();
                    }

                    //todo伤害结算
                    for (int j = 0; j < battleDamageDataList.Count; j++)
                    {
                        GameEntry.BattleCharacterManager.GetCharacterEntity(battleDamageDataList[j].TargetID).OnActionEffect(battleDamageDataList[j]);
                    }
                    //添加buff
                    for (int j = 0; j < battleDamageDataList.Count; j++)
                    {
                        battleSkill.AddBuff(j, battleDamageDataList[j]);
                    }
                    List<TargetInfoDTO> targetInfoDTOs= GetTargetInfoDTOList(battleDamageDataList);
                    if (battleTransferDTO.TargetInfos != null)
                        battleTransferDTO.TargetInfos.AddRange(targetInfoDTOs);
                    else
                        battleTransferDTO.TargetInfos = targetInfoDTOs;
                    battleTransferDTO.RoleId = owner.UniqueID;
                    battleTransferDTO.BattleCmd = BattleCmd.SkillInstruction;
                    battleTransferDTO.ClientCmdId = battleSkill.SkillID;
                    if (battleTransferDTO.TargetInfos == null)
                        battleTransferDTOList.Remove(battleTransferDTO);

                    //受击后buff事件处理
                    for (int j = 0; j < targetCharacterList.Count; j++)
                    {
                        targetCharacterList[j].BattleBuffController.behindOnHitEvent?.Invoke();
                    }

                    //每段攻击后buff事件结算
                    owner.BattleBuffController.behindAttackEvent?.Invoke();
                    for (int j = 0; j < battleDamageDataList.Count; j++)
                    {
                        //攻击后技能事件结算
                        battleSkill.TriggerSkillEventBehindAttack(battleTransferDTOList, battleDamageDataList[j]);
                    }
                }
                //todo受击后反应

                //使用技能后buff事件处理
                owner.BattleBuffController.behindUseSkill?.Invoke();
                return battleTransferDTOList;
            }
            else//所有目标的伤害分阶段打完
            {
                for (int i = 0; i < battleSkill.AttackSectionNumber; i++)
                {
                    List<BattleDamageData> battleDamageDataList = new List<BattleDamageData>();
                   
                    for (int j = 0; j < targetCharacterList.Count; j++)
                    {
                        if (!battleSkill.CanUseSkill(targetCharacterList[j]))
                            continue;

                        BattleTransferDTO battleTransferDTO = new BattleTransferDTO();
                        battleTransferDTOList.Add(battleTransferDTO);
                        //每段攻击前buff事件处理
                        owner.BattleBuffController.beforeAttackEvent?.Invoke();

                        battleSkill.ClearSkillAddition();
                        BattleDamageData battleDamageData = battleSkill.IsCrit(i, targetCharacterList[j]);
                        battleSkill.TriggerSkillEventBeforeAttack(battleTransferDTOList, battleDamageData);
                        battleDamageData = battleSkill.GetDamageData(i, j, targetCharacterList[j], battleDamageData);
                        if (battleDamageData == null)//根本没有打，移除传输事件
                        {
                            battleTransferDTOList.Remove(battleTransferDTO);
                            continue;
                        }
                        //受击前buff事件处理
                        targetCharacterList[j].BattleBuffController.beforeOnHitEvent?.Invoke();

                        battleDamageDataList.Add(battleDamageData);
                        GameEntry.BattleCharacterManager.GetCharacterEntity(battleDamageData.TargetID).OnActionEffect(battleDamageData);


                        battleSkill.AddBuff(j, battleDamageData);

                        List<TargetInfoDTO> targetInfoDTOs= GetTargetInfoDTOList(new List<BattleDamageData>() { battleDamageData });
                        if (battleTransferDTO.TargetInfos != null)
                            battleTransferDTO.TargetInfos.AddRange(targetInfoDTOs);
                        else
                            battleTransferDTO.TargetInfos = targetInfoDTOs;
                        battleTransferDTO.RoleId = owner.UniqueID;
                        battleTransferDTO.BattleCmd = BattleCmd.SkillInstruction;
                        battleTransferDTO.ClientCmdId = battleSkill.SkillID;
                        if (battleTransferDTO.TargetInfos == null)
                            battleTransferDTOList.Remove(battleTransferDTO);

                        Utility.Debug.LogError(Utility.Json.ToJson(battleTransferDTO));

                        //受击后buff事件处理
                        targetCharacterList[j].BattleBuffController.behindOnHitEvent?.Invoke();

                        //每段攻击后buff事件结算
                        owner.BattleBuffController.behindAttackEvent?.Invoke();

                        battleSkill.TriggerSkillEventBehindAttack(battleTransferDTOList, battleDamageData);
                    }


                }

                //使用技能后buff事件处理
                owner.BattleBuffController.behindAttackEvent?.Invoke();

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
            roleHasSkillHash = new HashSet<int>() { 21001,21005,21006, 21402 ,21202};
        }
    }
}
