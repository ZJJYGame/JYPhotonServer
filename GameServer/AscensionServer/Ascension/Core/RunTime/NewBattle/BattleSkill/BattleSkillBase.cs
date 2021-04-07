using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;

namespace AscensionServer
{
    public class BattleSkillBase
    {
        //角色的属性
        CharacterBattleData CharacterBattleData { get { return OwnerEntity.CharacterBattleData; } }
        public BattleCharacterEntity OwnerEntity { get; protected set; }


        public int SkillID { get { return battleSkillData.id; } }

        public List<int> damageAdditionList;
        public int DamgeAddition { get { return battleSkillData.damageAddition + damageAdditionList.Sum(); } }
        public int CritProp { get; protected set; }
        public int CritDamage { get; protected set; }
        public int IgnoreDefensive { get; protected set; }

        //技能攻击段数
        public int AttackSectionNumber { get { return battleSkillData.battleSkillDamageNumDataList.Count; } }
        public AttackProcess_Type AttackProcess_Type { get { return battleSkillData.attackProcessType; } }
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

        /// <summary>
        /// 获取该技能的伤害
        /// </summary>
        /// <param name="index">第几段伤害</param>
        /// <param name="targetIndex">第几个目标</param>
        public BattleDamageData GetDamageData(int index, int targetIndex, BattleCharacterEntity target)
        {
            //判断目标是否可以作为技能目标
            if (target.HasDie)
            {
                Utility.Debug.LogError("目标已死亡");
                if (battleSkillData.battleSkillActionType == BattleSkillActionType.Damage || battleSkillData.battleSkillActionType == BattleSkillActionType.Heal)
                {
                    Utility.Debug.LogError("直接返回");
                    return null;
                }
            }


            BattleSkillDamageNumData battleSkillDamageNumData = battleSkillData.battleSkillDamageNumDataList[index];
            BattleDamageData battleDamageData = new BattleDamageData();
            battleDamageData.TargetID = target.UniqueID;
            battleDamageData.battleSkillActionType = battleSkillData.battleSkillActionType;
            battleDamageData.damageType = battleSkillDamageNumData.battleSkillDamageType;
            battleDamageData.baseDamageTargetProperty = battleSkillDamageNumData.baseNumSourceDataList[targetIndex].battleSkillDamageTargetProperty;
            //计算基础伤害初始值
            int attackValue = 0;
            if (battleSkillDamageNumData.baseDamageAdditionSourceTarget)//根据目标数值
                attackValue = (int)target.CharacterBattleData.GetProperty(battleSkillDamageNumData.baseNumSourceDataList[targetIndex].battleSkillNumSourceType, battleDamageData) * battleSkillDamageNumData.baseNumSourceDataList[targetIndex].mulitity/100 + battleSkillDamageNumData.fixedNum;
            else//根据自身数值
                attackValue = (int)CharacterBattleData.GetProperty(battleSkillDamageNumData.baseNumSourceDataList[targetIndex].battleSkillNumSourceType, battleDamageData) * battleSkillDamageNumData.baseNumSourceDataList[targetIndex].mulitity/100 + battleSkillDamageNumData.fixedNum;
            int defendValue = target.CharacterBattleData.GetProperty(battleDamageData.damageType);
            //计算忽视防御
            defendValue = defendValue * (100 - (CharacterBattleData.IgnoreDef + IgnoreDefensive)) / 100;
            defendValue = defendValue < 0 ? 0 : defendValue;
            int damageValue = 0;
            if (battleDamageData.damageType == BattleSkillDamageType.Physic || battleDamageData.damageType == BattleSkillDamageType.Magic)
                damageValue = (attackValue - defendValue) * (100 + CharacterBattleData.DamageAddition + DamgeAddition - target.CharacterBattleData.DamageDeduction) / 100;
            else if (battleDamageData.damageType == BattleSkillDamageType.Reality)
                damageValue = attackValue;
            //进行暴击判断
            bool isCrit = false;
            if (battleDamageData.damageType == BattleSkillDamageType.Physic || battleDamageData.damageType == BattleSkillDamageType.Magic)
            {
                float crictRange = 0;
                if (battleDamageData.damageType == BattleSkillDamageType.Physic)
                    crictRange = (CharacterBattleData.PhysicalCritProb - target.CharacterBattleData.ReduceCritProb) * 100;
                else if (battleDamageData.damageType == BattleSkillDamageType.Magic)
                    crictRange = (CharacterBattleData.MagicCritProb - target.CharacterBattleData.ReduceCritProb) * 100;
                int randomValue = Utility.Algorithm.CreateRandomInt(0, 10000 + 1);
                if (randomValue <= crictRange)
                    isCrit = true;
            }
            //计算暴击伤害
            if (isCrit)
            {
                int finalCritDamage = 0;
                if (battleDamageData.damageType == BattleSkillDamageType.Physic)
                    finalCritDamage = CharacterBattleData.PhysicalCritDamage - target.CharacterBattleData.ReduceCritDamage;
                else if (battleDamageData.damageType == BattleSkillDamageType.Physic)
                    finalCritDamage = CharacterBattleData.MagicCritDamage - target.CharacterBattleData.ReduceCritDamage;
                finalCritDamage = finalCritDamage < 0 ? 0 : finalCritDamage;
                damageValue = damageValue * (200 + finalCritDamage) / 100;
            }
            damageValue = damageValue <= 0 ? 1 : damageValue;
            battleDamageData.isCrit = isCrit;
            battleDamageData.damageNum = battleSkillData.battleSkillActionType==BattleSkillActionType.Damage?-damageValue:damageValue;
            //计算额外伤害初始值
            if (battleSkillDamageNumData.extraNumSourceData.Count > 0)
            {
                if (battleSkillDamageNumData.extraDamageAdditionSourceTarget)//根据目标数值
                    attackValue = (int)target.CharacterBattleData.GetProperty(battleSkillDamageNumData.extraNumSourceData[targetIndex].battleSkillNumSourceType, battleDamageData) * battleSkillDamageNumData.extraNumSourceData[targetIndex].mulitity;
                else//根据自身数值
                    attackValue = (int)CharacterBattleData.GetProperty(battleSkillDamageNumData.extraNumSourceData[targetIndex].battleSkillNumSourceType, battleDamageData) * battleSkillDamageNumData.extraNumSourceData[targetIndex].mulitity;
                battleDamageData.extraDamageNum = battleSkillData.battleSkillActionType == BattleSkillActionType.Damage ? -attackValue : attackValue; ;
            }
            return battleDamageData;
        }

        /// <summary>
        /// 攻击前事件触发
        /// </summary>
        public void TriggerSkillEventBeforeAttack()
        {

        }
        /// <summary>
        /// 攻击后事件触发
        /// </summary>
        public void TriggerSkillEventBehindAttack(List<BattleTransferDTO> battleTransferDTOList,BattleDamageData battleDamageData)
        {
            skillEventBehindAttack?.Invoke(battleTransferDTOList,battleDamageData);
        }

        public BattleSkillBase(int skillID,BattleCharacterEntity battleCharacterEntity)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, BattleSkillData>>(out var battleSkillDict);
            battleSkillData = battleSkillDict[skillID];
            OwnerEntity = battleCharacterEntity;
            damageAdditionList = new List<int>();

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
                    case BattleSkillTriggerEventType.AddCrit:
                        break;
                    case BattleSkillTriggerEventType.AddDamage:
                        break;
                    case BattleSkillTriggerEventType.AddPierce:
                        break;
                }
            }
        }
    }
}
