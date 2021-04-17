using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;

namespace AscensionServer
{
    public class BattleSkillBase: ISkillAdditionData
    {
        //角色的属性
        CharacterBattleData CharacterBattleData { get { return OwnerEntity.CharacterBattleData; } }
        public BattleCharacterEntity OwnerEntity { get; protected set; }


        public int SkillID { get { return battleSkillData.id; } }

        public List<int> damageAdditionList;
        public int DamgeAddition { get { return battleSkillData.damageAddition + damageAdditionList.Sum(); } }
        public List<int> critPropList;
        public int CritProp { get { return battleSkillData.critProp+critPropList.Sum(); } }
        public List<int> critDamageList;
        public int CritDamage { get { return battleSkillData.critDamage + critDamageList.Sum(); } }
        public List<int> ignoreDefensiveList;
        public int IgnoreDefensive { get { return battleSkillData.ignoreDefensive + ignoreDefensiveList.Sum(); } }

        //技能攻击段数
        public int AttackSectionNumber { get { return battleSkillData.battleSkillDamageNumDataList.Count; } }
        public AttackProcess_Type AttackProcess_Type { get { return battleSkillData.attackProcessType; } }
        //最后一个攻击值
        public int LastAttackValue { get; protected set; }
        //最后一个伤害值
        public int LastDamageValue { get; protected set; }
        //技能对应的json数据
        BattleSkillData battleSkillData;

        List<BattleSkillEventBase> battleSkillEventBaseList;
        //攻击前技能触发事件
        Action<List<BattleTransferDTO>,BattleDamageData> skillEventBeforeAttack;
        public event Action<List<BattleTransferDTO>,BattleDamageData> SkillEventBeforeAttack
        {
            add { skillEventBeforeAttack += value; }
            remove { skillEventBeforeAttack -= value; }
        }
        //攻击后技能触发事件
        Action<List<BattleTransferDTO>,BattleDamageData> skillEventBehindAttack;
        public event Action<List<BattleTransferDTO>, BattleDamageData> SkillEventBehindAttack
        {
            add { skillEventBehindAttack += value; }
            remove { skillEventBehindAttack -= value; }
        }

        public bool CanUseSkill(BattleCharacterEntity target)
        {
            if (target.HasDie)
            {
                Utility.Debug.LogError("目标已死亡");
                if (battleSkillData.battleSkillActionType == BattleSkillActionType.Damage || battleSkillData.battleSkillActionType == BattleSkillActionType.Heal)
                {
                    return false;
                }
                else return true;
            }
            else
            {
                if (battleSkillData.battleSkillActionType == BattleSkillActionType.Damage || battleSkillData.battleSkillActionType == BattleSkillActionType.Heal)
                {
                    return true;
                }
                else return false;
            }
        }

        /// <summary>
        /// 获取该技能的伤害
        /// </summary>
        /// <param name="index">第几段伤害</param>
        /// <param name="targetIndex">第几个目标</param>
        public BattleDamageData GetDamageData(int index, int targetIndex, BattleCharacterEntity target, BattleDamageData battleDamageData)
        {

            BattleSkillDamageNumData battleSkillDamageNumData = battleSkillData.battleSkillDamageNumDataList[index];
            battleDamageData.battleSkillActionType = battleSkillData.battleSkillActionType;
            battleDamageData.damageType = battleSkillDamageNumData.battleSkillDamageType;
            battleDamageData.baseDamageTargetProperty = battleSkillDamageNumData.baseNumSourceDataList[targetIndex].battleSkillDamageTargetProperty;
            //计算基础伤害初始值
            int attackValue = 0;
            if (battleSkillDamageNumData.baseDamageAdditionSourceTarget)//根据目标数值
                attackValue = (int)target.CharacterBattleData.GetProperty(battleSkillDamageNumData.baseNumSourceDataList[targetIndex].battleSkillNumSourceType, battleDamageData) * battleSkillDamageNumData.baseNumSourceDataList[targetIndex].mulitity/100 + battleSkillDamageNumData.fixedNum;
            else//根据自身数值
                attackValue = (int)CharacterBattleData.GetProperty(battleSkillDamageNumData.baseNumSourceDataList[targetIndex].battleSkillNumSourceType, battleDamageData) * battleSkillDamageNumData.baseNumSourceDataList[targetIndex].mulitity/100 + battleSkillDamageNumData.fixedNum;

            int defendValue = 0;
            if(battleSkillData.battleSkillActionType == BattleSkillActionType.Damage)
            {
                defendValue = target.CharacterBattleData.GetProperty(battleDamageData.damageType);
                //计算忽视防御
                defendValue = defendValue * (100 - (CharacterBattleData.IgnoreDef + IgnoreDefensive)) / 100;
                defendValue = defendValue < 0 ? 0 : defendValue;
            }
            int damageValue = 0;
            if (battleDamageData.damageType == BattleSkillDamageType.Physic || battleDamageData.damageType == BattleSkillDamageType.Magic)
                damageValue = (attackValue - defendValue) * (100 + CharacterBattleData.DamageAddition + DamgeAddition - target.CharacterBattleData.DamageDeduction) / 100;
            else if (battleDamageData.damageType == BattleSkillDamageType.Reality)
                damageValue = attackValue;
            //计算暴击伤害
            if (battleDamageData.isCrit)
            {
                int finalCritDamage = 0;
                if (battleDamageData.damageType == BattleSkillDamageType.Physic)
                    finalCritDamage = CharacterBattleData.PhysicalCritDamage - target.CharacterBattleData.ReduceCritDamage;
                else if (battleDamageData.damageType == BattleSkillDamageType.Magic)
                    finalCritDamage = CharacterBattleData.MagicCritDamage - target.CharacterBattleData.ReduceCritDamage;
                damageValue = damageValue * (200 + finalCritDamage+CritDamage) / 100;
            }
            damageValue = damageValue <= 0 ? 1 : damageValue;
            battleDamageData.damageNum = battleSkillData.battleSkillActionType==BattleSkillActionType.Damage?-damageValue:damageValue;

            //计算额外伤害初始值
            int extraAttackValue = 0;
            if (battleSkillDamageNumData.extraNumSourceData.Count > 0)
            {
                if (battleSkillDamageNumData.extraDamageAdditionSourceTarget)//根据目标数值
                    extraAttackValue = (int)target.CharacterBattleData.GetProperty(battleSkillDamageNumData.extraNumSourceData[targetIndex].battleSkillNumSourceType, battleDamageData) * battleSkillDamageNumData.extraNumSourceData[targetIndex].mulitity;
                else//根据自身数值
                    extraAttackValue = (int)CharacterBattleData.GetProperty(battleSkillDamageNumData.extraNumSourceData[targetIndex].battleSkillNumSourceType, battleDamageData) * battleSkillDamageNumData.extraNumSourceData[targetIndex].mulitity;
                battleDamageData.extraDamageNum = battleSkillData.battleSkillActionType == BattleSkillActionType.Damage ? -extraAttackValue : extraAttackValue; ;
            }
            LastAttackValue = attackValue;
            LastDamageValue = damageValue;
            return battleDamageData;
        }


        public BattleDamageData IsCrit(int index, BattleCharacterEntity target)
        {
            BattleDamageData battleDamageData = new BattleDamageData();
            BattleSkillDamageNumData battleSkillDamageNumData = battleSkillData.battleSkillDamageNumDataList[index];
            bool isCrit = false;
            if (battleSkillDamageNumData.battleSkillDamageType == BattleSkillDamageType.Physic || battleSkillDamageNumData.battleSkillDamageType == BattleSkillDamageType.Magic)
            {
                float crictRange = 0;
                if (battleSkillDamageNumData.battleSkillDamageType == BattleSkillDamageType.Physic)
                    crictRange = (CharacterBattleData.PhysicalCritProb - target.CharacterBattleData.ReduceCritProb) * 100;
                else if (battleSkillDamageNumData.battleSkillDamageType == BattleSkillDamageType.Magic)
                    crictRange = (CharacterBattleData.MagicCritProb - target.CharacterBattleData.ReduceCritProb) * 100;
                crictRange += CritProp*100;
                int randomValue = Utility.Algorithm.CreateRandomInt(0, 10000 + 1);
                if (randomValue <= crictRange)
                    isCrit = true;
            }
            battleDamageData.TargetID = target.UniqueID;
            battleDamageData.isCrit = isCrit;
            battleDamageData.attackSection = index;
            return battleDamageData;
        }
        
        public void AddBuff(int targetIndex,BattleDamageData battleDamageData)
        {
            for (int i = 0; i < battleSkillData.battleSkillAddBuffList.Count; i++)
            {
                BattleSkillAddBuffData battleSkillAddBuffData = battleSkillData.battleSkillAddBuffList[i];
                BattleCharacterEntity target;
                if (battleSkillAddBuffData.TargetType)//受击方
                    target = GameEntry.BattleCharacterManager.GetCharacterEntity(battleDamageData.TargetID);
                else
                    target = OwnerEntity;
                target.BattleBuffController.AddBuff(battleSkillAddBuffData, this);
            }
        }

        /// <summary>
        /// 攻击前事件触发
        /// </summary>
        public void TriggerSkillEventBeforeAttack(List<BattleTransferDTO> battleTransferDTOList, BattleDamageData battleDamageData)
        {
            skillEventBeforeAttack?.Invoke(battleTransferDTOList, battleDamageData);
        }
        /// <summary>
        /// 攻击后事件触发
        /// </summary>
        public void TriggerSkillEventBehindAttack(List<BattleTransferDTO> battleTransferDTOList,BattleDamageData battleDamageData)
        {
            skillEventBehindAttack?.Invoke(battleTransferDTOList,battleDamageData);
        }
        public void ClearSkillAddition()
        {
            damageAdditionList.Clear();
            critPropList.Clear();
            critDamageList.Clear();
            ignoreDefensiveList.Clear();
        }

        public BattleSkillBase(int skillID,BattleCharacterEntity battleCharacterEntity)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, BattleSkillData>>(out var battleSkillDict);
            battleSkillData = battleSkillDict[skillID];
            OwnerEntity = battleCharacterEntity;
            damageAdditionList = new List<int>();
            critPropList = new List<int>();
            critDamageList = new List<int>();
            ignoreDefensiveList = new List<int>();

            //添加技能事件
            battleSkillEventBaseList = new List<BattleSkillEventBase>();
            for (int i = 0; i < battleSkillData.battleSkillEventDataList.Count; i++)
            {
                switch (battleSkillData.battleSkillEventDataList[i].battleSkillTriggerEventType)
                {
                    case BattleSkillTriggerEventType.Skill:
                        battleSkillEventBaseList.Add(new BattleSkillEvent_Skill(this, battleSkillData.battleSkillEventDataList[i]));
                        break;
                    case BattleSkillTriggerEventType.Heal:
                        battleSkillEventBaseList.Add(new BattleSkillEvent_Heal(this, battleSkillData.battleSkillEventDataList[i]));
                        break;
                    case BattleSkillTriggerEventType.SuckBlood:
                        battleSkillEventBaseList.Add(new BattleSkillEvent_SuckBlood(this, battleSkillData.battleSkillEventDataList[i]));
                        break;
                    case BattleSkillTriggerEventType.AddCritProp:
                        battleSkillEventBaseList.Add(new BattleSkillEvent_AddCritProp(this, battleSkillData.battleSkillEventDataList[i]));
                        break;
                    case BattleSkillTriggerEventType.AddDamage:
                        battleSkillEventBaseList.Add(new BattleSkillEvent_AddDamage(this, battleSkillData.battleSkillEventDataList[i]));
                        break;
                    case BattleSkillTriggerEventType.AddPierce:
                        battleSkillEventBaseList.Add(new BattleSkillEvent_AddIgnoreDefence(this, battleSkillData.battleSkillEventDataList[i]));
                        break;
                    case BattleSkillTriggerEventType.AddCritDamage:
                        battleSkillEventBaseList.Add(new BattleSkillEvent_AddCritDamage(this, battleSkillData.battleSkillEventDataList[i]));
                        break;
                }
            }
        }
    }
}
