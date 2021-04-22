using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class BattleBuffEvent_DamageOrHeal : BattleBuffEventBase
    {


        public override void RemoveEvent()
        {
            base.RemoveEvent();
        }

        protected override void AddTriggerEvent()
        {
            base.AddTriggerEvent();
        }

        protected override void TriggerEventMethod(BattleCharacterEntity target, BattleDamageData battleDamageData, ISkillAdditionData skillAdditionData)
        {
            base.TriggerEventMethod(target, battleDamageData, skillAdditionData);
        }

        public BattleBuffEvent_DamageOrHeal(BattleBuffEventData battleBuffEventData, BattleBuffObj battleBuffObj) : base(battleBuffEventData, battleBuffObj)
        {
        }
    }
}
