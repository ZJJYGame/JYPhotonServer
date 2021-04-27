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
    /// <summary>
    /// 战斗buff个体对象
    /// </summary>
    public class BattleBuffObj : IReference,ISkillAdditionData
    {
        public BattleCharacterEntity Owner { get; protected set; }
        //buff的来源
        public BattleCharacterEntity OrginRole { get { return OwnerSkill.OwnerEntity; } }
        public BattleSkillBase OwnerSkill { get; protected set; }
        public BattleController BattleController { get; protected set; }

        public int BuffId { get; protected set; }
        public int MaxRound { get; protected set; }
        public int NowRound { get; protected set; }
        //buff叠加层数
        public int OverlayLayer { get; protected set; }
        //最大可叠加次数
        public int MaxOverlayLayer { get; protected set; }

        public BattleBuffData BattleBuffData { get; protected set; }

        public int DamgeAddition { get; set; }
        public int CritProp { get; set; }
        public int CritDamage { get; set; }
        public int IgnoreDefensive { get; set; }
        public int DamagDeduction { get; set; }
        public int DodgeProp { get; set; }

        List<BattleBuffEventBase> battleBuffEventList;
        List<BattleBuffEventConditionBase> battleBuffEventConditionList;

        Action<BattleCharacterEntity,BattleDamageData,ISkillAdditionData> buffAddEvent;
        public event Action<BattleCharacterEntity, BattleDamageData, ISkillAdditionData> BuffAddEvent
        {
            add { buffAddEvent += value; }
            remove { buffAddEvent -= value; }
        }
        Action buffRemoveEvent;
        public event Action BuffRemoveEvent
        {
            add { buffRemoveEvent += value; }
            remove { buffRemoveEvent -= value; }
        }
        Action buffCoverEvent;
        public event Action BuffCoverEvent
        {
            add { buffCoverEvent += value; }
            remove { buffCoverEvent -= value; }
        }

        public void SetData(BattleBuffData battleBuffData,BattleCharacterEntity owner,BattleSkillBase battleSkillBase, BattleSkillAddBuffData battleSkillAddBuffData)
        {
            Owner = owner;
            OwnerSkill = battleSkillBase;
            BattleBuffData = battleBuffData;

            battleBuffEventList = new List<BattleBuffEventBase>();
            battleBuffEventConditionList = new List<BattleBuffEventConditionBase>();

            BuffId = battleBuffData.id;
            MaxRound = battleSkillAddBuffData.round;
            NowRound = MaxRound;
            OverlayLayer = 1;
            MaxOverlayLayer = battleBuffData.maxSuperpositionCount;

            BattleController = GameEntry.BattleRoomManager.GetBattleRoomEntity(Owner.RoomID).BattleController;
            BattleController.RoundFinishEvent -= RoundEnd;


            for (int i = 0; i < battleBuffData.battleBuffEventDataList.Count; i++)
            {
                BattleBuffEventBase battleBuffEventBase = null;
                switch (battleBuffData.battleBuffEventDataList[i].battleBuffEventType)
                {
                    case BattleBuffEventType.RolePropertyChange:
                        battleBuffEventBase = new BattleBuffEvent_ChangeProperty(battleBuffData.battleBuffEventDataList[i], this, battleSkillAddBuffData.battleSkillAddBuffValueList[i]);
                        break;
                    case BattleBuffEventType.BuffPropertyChange:
                        battleBuffEventBase = new BattleBuffEvent_ChangeBuffProperty(battleBuffData.battleBuffEventDataList[i], this);
                        break;
                    case BattleBuffEventType.ForbiddenBuff:
                        break;
                    case BattleBuffEventType.RoleStateChange:
                        break;
                    case BattleBuffEventType.UseDesignateSkill:
                        battleBuffEventBase = new BattleBuffEvent_UseDesignateSkill(battleBuffData.battleBuffEventDataList[i], this);
                        break;
                    case BattleBuffEventType.DamageOrHeal:
                        battleBuffEventBase = new BattleBuffEvent_DamageOrHeal(battleBuffData.battleBuffEventDataList[i], this);
                        break;
                    case BattleBuffEventType.Shield:
                        battleBuffEventBase = new BattleBuffEvent_Shield(battleBuffData.battleBuffEventDataList[i], this);
                        break;
                    case BattleBuffEventType.DamageReduce:
                        battleBuffEventBase = new BattleBuffEvent_DamageReduce(battleBuffData.battleBuffEventDataList[i], this);
                        break;
                    case BattleBuffEventType.ShareDamage:
                        battleBuffEventBase = new BattleBuffEvent_ShareDamage(battleBuffData.battleBuffEventDataList[i], this);
                        break;
                    case BattleBuffEventType.AddBuff:
                        battleBuffEventBase = new BattleBuffEvent_AddBuff(battleBuffData.battleBuffEventDataList[i], this);
                        break;
                    case BattleBuffEventType.DispelBuff:
                        battleBuffEventBase = new BattleBuffEvent_DispelBuff(battleBuffData.battleBuffEventDataList[i], this);
                        break;
                    case BattleBuffEventType.NotResurgence:
                        break;
                    case BattleBuffEventType.ImmuneBuff:
                        battleBuffEventBase = new BattleBuffEvent_ImmuneBuff(battleBuffData.battleBuffEventDataList[i], this);
                        break;
                    case BattleBuffEventType.ChangeCmd:
                        battleBuffEventBase = new BattleBuffEvent_ChangeCmd(battleBuffData.battleBuffEventDataList[i], this);
                        break;
                    case BattleBuffEventType.ChangeTagrget:
                        battleBuffEventBase = new BattleBuffEvent_ChangeTarget(battleBuffData.battleBuffEventDataList[i], this);
                        break;
                }
                battleBuffEventList.Add(battleBuffEventBase);
            }
            //添加回合结束事件
            BattleController.RoundFinishEvent += RoundEnd;
        }

        public bool CanTrigger()
        {
            bool flag = true;
            for (int i = 0; i < battleBuffEventConditionList.Count; i++)
            {
                flag = flag && battleBuffEventConditionList[i].CanTrigger();
            }
            return flag;
        }
        /// <summary>
        /// buff添加事件
        /// </summary>
        public void OnAdd()
        {
            buffAddEvent?.Invoke(null,null,null);
        }
        /// <summary>
        /// buff移除事件
        /// </summary>
        public void OnRemove()
        {
            buffRemoveEvent?.Invoke();
        }
        //被覆盖
        public void OnCover(BattleBuffData battleBuffData,BattleSkillAddBuffData battleSkillAddBuffData)
        {
            buffCoverEvent?.Invoke();
            for (int i = 0; i < battleBuffData.battleBuffEventDataList.Count; i++)
            {
                battleBuffEventList[i].SetValue(battleBuffData.battleBuffEventDataList[i], battleSkillAddBuffData.battleSkillAddBuffValueList[i]);
            }
            MaxRound = battleSkillAddBuffData.round;
            NowRound = MaxRound;
            OnAdd();
        }
        //添加叠加层数
        public void AddOverlayLayer()
        {
            OverlayLayer++;
            if (MaxOverlayLayer != -1)
                OverlayLayer = OverlayLayer > MaxOverlayLayer ? MaxOverlayLayer : OverlayLayer;
            NowRound = MaxRound;
            OnRemove();
            OnAdd();
        }

        //回合结束的事件
        void RoundEnd()
        {
            NowRound--;
            if (NowRound <= 0)
                Owner.BattleBuffController.RemoveBuff(this);
        }


        public void Clear()
        {
            BattleController.RoundFinishEvent -= RoundEnd;

            for (int i = 0; i < battleBuffEventList.Count; i++)
            {
                battleBuffEventList[i].RemoveEvent();
            }

            Owner = null;
            OwnerSkill = null;
            BuffId = 0;
            MaxRound = 0;
            NowRound = 0;
            OverlayLayer = 0;
            MaxOverlayLayer = 0;
            BattleBuffData = null;

        }
    }
}
