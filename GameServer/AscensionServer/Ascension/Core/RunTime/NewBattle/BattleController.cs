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
        /// <param name="count"></param>
        /// <param name="battleSkillFactionType"></param>
        /// <param name="targetIsAlive">目标是否是活着的状态</param>
        /// <param name="targetIDList"></param>
        /// <returns></returns>
        public List<int> RandomGetTarget(int count,BattleFactionType battleSkillFactionType,bool targetIsAlive,List<int> targetIDList=null)
        {
            List<int> resultList = new List<int>();
            List<BattleCharacterEntity> targetCharacterList;
            if (battleSkillFactionType == BattleFactionType.FactionOne)
                targetCharacterList = FactionOneCharacterEntites;
            else
                targetCharacterList = FactionTwoCharacterEntites;
            //筛选出未死亡或者死亡的目标列表
            List<BattleCharacterEntity> battleCharacterEntities = new List<BattleCharacterEntity>();
            for (int i = 0; i < targetCharacterList.Count; i++)
            {
                if (targetCharacterList[i].HasDie && !targetIsAlive)
                    battleCharacterEntities.Add(targetCharacterList[i]);
                else if (!targetCharacterList[i].HasDie && targetIsAlive)
                    battleCharacterEntities.Add(targetCharacterList[i]);
            }
            targetCharacterList = battleCharacterEntities;

            //开始随机目标
            List<int> AllIndexList = new List<int>();
            for (int i = 0; i < targetCharacterList.Count; i++)
            {
                AllIndexList.Add(i);
            }
            if (targetIDList != null) {
                for (int i = 0; i < targetIDList.Count; i++)
                {
                    //判断当前选中目标是否可以作为目标
                    BattleCharacterEntity tempEntity = targetCharacterList.Find((k) => k.UniqueID == targetIDList[i]);
                    if ((tempEntity.HasDie && targetIsAlive)||(!tempEntity.HasDie&&!targetIsAlive))
                        continue;
                    AllIndexList.Remove(targetCharacterList.IndexOf(tempEntity));
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

        /// <summary>
        /// 判断战斗是否结束
        /// 双方阵营有一边全部死亡则代表战斗结束
        /// </summary>
        /// <returns></returns>
        public bool BattleIsEnd()
        {
            bool resultFlagOne = true;
            for (int i = 0; i < FactionOneCharacterEntites.Count; i++)
            {
                resultFlagOne = resultFlagOne && FactionOneCharacterEntites[i].HasDie;
            }
            bool resultFlagTwo = true;
            for (int i = 0; i < FactionTwoCharacterEntites.Count; i++)
            {
                resultFlagTwo = resultFlagTwo && FactionTwoCharacterEntites[i].HasDie;
            }
            return resultFlagOne || resultFlagTwo;
        }

        public void Clear()
        {
        }
    }
}
