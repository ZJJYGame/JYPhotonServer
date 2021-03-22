using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionServer.Model;
using AscensionProtocol.DTO;

namespace AscensionServer
{
    public class BattlePetEntity:BattleCharacterEntity
    {
        public int MasterID { get; private set; }

        public void InitPet(int roomID, int roleID,BattleFactionType battleFactionType)
        {
            Init();
            NHCriteria nHCriteriaRoleID = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            RolePet rolePet = NHibernateQuerier.CriteriaSelect<RolePet>(nHCriteriaRoleID);
            if (rolePet.PetIsBattle == 0)
                return;
            NHCriteria nHCriteriaPetID = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("PetID", rolePet.PetIsBattle);
            PetStatus petStatus = NHibernateQuerier.CriteriaSelect<PetStatus>(nHCriteriaPetID);
            Pet pet= NHibernateQuerier.CriteriaSelect<Pet>(nHCriteriaPetID);
            //todo 拿取宠物数据
            CharacterBattleData = CosmosEntry.ReferencePoolManager.Spawn<CharacterBattleData>();
            CharacterBattleData.Init(petStatus);
            UniqueID = petStatus.PetID;
            GlobalID = rolePet.PetIDDict[UniqueID];
            MasterID = roleID;
            BattleFactionType = battleFactionType;
            Name = pet.PetName;
            RoomID = roomID;
        }

        public override T ToBattleDataBase<T>()
        {
            T t = new PetBattleDataDTO()
            {
                RoleId=MasterID,
                ObjectID=UniqueID,
                ObjectName = Name,
                PetStatusDTO = new PetStatusDTO
                {
                    PetMaxHP = CharacterBattleData.MaxHp,
                    PetHP = CharacterBattleData.Hp,
                    PetMaxMP = CharacterBattleData.MaxMp,
                    PetMP = CharacterBattleData.Mp,
                    PetMaxShenhun = CharacterBattleData.MaxSoul,
                    PetShenhun = CharacterBattleData.Soul,
                }
            } as T;
            return t;
        }

        public override void SetBattleAction(BattleCmd battleCmd, BattleTransferDTO battleTransferDTO)
        {
            BattleCmd = battleCmd;
            ActionID = battleTransferDTO.ClientCmdId;
            for (int i = 0; i < battleTransferDTO.TargetInfos.Count; i++)
            {
                TargetIDList.Add(battleTransferDTO.TargetInfos[i].TargetID);
            }
        }

        public override void Clear()
        {
        }
    }
}
