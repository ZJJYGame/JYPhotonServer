using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using Cosmos;

namespace AscensionServer
{
    /// <summary>
    /// 战斗技能事件基类
    /// </summary>
    public class BattleSkillEventBase
    {
        public BattleCharacterEntity OwnerEntity { get { return ownerSkill.OwnerEntity; } }
        public BattleSkillBase ownerSkill;
        protected BattleSkillEventConditionBase battleSkillEventConditionBase;
        //战斗事件触发
        protected virtual void Trigger(List<BattleTransferDTO> battleTransferDTOList,BattleDamageData battleDamageData)
        {
            Utility.Debug.LogError("无事件触发");
            if (!battleSkillEventConditionBase.CanTrigger(battleDamageData))
                return;

        }
        public BattleSkillEventBase(BattleSkillBase battleSkillBase, BattleSkillEventData battleSkillEventData)
        {
            ownerSkill = battleSkillBase;

            switch (battleSkillEventData.battleSkillEventTriggerCondition)
            {
                case BattleSkillEventTriggerCondition.None:
                    break;
                case BattleSkillEventTriggerCondition.Crit:
                    battleSkillEventConditionBase = new BattleSkillEventCondition_Crit();
                    break;
                case BattleSkillEventTriggerCondition.TargetPropertyUnder:
                    battleSkillEventConditionBase = new battleSkillEventCondition_PropertyLimit(this, battleSkillEventData, false, false);
                    break;
                case BattleSkillEventTriggerCondition.TargetPropertyOver:
                    battleSkillEventConditionBase = new battleSkillEventCondition_PropertyLimit(this, battleSkillEventData, false, true);
                    break;
                case BattleSkillEventTriggerCondition.SelfPropertyUnder:
                    battleSkillEventConditionBase = new battleSkillEventCondition_PropertyLimit(this, battleSkillEventData, true, false);
                    break;
                case BattleSkillEventTriggerCondition.SelfPropertyOver:
                    battleSkillEventConditionBase = new battleSkillEventCondition_PropertyLimit(this, battleSkillEventData, true, true);
                    break;
            }

            switch (battleSkillEventData.battleSkillEventTriggerTime)
            {
                case BattleSkillEventTriggerTime.BeforeAttack:
                    battleSkillBase.SkillEventBeforeAttack += Trigger;
                    break;
                case BattleSkillEventTriggerTime.BehindAttack:
                    battleSkillBase.SkillEventBehindAttack += Trigger;
                    break;
            }
        }
    }
    //释放技能
    public class BattleSkillEvent_Skill: BattleSkillEventBase
    {
        int triggerSkillID;
        protected override void Trigger(List<BattleTransferDTO> battleTransferDTOList, BattleDamageData battleDamageData)
        {
            if (!battleSkillEventConditionBase.CanTrigger(battleDamageData))
                return;
            List<int> tempTargetList = ownerSkill.OwnerEntity.GetTargetIdList(triggerSkillID, new List<int>() { battleDamageData.TargetID });
            var tempList = ownerSkill.OwnerEntity.BattleSkillController.UseSkill(triggerSkillID, tempTargetList);
            battleTransferDTOList.AddRange(tempList);
        }

        public BattleSkillEvent_Skill(BattleSkillBase battleSkillBase, BattleSkillEventData battleSkillEventData) : base(battleSkillBase, battleSkillEventData)
        {
            triggerSkillID = battleSkillEventData.EventValue;
        }
    }
    //治疗
    public class BattleSkillEvent_Heal: BattleSkillEventBase
    {
        int healValue;
        protected override void Trigger(List<BattleTransferDTO> battleTransferDTOList,BattleDamageData battleDamageData)
        {
            if (!battleSkillEventConditionBase.CanTrigger(battleDamageData))
                return;
            int value = OwnerEntity.CharacterBattleData.MaxHp * healValue / 100;
            OwnerEntity.CharacterBattleData.ChangeProperty(BattleSkillDamageTargetProperty.Health, value);
            battleTransferDTOList[battleTransferDTOList.Count - 1].TargetInfos.Add(new TargetInfoDTO()
            {
                TargetID= OwnerEntity.UniqueID,
                TargetHPDamage= value,
            });
        }

        public BattleSkillEvent_Heal(BattleSkillBase battleSkillBase, BattleSkillEventData battleSkillEventData) : base(battleSkillBase, battleSkillEventData)
        {
            healValue = battleSkillEventData.EventValue;
        }
    }
    //技能吸血
    public class BattleSkillEvent_SuckBlood : BattleSkillEventBase
    {
        int healValue;
        protected override void Trigger(List<BattleTransferDTO> battleTransferDTOList, BattleDamageData battleDamageData)
        {
            if (!battleSkillEventConditionBase.CanTrigger(battleDamageData))
                return;
            int value = Math.Abs(battleDamageData.damageNum) * healValue / 100;
            OwnerEntity.CharacterBattleData.ChangeProperty(BattleSkillDamageTargetProperty.Health, value);
            battleTransferDTOList[battleTransferDTOList.Count - 1].TargetInfos.Add(new TargetInfoDTO()
            {
                TargetID = OwnerEntity.UniqueID,
                TargetHPDamage = value,
            });
        }

        public BattleSkillEvent_SuckBlood(BattleSkillBase battleSkillBase, BattleSkillEventData battleSkillEventData) : base(battleSkillBase, battleSkillEventData)
        {
            healValue = battleSkillEventData.EventValue;
        }
    }

    public class BattleSkillEvent_AddDamage : BattleSkillEventBase
    {
        int addDamageValue;
        protected override void Trigger(List<BattleTransferDTO> battleTransferDTOList, BattleDamageData battleDamageData)
        {
            if (!battleSkillEventConditionBase.CanTrigger(battleDamageData))
                return;
        }
        public BattleSkillEvent_AddDamage(BattleSkillBase battleSkillBase, BattleSkillEventData battleSkillEventData) : base(battleSkillBase, battleSkillEventData)
        {
            addDamageValue = battleSkillEventData.EventValue;
        }
    }
}
