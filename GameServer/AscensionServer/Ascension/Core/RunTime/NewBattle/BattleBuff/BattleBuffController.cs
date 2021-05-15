﻿using System;
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

        public Dictionary<BuffCoverType, List<BattleBuffObj>> battleBuffObjDict8Type;
        public Dictionary<int, List<BattleBuffObj>> battleBuffObjDict8Id;

        public BuffCharacterData BuffCharacterData { get; private set; }

        public HashSet<int> ImmuneBuffId { get; set; }
        public HashSet<int> ForbiddenBuff { get; set; }


        #region buff事件
        Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> berforePropertyChangeEvent;
        public event Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> BeforePropertyChangeEvent
        {
            add { berforePropertyChangeEvent += value; }
            remove { berforePropertyChangeEvent -= value; }
        }
        Action<BattleTransferDTO, BattleCharacterEntity,BattleDamageData, ISkillAdditionData> afterPropertyChangeEvent;
        public event Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> AfterPropertyChangeEvent
        {
            add { afterPropertyChangeEvent += value; }
            remove { afterPropertyChangeEvent -= value; }
        }
        Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> beforeAllocationActionEvent;
        public event Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> BeforeAllocationActionEvent
        {
            add { beforeAllocationActionEvent += value; }
            remove { beforeAllocationActionEvent -= value; }
        }
         Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> beforeUseSkill;
        public event Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> BeforeUseSkill
        {
            add { beforeUseSkill += value; }
            remove { beforeUseSkill -= value; }
        }
        Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> beforeAttackEvent;
        public event Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> BeforeAttackEvent
        {
            add { beforeAttackEvent += value; }
            remove { beforeAttackEvent -= value; }
        }
        Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> behindAttackEvent;
        public event Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> BehindAttackEvent
        {
            add { behindAttackEvent += value; }
            remove { behindAttackEvent -= value; }
        }
        Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> behindUseSkill;
        public event Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> BehindUseSkill
        {
            add { behindUseSkill += value; }
            remove { behindUseSkill -= value; }
        }
        Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> beforeOnHitEvent;
        public event Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> BeforeOnHitEvent
        {
            add { beforeOnHitEvent += value; }
            remove { beforeOnHitEvent -= value; }
        }
        Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> behindOnHitEvent;
        public event Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> BehindOnHitEvent
        {
            add { behindOnHitEvent += value; }
            remove { behindOnHitEvent -= value; }
        }
        Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> roleBeforeDieEvent;
        public event Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> RoleBeforeDieEvent
        {
            add { roleBeforeDieEvent += value; }
            remove { roleBeforeDieEvent -= value; }
        }
        Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> roleAfterDieEvent;
        public event Action<BattleTransferDTO, BattleCharacterEntity, BattleDamageData, ISkillAdditionData> RoleAfterDieEvent
        {
            add { roleAfterDieEvent += value; }
            remove { roleAfterDieEvent -= value; }
        }
        #endregion
        public bool HasBuff(int buffId)
        {
            if (battleBuffObjDict8Id.ContainsKey(buffId))
            {
                if (battleBuffObjDict8Id[buffId].Count > 0)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="battleSkillAddBuffData"></param>
        /// <param name="battleSkillBase"></param>
        /// <param name="orginRole">buff的来源角色</param>
        public void AddBuff(BattleSkillAddBuffData battleSkillAddBuffData,BattleSkillBase battleSkillBase)
        {
            if (ImmuneBuffId.Contains(battleSkillAddBuffData.buffId))
                return;
            GameEntry.DataManager.TryGetValue<Dictionary<int, BattleBuffData>>(out var battleBuffDataDict);
            BattleBuffData battleBuffData = battleBuffDataDict[battleSkillAddBuffData.buffId];
            if (CanCoverBuff(battleBuffData, battleSkillAddBuffData, battleSkillBase))
            {
                Utility.Debug.LogError("添加新buff");
                BattleBuffObj battleBuffObj = CosmosEntry.ReferencePoolManager.Spawn<BattleBuffObj>();
                battleBuffObj.SetData(battleBuffData,owner, battleSkillBase, battleSkillAddBuffData);
                battleBuffObjDict8Type[battleBuffObj.BattleBuffData.buffCoverType].Add(battleBuffObj);
                if (!battleBuffObjDict8Id.ContainsKey(battleBuffObj.BuffId))
                    battleBuffObjDict8Id[battleBuffObj.BuffId] = new List<BattleBuffObj>();
                battleBuffObjDict8Id[battleBuffObj.BuffId].Add(battleBuffObj);
                battleBuffObj.OnAdd();
            }
        }
        public void RemoveBuff(BattleBuffObj battleBuffObj)
        {
            Utility.Debug.LogError("移除buff");
            battleBuffObj.OnRemove();
            battleBuffObjDict8Type[battleBuffObj.BattleBuffData.buffCoverType].Remove(battleBuffObj);
            battleBuffObjDict8Id[battleBuffObj.BuffId].Remove(battleBuffObj);
            CosmosEntry.ReferencePoolManager.Despawn(battleBuffObj);
        }
        public void RemoveBuff(int buffId)
        {
            if (!battleBuffObjDict8Id.ContainsKey(buffId))
                return;
            for (int i = battleBuffObjDict8Id[buffId].Count-1; i >=0; i--)
            {
                RemoveBuff(battleBuffObjDict8Id[buffId][i]);
            }
        }
        //覆盖buff
        public void CoverBuff(BattleBuffObj battleBuffObj,BattleSkillAddBuffData battleSkillAddBuffData, BattleBuffData battleBuffData,BattleSkillBase battleSkillBase)
        {
            Utility.Debug.LogError(owner.UniqueID+"覆盖buff");
            BattleBuffObj newBbattleBuffObj = CosmosEntry.ReferencePoolManager.Spawn<BattleBuffObj>();
            newBbattleBuffObj.SetData(battleBuffData, owner, battleSkillBase, battleSkillAddBuffData);
            battleBuffObjDict8Type[newBbattleBuffObj.BattleBuffData.buffCoverType].Add(newBbattleBuffObj);
            battleBuffObjDict8Id[newBbattleBuffObj.BuffId].Add(newBbattleBuffObj);
            newBbattleBuffObj.OnAdd();

            battleBuffObj.OnCover(battleBuffData, battleSkillAddBuffData);
            battleBuffObjDict8Type[battleBuffObj.BattleBuffData.buffCoverType].Remove(battleBuffObj);
            battleBuffObjDict8Id[newBbattleBuffObj.BuffId].Remove(newBbattleBuffObj);
            CosmosEntry.ReferencePoolManager.Despawn(battleBuffObj);
        }

        #region 事件触发
        public ISkillAdditionData TriggerBuffEventBeforePropertyChange(BattleTransferDTO battleTransferDTO, BattleCharacterEntity target = null, BattleDamageData battleDamageData = null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            berforePropertyChangeEvent?.Invoke(battleTransferDTO,target, battleDamageData, skillAdditionData);
            return skillAdditionData;
        }
        public ISkillAdditionData TriggerBuffEventAfterPropertyChange( BattleCharacterEntity target =null,BattleDamageData battleDamageData=null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            afterPropertyChangeEvent?.Invoke(null,target, battleDamageData,skillAdditionData);
            return skillAdditionData;
        }
        public ISkillAdditionData TriggerBuffEventBerforeUseSkill(BattleTransferDTO battleTransferDTO, BattleCharacterEntity target = null, BattleDamageData battleDamageData = null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            beforeUseSkill?.Invoke(battleTransferDTO, target, battleDamageData, skillAdditionData);
            return skillAdditionData;
        }
        public ISkillAdditionData TriggerBuffEventBeforeAllocationAction( BattleCharacterEntity target = null, BattleDamageData battleDamageData = null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            beforeAllocationActionEvent?.Invoke(null, target, battleDamageData, skillAdditionData);
            return skillAdditionData;
        }
        public ISkillAdditionData TriggerBuffEventBeforeAttack(BattleTransferDTO battleTransferDTO, BattleCharacterEntity target = null, BattleDamageData battleDamageData = null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            beforeAttackEvent?.Invoke(battleTransferDTO, target, battleDamageData, skillAdditionData);
            return skillAdditionData;
        }
        public ISkillAdditionData TriggerBuffEventBehindAttack(BattleTransferDTO battleTransferDTO, BattleCharacterEntity target = null, BattleDamageData battleDamageData = null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            behindAttackEvent?.Invoke(battleTransferDTO, target, battleDamageData, skillAdditionData);
            return skillAdditionData;
        }
        public ISkillAdditionData TriggerBuffEventBehindUseSkill(BattleTransferDTO battleTransferDTO, BattleCharacterEntity target = null, BattleDamageData battleDamageData = null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            behindUseSkill?.Invoke(battleTransferDTO, target, battleDamageData, skillAdditionData);
            return skillAdditionData;
        }
        public ISkillAdditionData TriggerBuffEventBeforeOnHit(BattleTransferDTO battleTransferDTO, BattleCharacterEntity target = null, BattleDamageData battleDamageData = null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            beforeOnHitEvent?.Invoke(battleTransferDTO, target, battleDamageData, skillAdditionData);
            return skillAdditionData;
        }
        public ISkillAdditionData TriggerBuffEventBehindOnHit(BattleTransferDTO battleTransferDTO, BattleCharacterEntity target = null, BattleDamageData battleDamageData = null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            behindOnHitEvent?.Invoke(battleTransferDTO, target, battleDamageData, skillAdditionData);
            return skillAdditionData;
        }
        public ISkillAdditionData TriggerBuffEventBeforeRoleDie(BattleTransferDTO battleTransferDTO, BattleCharacterEntity target = null, BattleDamageData battleDamageData = null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            roleBeforeDieEvent?.Invoke(battleTransferDTO, target, battleDamageData, skillAdditionData);
            return skillAdditionData;
        }
        public ISkillAdditionData TriggerBuffEventAfterRoleDie(BattleTransferDTO battleTransferDTO, BattleCharacterEntity target = null, BattleDamageData battleDamageData = null)
        {
            ISkillAdditionData skillAdditionData = new SkillAdditionData();
            roleAfterDieEvent?.Invoke(battleTransferDTO, target, battleDamageData, skillAdditionData);
            return skillAdditionData;
        }
        #endregion

        // <summary>
        // buff覆盖的处理
        // </summary>
        // <returns>是否可以创建新buff</returns>
        bool CanCoverBuff(BattleBuffData battleBuffData,BattleSkillAddBuffData battleSkillAddBuffData,BattleSkillBase battleSkillBase)
        {
            bool flag = battleBuffObjDict8Type.TryGetValue(battleBuffData.buffCoverType, out var targetList);
            if (!flag)
            {
                battleBuffObjDict8Type[battleBuffData.buffCoverType] = new List<BattleBuffObj>();
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
                            CoverBuff(battleBuffObj, battleSkillAddBuffData, battleBuffData, battleSkillBase);
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
                        CoverBuff(battleBuffObj, battleSkillAddBuffData, battleBuffData, battleSkillBase);
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
            battleBuffObjDict8Type = new Dictionary<BuffCoverType, List<BattleBuffObj>>();
            battleBuffObjDict8Id = new Dictionary<int, List<BattleBuffObj>>();
            BuffCharacterData = new BuffCharacterData(owner.CharacterBattleData);
            ImmuneBuffId = new HashSet<int>();
            ForbiddenBuff = new HashSet<int>();
        }
    }
}
