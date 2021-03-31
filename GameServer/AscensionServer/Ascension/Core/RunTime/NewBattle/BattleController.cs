using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;

namespace AscensionServer
{
    public class BattleController : IReference
    {
        BattleRoomEntity battleRoomEntity;

        //所有角色列表
        public List<BattleCharacterEntity> AllCharacterEntities { get; private set; }
        //阵营一角色列表
        public List<BattleCharacterEntity> FactionOneCharacterEntites { get;private set; }
        //阵营二角色列表
        public List<BattleCharacterEntity> FactionTwoCharacterEntites { get; private set; }


        public void AddCharacterEntity(BattleCharacterEntity battleCharacterEntity)
        {
            AllCharacterEntities.Add(battleCharacterEntity);
            switch (battleCharacterEntity.BattleFactionType)
            {
                case BattleFactionType.FactionOne:
                    FactionOneCharacterEntites.Add(battleCharacterEntity);
                    break;
                case BattleFactionType.FactionTwo:
                    FactionTwoCharacterEntites.Add(battleCharacterEntity);
                    break;
            }
            battleCharacterEntity.SetFriendAndEnemy(FactionOneCharacterEntites, FactionTwoCharacterEntites);
        }
        public void InitController(BattleRoomEntity battleRoomEntity)
        {
            AllCharacterEntities = new List<BattleCharacterEntity>();
            FactionOneCharacterEntites = new List<BattleCharacterEntity>();
            FactionTwoCharacterEntites = new List<BattleCharacterEntity>();
            this.battleRoomEntity = battleRoomEntity;
        }

        public void StartBattle()
        {
            List<BattleTransferDTO> battleTransferDTOs = new List<BattleTransferDTO>();
            //角色按速度排序
            AllCharacterEntities.Sort();

            //按角色出手顺序开始出手
            int allCount = AllCharacterEntities.Count;
            BattleCharacterEntity actCharacter;
            for (int i = 0; i < allCount; i++)
            {
                actCharacter = AllCharacterEntities[i];
                //分配角色行为
                actCharacter.AllocationBattleAction();
                //开始计算角色行动
                battleTransferDTOs.AddRange(actCharacter.Action());
            }
            battleRoomEntity.SendBattlePerformDataS2C(battleTransferDTOs);
        }

        /// <summary>
        /// 随机获得行为目标列表
        /// </summary>
        public List<int> RandomGetTarget(int count,BattleFactionType battleSkillFactionType,List<int> targetIDList=null)
        {
            List<int> resultList = new List<int>();
            List<BattleCharacterEntity> targetCharacterList;
            if (battleSkillFactionType == BattleFactionType.FactionOne)
                targetCharacterList = FactionOneCharacterEntites;
            else
                targetCharacterList = FactionTwoCharacterEntites;
            List<int> AllIndexList = new List<int>();
            for (int i = 0; i < targetCharacterList.Count; i++)
            {
                AllIndexList.Add(i);
            }
            if (targetIDList != null) {
                for (int i = 0; i < targetIDList.Count; i++)
                {
                    AllIndexList.Remove(targetCharacterList.FindIndex((k) => k.UniqueID == targetIDList[i]));
                    resultList.Add(targetIDList[i]);
                }

            }

            while (resultList.Count < count)
            {
                int randomValue = Utility.Algorithm.CreateRandomInt(0, AllIndexList.Count);
                int value = AllIndexList[randomValue];
                resultList.Add(targetCharacterList[value].UniqueID);
                AllIndexList.RemoveAt(randomValue);
                if (AllIndexList.Count == 0)
                    break;
            }
            return resultList;
        }

        public void Clear()
        {
        }
    }
}
