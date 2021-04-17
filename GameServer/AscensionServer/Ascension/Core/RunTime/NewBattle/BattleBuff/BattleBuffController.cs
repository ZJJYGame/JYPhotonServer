using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Cosmos.Reference;

namespace AscensionServer
{
    public class BattleBuffController
    {
        BattleCharacterEntity owner;

        Dictionary<BuffCoverType, List<BattleBuffObj>> battleBuffObjDict;

        public BuffCharacterData BuffCharacterData { get; private set; }

        #region buff事件
        public Action afterPropertyChangeEvent;
        public event Action AfterPropertyChangeEvent
        {
            add { afterPropertyChangeEvent += value; }
            remove { afterPropertyChangeEvent -= value; }
        }
        public Action allocationActionEvent;
        public event Action AllocationActionEvent
        {
            add { allocationActionEvent += value; }
            remove { allocationActionEvent -= value; }
        }
        public Action beforeUseSkill;
        public event Action BeforeUseSkill
        {
            add { beforeUseSkill += value; }
            remove { beforeUseSkill -= value; }
        }
        public Action beforeAttackEvent;
        public event Action BeforeAttackEvent
        {
            add { beforeAttackEvent += value; }
            remove { beforeAttackEvent -= value; }
        }
        public Action behindAttackEvent;
        public event Action BehindAttackEvent
        {
            add { behindAttackEvent += value; }
            remove { behindAttackEvent -= value; }
        }
        public Action behindUseSkill;
        public event Action BehindUseSkill
        {
            add { behindUseSkill += value; }
            remove { behindUseSkill -= value; }
        }
        public Action beforeOnHitEvent;
        public event Action BeforeOnHitEvent
        {
            add { beforeOnHitEvent += value; }
            remove { beforeOnHitEvent -= value; }
        }
        public Action behindOnHitEvent;
        public event Action BehindOnHitEvent
        {
            add { behindOnHitEvent += value; }
            remove { behindOnHitEvent -= value; }
        }
        public Action roleBeforeDieEvent;
        public event Action RoleBeforeDieEvent
        {
            add { roleBeforeDieEvent += value; }
            remove { roleBeforeDieEvent -= value; }
        }
        public Action roleAfterDieEvent;
        public event Action RoleAfterDieEvent
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
