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
    /// 分担伤害buff事件
    /// </summary>
    public class BattleBuffEvent_ShareDamage : BattleBuffEventBase
    {


        protected override void AddTriggerEvent()
        {


            List<BattleCharacterEntity> battleCharacterEntities = GetFriendEnentity();
            for (int i = 0; i < battleCharacterEntities.Count; i++)
            {
                Utility.Debug.LogError(battleCharacterEntities[i].UniqueID);
                battleCharacterEntities[i].BattleBuffController.BeforeOnHitEvent += Trigger;
            }

        }
        public override void RemoveEvent()
        {
            List<BattleCharacterEntity> battleCharacterEntities = GetFriendEnentity();
            for (int i = 0; i < battleCharacterEntities.Count; i++)
            {
                battleCharacterEntities[i].BattleBuffController.BeforeOnHitEvent -= Trigger;
            }
        }
        protected override void TriggerEventMethod(BattleCharacterEntity target, BattleDamageData battleDamageData, ISkillAdditionData skillAdditionData)
        {
            //自己为别人承担伤害

            BattleDamageData newBattleDamageData = new BattleDamageData();
            newBattleDamageData.TargetID = owner.UniqueID;
            newBattleDamageData.damageNum = battleDamageData.damageNum * 50 / 100;
            newBattleDamageData.battleSkillActionType = BattleSkillActionType.Damage;
            battleDamageData.damageNum = battleDamageData.damageNum * 50 / 100;
            owner.OnActionEffect(battleDamageData);
            if (target.BattleBuffController.NowBattleTransferDTO.TargetInfos == null)
                target.BattleBuffController.NowBattleTransferDTO.TargetInfos = new List<TargetInfoDTO>();
            target.BattleBuffController.NowBattleTransferDTO.TargetInfos.Add(new TargetInfoDTO()
            {
                TargetID = owner.UniqueID,
                TargetHPDamage = newBattleDamageData.damageNum,
            });

        }
        List<BattleCharacterEntity> GetFriendEnentity()
        {
            List<BattleCharacterEntity> battleCharacterEntities = new List<BattleCharacterEntity>();
            Utility.Debug.LogError("数量=>" + owner.FriendCharacterEntities.Count);

            for (int i = 0; i < owner.FriendCharacterEntities.Count; i++)
            {
                if (owner.FriendCharacterEntities[i] != owner)
                    battleCharacterEntities.Add(owner.FriendCharacterEntities[i]);
            }
            return battleCharacterEntities;
        }
        public BattleBuffEvent_ShareDamage(BattleBuffEventData battleBuffEventData, BattleBuffObj battleBuffObj) : base(battleBuffEventData, battleBuffObj)
        {
           
        }
    }
}
