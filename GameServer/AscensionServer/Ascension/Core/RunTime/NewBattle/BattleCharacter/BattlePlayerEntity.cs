using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Protocol;

namespace AscensionServer
{
    public class BattlePlayerEntity:BattleCharacterEntity
    {
        //上场的宠物ID；
        public int PetID { get; protected set; }

        public void InitPlayer(int roomID, int roleID,BattleFactionType battleFactionType)
        {
            Init();
            NHCriteria nHCriteriaRoleID = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            RoleStatus roleStatus = NHibernateQuerier.CriteriaSelect<RoleStatus>(nHCriteriaRoleID);
            Role role= NHibernateQuerier.CriteriaSelect<Role>(nHCriteriaRoleID);
            PetID = NHibernateQuerier.CriteriaSelect<RolePet>(nHCriteriaRoleID).PetIsBattle;
            //todo 从俞拿取正确的数据
            CharacterBattleData = CosmosEntry.ReferencePoolManager.Spawn<CharacterBattleData>();
            CharacterBattleData.Init(roleStatus);
            UniqueID = roleID;
            GlobalID = 0;
            BattleFactionType = battleFactionType;
            Name = role.RoleName;
            RoomID = roomID;
        }

        public override T ToBattleDataBase<T>()
        {
            T t = new RoleBattleDataDTO() {
                ObjectName = Name,
                RoleStatusDTO = new RoleStatusDTO
                {
                    RoleID = UniqueID,
                    RoleMaxHP = CharacterBattleData.MaxHp,
                    RoleHP = CharacterBattleData.Hp,
                    RoleMaxMP = CharacterBattleData.MaxMp,
                    RoleMP = CharacterBattleData.Mp,
                    RoleMaxSoul = CharacterBattleData.MaxSoul,
                    RoleSoul = CharacterBattleData.Soul,
                    BestBloodMax = (short)CharacterBattleData.BestBloodMax,
                    BestBlood = (short)CharacterBattleData.BestBlood,
                }
            } as T;
            return t ;
        }

        public override void SetBattleAction(BattleCmd battleCmd, BattleTransferDTO battleTransferDTO)
        {
            TargetIDList.Clear();
            BattleCmd = battleCmd;
            ActionID = battleTransferDTO.ClientCmdId;
            for (int i = 0; i < battleTransferDTO.TargetInfos.Count; i++)
            {
                TargetIDList.Add(battleTransferDTO.TargetInfos[i].TargetID);
            }
            if (PetID != 0)
            {
                GameEntry.BattleCharacterManager.GetCharacterEntity(PetID).SetBattleAction(battleTransferDTO.petBattleTransferDTO.BattleCmd, battleTransferDTO.petBattleTransferDTO);
            }
        }
        public override void AllocationBattleAction()
        {
            base.AllocationBattleAction();
            //todo 先临时将AI的行为设置为普通攻击
            GameEntry.DataManager.TryGetValue<Dictionary<int, BattleSkillData>>(out var battleskillDataDict);
            BattleSkillData battleSkillData = battleskillDataDict[ActionID];
            BattleFactionType battleFactionType = default;
            switch (battleSkillData.battleSkillFactionType)
            {
                case BattleSkillFactionType.Enemy:
                    battleFactionType = (BattleFactionType == BattleFactionType.FactionOne) ? BattleFactionType.FactionTwo : BattleFactionType.FactionOne;
                    break;
                case BattleSkillFactionType.TeamMate:
                    battleFactionType = (BattleFactionType == BattleFactionType.FactionOne) ? BattleFactionType.FactionOne : BattleFactionType.FactionTwo;
                    break;
            }

            TargetIDList = GameEntry.BattleRoomManager.GetBattleRoomEntity(RoomID).BattleController.RandomGetTarget(battleSkillData.TargetNumber, battleFactionType,true,TargetIDList);
        }

        public void SendMessage(OperationData opData)
        {
            GameEntry.RoleManager.SendMessage(UniqueID, opData);
        }

        public override void Clear()
        {

        }
    }
}
