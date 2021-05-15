using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using Cosmos;

namespace AscensionServer
{
    public class BattleBuffEvent_DamageOrHeal : BattleBuffEventBase
    {
        //是=>伤害，否=>治疗
        bool isDamge;
        BattleSkillDamageType battleSkillDamageType;
        BuffEvent_DamageOrHeal_EffectTargetType  buffEvent_DamageOrHeal_EffectTargetType;
        //是=>作用目标是自身,否=>作用目标是目标
        bool targetIsSelf;
        //数据来源：是=>自身，否=>目标
        bool dataSource;
        BuffEvent_DamageOrHeal_SourceDataType buffEvent_DamageOrHeal_SourceDataType;
        int fixedValue;
        int percentValue;

        protected override void AddTriggerEvent()
        {
            switch (battleBuffTriggerTime)
            {
                case BattleBuffTriggerTime.RoundStart:
                    battleBuffObj.BattleController.RoundStartEvent += Trigger;
                    break;
                case BattleBuffTriggerTime.BeforeAttack:
                    owner.BattleBuffController.BeforeAttackEvent += Trigger;
                    break;
                case BattleBuffTriggerTime.BehindAttack:
                    owner.BattleBuffController.BehindAttackEvent += Trigger;
                    break;
                case BattleBuffTriggerTime.BehindUseSkill:
                    owner.BattleBuffController.BehindUseSkill += Trigger;
                    break;
                case BattleBuffTriggerTime.BeforeOnHit:
                    owner.BattleBuffController.BeforeOnHitEvent += Trigger;
                    break;
                case BattleBuffTriggerTime.BehindOnHit:
                    owner.BattleBuffController.BehindOnHitEvent += Trigger;
                    break;
                case BattleBuffTriggerTime.RoleBeforeDie:
                    owner.BattleBuffController.RoleBeforeDieEvent += Trigger;
                    break;
                case BattleBuffTriggerTime.RoleAfterDie:
                    owner.BattleBuffController.RoleAfterDieEvent += Trigger;
                    break;
                case BattleBuffTriggerTime.RoundEnd:
                    battleBuffObj.BattleController.RoundEndEvent += Trigger;
                    break;
            }
        }
        public override void RemoveEvent()
        {
            switch (battleBuffTriggerTime)
            {
                case BattleBuffTriggerTime.RoundStart:
                    battleBuffObj.BattleController.RoundStartEvent -= Trigger;
                    break;
                case BattleBuffTriggerTime.BeforeAttack:
                    owner.BattleBuffController.BeforeAttackEvent -= Trigger;
                    break;
                case BattleBuffTriggerTime.BehindAttack:
                    owner.BattleBuffController.BehindAttackEvent -= Trigger;
                    break;
                case BattleBuffTriggerTime.BehindUseSkill:
                    owner.BattleBuffController.BehindUseSkill -= Trigger;
                    break;
                case BattleBuffTriggerTime.BeforeOnHit:
                    owner.BattleBuffController.BeforeOnHitEvent -= Trigger;
                    break;
                case BattleBuffTriggerTime.BehindOnHit:
                    owner.BattleBuffController.BehindOnHitEvent -= Trigger;
                    break;
                case BattleBuffTriggerTime.RoleBeforeDie:
                    owner.BattleBuffController.RoleBeforeDieEvent -= Trigger;
                    break;
                case BattleBuffTriggerTime.RoleAfterDie:
                    owner.BattleBuffController.RoleAfterDieEvent -= Trigger;
                    break;
                case BattleBuffTriggerTime.RoundEnd:
                    battleBuffObj.BattleController.RoundEndEvent -= Trigger;
                    break;
            }
        }
        protected override void TriggerEventMethod(BattleTransferDTO battleTransferDTO, BattleCharacterEntity target, BattleDamageData battleDamageData, ISkillAdditionData skillAdditionData)
        {
            int value = 0;
            if (dataSource)
                value = owner.CharacterBattleData.GetProperty(buffEvent_DamageOrHeal_SourceDataType);
            else 
                value=target.CharacterBattleData.GetProperty(buffEvent_DamageOrHeal_SourceDataType);
            value = value * percentValue / 100 + fixedValue;
            //todo 对buff伤害值进行防御值的计算
            value = isDamge ? -value : value;
            BattleCharacterEntity targetEntity = targetIsSelf ? owner : target;
            targetEntity.CharacterBattleData.ChangeProperty(buffEvent_DamageOrHeal_EffectTargetType, value);
            if (battleTransferDTO.TargetInfos == null)
                battleTransferDTO.TargetInfos = new List<TargetInfoDTO>();
            battleTransferDTO.TargetInfos.Add(new TargetInfoDTO()
            {
                TargetID = targetEntity.UniqueID,
                TargetHPDamage = value,
            });
        }

        public BattleBuffEvent_DamageOrHeal(BattleBuffEventData battleBuffEventData, BattleBuffObj battleBuffObj) : base(battleBuffEventData, battleBuffObj)
        {
            isDamge = battleBuffEventData.flag;
            battleSkillDamageType = battleBuffEventData.BattleSkillDamageType;
            buffEvent_DamageOrHeal_EffectTargetType = battleBuffEventData.buffEvent_DamageOrHeal_EffectTargetType;
            targetIsSelf = battleBuffEventData.flag_3;
            dataSource = battleBuffEventData.flag_2;
            buffEvent_DamageOrHeal_SourceDataType = battleBuffEventData.buffEvent_DamageOrHeal_SourceDataType;
            fixedValue = battleBuffEventData.fixedValue;
            percentValue = battleBuffEventData.percentValue;
        }
    }
}
