using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Cosmos.Reference;
using AscensionProtocol.DTO;

namespace AscensionServer
{
    public class BattleBuffController
    {
        BattleCharacterEntity owner;

        Dictionary<BuffCoverType, List<BattleBuffObj>> battleBuffObjDict;

        public BuffCharacterData BuffCharacterData { get; private set; }

        #region buff事件
        Action< BattleCharacterEntity, ISkillAdditionData> afterPropertyChangeEvent;
        public event Action< BattleCharacterEntity, ISkillAdditionData> AfterPropertyChangeEvent
        {
            add { afterPropertyChangeEvent += value; }
            remove { afterPropertyChangeEvent -= value; }
        }
        Action< BattleCharacterEntity, ISkillAdditionData> beforeAllocationActionEvent;
        public event Action< BattleCharacterEntity, ISkillAdditionData> BeforeAllocationActionEvent
        {
            add { beforeAllocationActionEvent += value; }
            remove { beforeAllocationActionEvent -= value; }
        }
         Action<BattleCharacterEntity, ISkillAdditionData> beforeUseSkill;
        public event Action< BattleCharacterEntity, ISkillAdditionData> BeforeUseSkill
        {
            add { beforeUseSkill += value; }
            remove { beforeUseSkill -= value; }
        }
        Action<BattleCharacterEntity, ISkillAdditionData> beforeAttackEvent;
        public event Action<BattleCharacterEntity, ISkillAdditionData> BeforeAttackEvent
        {
            add { beforeAttackEvent += value; }
            remove { beforeAttackEvent -= value; }
        }
        Action<BattleCharacterEntity, ISkillAdditionData> behindAttackEvent;
        public event Action< BattleCharacterEntity, ISkillAdditionData> BehindAttackEvent
        {
            add { behindAttackEvent += value; }
            remove { behindAttackEvent -= value; }
        }
        Action<BattleCharacterEntity, ISkillAdditionData> behindUseSkill;
        public event Action<BattleCharacterEntity, ISkillAdditionData> BehindUseSkill
        {
            add { behindUseSkill += value; }
            remove { behindUseSkill -= value; }
        }
        Action<BattleCharacterEntity, ISkillAdditionData> beforeOnHitEvent;
        public event Action<BattleCharacterEntity, ISkillAdditionData> BeforeOnHitEvent
        {
            add { beforeOnHitEvent += value; }
            remove { beforeOnHitEvent -= value; }
        }
        Action< BattleCharacterEntity, ISkillAdditionData> behindOnHitEvent;
        public event Action<BattleCharacterEntity, ISkillAdditionData> BehindOnHitEvent
        {
            add { behindOnHitEvent += value; }
            remove { behindOnHitEvent -= value; }
        }
        Action<BattleCharacterEntity, ISkillAdditionData> roleBeforeDieEvent;
        public event Action< BattleCharacterEntity, ISkillAdditionData> RoleBeforeDieEvent
        {
            add { roleBeforeDieEvent += value; }
            remove { roleBeforeDieEvent -= value; }
        }
        Action<BattleCharacterEntity, ISkillAdditionData> roleAfterDieEvent;
        public event Action< BattleCharacterEntity, ISkillAdditionData> RoleAfterDieEvent
        {
            add { roleAfterDieEvent += value; }
            remove { roleAfterDieEvent -= value; }
        }
        #endregion

        public void AddBuff(BattleSkillAddBuffData battleSkillAddBuffData,BattleSkillBase battleSkillBase)
        {
            Utility.Debug.LogError("尝试添加buff");
            GameEntry.DataManager.TryGetValue<Dictionary<int, BattleBuffData>>(out var battleBuffDataDict);
            BattleBuffData battleBuffData = battleBuffDataDict[battleSkillAddBuffData.buffId];
            if (CanCoverBuff(battleBuffData, battleSkillAddBuffData))
            {
                Utility.Debug.LogError("添加新buff");
                BattleBuffObj battleBuffObj = CosmosEntry.ReferencePoolManager.Spawn<BattleBuffObj>();
                battleBuffObj.SetData(battleBuffData,owner, battleSkillBase, battleSkillAddBuffData);
                battleBuffObjDict[battleBuffObj.BattleBuffData.buffCoverType].Add(battleBuffObj);
                battleBuffObj.OnAdd();
            }
        }
        public void RemoveBuff(BattleBuffObj battleBuffObj)
        {

            battleBuffObj.OnRemove();
            battleBuffObjDict[battleBuffObj.BattleBuffData.buffCoverType].Remove(battleBuffObj);
            CosmosEntry.ReferencePoolManager.Despawn(battleBuffObj);
        }
        //覆盖buff
        public void CoverBuff(BattleBuffObj battleBuffObj,BattleSkillAddBuffData battleSkillAddBuffData, BattleBuffData battleBuffData)
        {
            Utility.Debug.LogError("覆盖buff");
            BattleBuffObj newBbattleBuffObj = CosmosEntry.ReferencePoolManager.Spawn<BattleBuffObj>();
            newBbattleBuffObj.SetData(battleBuffData, owner, battleBuffObj.OwnerSkill, battleSkillAddBuffData);
            battleBuffObjDict[newBbattleBuffObj.BattleBuffData.buffCoverType].Add(newBbattleBuffObj);
            newBbattleBuffObj.OnAdd();

            battleBuffObj.OnCover(battleBuffData, battleSkillAddBuffData);
            battleBuffObjDict[battleBuffObj.BattleBuffData.buffCoverType].Remove(battleBuffObj);
            CosmosEntry.ReferencePoolManager.Despawn(battleBuffObj);
        }

        #region 事件触发
        public ISkillAdditionData TriggerBuffEventAfterPropertyChange(BattleCharacterEntity target=null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            afterPropertyChangeEvent?.Invoke(target,skillAdditionData);
            return skillAdditionData;
        }
        public ISkillAdditionData TriggerBuffEventBerforeUseSkill(BattleCharacterEntity target = null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            beforeUseSkill?.Invoke( target, skillAdditionData);
            return skillAdditionData;
        }
        public ISkillAdditionData TriggerBuffEventBeforeAllocationAction( BattleCharacterEntity target = null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            beforeAllocationActionEvent?.Invoke( target, skillAdditionData);
            return skillAdditionData;
        }
        public ISkillAdditionData TriggerBuffEventBeforeAttack(BattleCharacterEntity target = null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            beforeAttackEvent?.Invoke( target, skillAdditionData);
            return skillAdditionData;
        }
        public ISkillAdditionData TriggerBuffEventBehindAttack( BattleCharacterEntity target = null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            behindAttackEvent?.Invoke( target, skillAdditionData);
            return skillAdditionData;
        }
        public ISkillAdditionData TriggerBuffEventBehindUseSkill( BattleCharacterEntity target = null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            behindUseSkill?.Invoke( target, skillAdditionData);
            return skillAdditionData;
        }
        public ISkillAdditionData TriggerBuffEventBeforeOnHit(BattleCharacterEntity target = null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            beforeOnHitEvent?.Invoke(target, skillAdditionData);
            return skillAdditionData;
        }
        public ISkillAdditionData TriggerBuffEventBehindOnHit(BattleCharacterEntity target = null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            behindOnHitEvent?.Invoke( target, skillAdditionData);
            return skillAdditionData;
        }
        public ISkillAdditionData TriggerBuffEventBehindRoleDie(BattleCharacterEntity target = null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            roleBeforeDieEvent?.Invoke( target, skillAdditionData);
            return skillAdditionData;
        }
        public ISkillAdditionData TriggerBuffEventAfterRoleDie( BattleCharacterEntity target = null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            roleAfterDieEvent?.Invoke( target, skillAdditionData);
            return skillAdditionData;
        }
        #endregion

        // <summary>
        // buff覆盖的处理
        // </summary>
        // <returns>是否可以创建新buff</returns>
        bool CanCoverBuff(BattleBuffData battleBuffData,BattleSkillAddBuffData battleSkillAddBuffData)
        {
            bool flag = battleBuffObjDict.TryGetValue(battleBuffData.buffCoverType, out var targetList);
            if (!flag)
            {
                battleBuffObjDict[battleBuffData.buffCoverType] = new List<BattleBuffObj>();
                return true;
            }
            switch (battleBuffData.buffCoverType)
            {
                case BuffCoverType.Property:
                    BattleBuffObj battleBuffObj = targetList.Find(p => p.BuffId == battleBuffData.id);
                    if (battleBuffObj != null)
                    {
                        if (battleBuffObj.MaxOverlayLayer>1|| battleBuffObj.MaxOverlayLayer==-1)//可以叠加
                        {
                            battleBuffObj.AddOverlayLayer();
                        }
                        else//不可叠加
                        {
                            CoverBuff(battleBuffObj, battleSkillAddBuffData, battleBuffData);
                        }
                        
                        return false;
                    }
                    else
                        return true;
                case BuffCoverType.Bleed:
                case BuffCoverType.Burn:
                case BuffCoverType.Poisoning:
                case BuffCoverType.Frozen:
                    if (targetList.Count == 0) return true;
                    battleBuffObj = targetList[0];
                    if (battleBuffData.buffLayer > battleBuffObj.BattleBuffData.buffLayer)
                    {
                        RemoveBuff(battleBuffObj);
                        return true;
                    }
                    else if (battleBuffData.buffLayer == battleBuffObj.BattleBuffData.buffLayer)
                    {
                        CoverBuff(battleBuffObj, battleSkillAddBuffData, battleBuffData);
                        return false;
                    }
                    else
                        return false;
            }
            return true;
        }

        public BattleBuffController(BattleCharacterEntity battleCharacterEntity)
        {
            owner = battleCharacterEntity;
            battleBuffObjDict = new Dictionary<BuffCoverType, List<BattleBuffObj>>();
            BuffCharacterData = new BuffCharacterData(owner.CharacterBattleData);
        }
    }
}
