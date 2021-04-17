using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

namespace AscensionServer
{
    public class BattleBuffEventBase
    {
        protected BattleCharacterEntity owner;
        protected BattleBuffObj battleBuffObj;
        int prob;
        BattleBuffEventConditionBase battleBuffEventConditionBase;
        protected BattleBuffTriggerTime battleBuffTriggerTime;
        protected int triggerCount;
        protected int maxTriggerCount;

        protected int OverlayLayer { get { return battleBuffObj.OverlayLayer; } }

        bool CanTrigger()
        {
            if (!battleBuffObj.CanTrigger())
                return false;
            int randomValue = Utility.Algorithm.CreateRandomInt(0, 100);
            if (randomValue >= prob)
                return false;
            return battleBuffEventConditionBase.CanTrigger();
        }
        public void Trigger()
        {

            //todo 暂时设置为必定触发
            //if (!CanTrigger())
            //    return;
            //暂时设置为血量低于50%触发
            //int healthPercent = owner.CharacterBattleData.GetPropertyPercent(BattleSkillEventTriggerNumSourceType.Health);
            //if (healthPercent > 50)
            //    return;
            //else//触发条件不满足尝试恢复
            //{
            //    Recover();
            //}
            //最大触发次数超过限制
            triggerCount++;
            if (maxTriggerCount != -1 && triggerCount >= maxTriggerCount)
                triggerCount = maxTriggerCount;
            TriggerEventMethod();

        }
        //恢复到未触发
        public void Recover()
        {
            if (triggerCount==0)
                return;
            RecoverEventMethod();
            triggerCount = 0;
        }
        protected virtual void TriggerEventMethod() { }
        protected virtual void RecoverEventMethod() { }
        protected virtual void AddTriggerEvent()
        {
            
        }
        public virtual void RemoveEvent()
        {
           
        }
        /// <summary>
        /// buff覆盖时重新设置buff参数
        /// </summary>
        public virtual void SetValue(BattleBuffEventData battleBuffEventData,BattleSkillAddBuffValue battleSkillAddBuffValue )
        {

        }
        public BattleBuffEventBase(BattleBuffEventData battleBuffEventData, BattleBuffObj battleBuffObj)
        {
            battleBuffTriggerTime = battleBuffEventData.battleBuffTriggerTime;
            prob = battleBuffEventData.probability;
            this.battleBuffObj = battleBuffObj;
            owner = battleBuffObj.Owner;
            maxTriggerCount = battleBuffEventData.maxTriggerCount;
            AddTriggerEvent();
        }
    }

 
}
