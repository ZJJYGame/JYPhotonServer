﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;

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
        /// <summary>
        /// 具体buff触发事件实现
        /// </summary>
        /// <param name="skillAdditionData">用于记录临时的加成数据，比如技能加成</param>
        public void Trigger(BattleCharacterEntity target,BattleDamageData battleDamageData, ISkillAdditionData skillAdditionData)
        {

            //todo 暂时设置为必定触发
            //if (!CanTrigger())
            //    return;
            //暂时设置为血量低于50%触发
            //int healthPercent = owner.CharacterBattleData.GetPropertyPercent(BattleSkillEventTriggerNumSourceType.Health);
            //if (healthPercent > 90)
            //{
            //    Recover();
            //    return;
            //}
            //最大触发次数超过限制
            triggerCount++;
            if (maxTriggerCount != -1 && triggerCount >= maxTriggerCount)
                triggerCount = maxTriggerCount;
            TriggerEventMethod(target,battleDamageData,skillAdditionData);

        }
        //恢复到未触发
        public void Recover()
        {
            if (triggerCount==0)
                return;
            triggerCount = 0;
            TriggerEventMethod(null,null,null);

        }
        protected virtual void TriggerEventMethod( BattleCharacterEntity target, BattleDamageData battleDamageData,ISkillAdditionData skillAdditionData) { }
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