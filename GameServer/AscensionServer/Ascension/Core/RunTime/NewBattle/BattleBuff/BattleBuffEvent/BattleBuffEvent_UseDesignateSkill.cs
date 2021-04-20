using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 使用指定技能
    /// </summary>
    public class BattleBuffEvent_UseDesignateSkill : BattleBuffEventBase
    {
        int skillID;
        int percentValue;

        protected override void TriggerEventMethod(BattleCharacterEntity target, ISkillAdditionData skillAdditionData)
        {
            base.TriggerEventMethod(target, skillAdditionData);
        }
        public BattleBuffEvent_UseDesignateSkill(BattleBuffEventData battleBuffEventData, BattleBuffObj battleBuffObj) : base(battleBuffEventData, battleBuffObj)
        {
            skillID = battleBuffEventData.id;
            percentValue = battleBuffEventData.percentValue;
        }
    }
}
