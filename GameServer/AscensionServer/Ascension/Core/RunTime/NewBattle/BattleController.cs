using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

namespace AscensionServer
{
    public class BattleController : IReference
    {
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
        public void InitController()
        {
            AllCharacterEntities = new List<BattleCharacterEntity>();
            FactionOneCharacterEntites = new List<BattleCharacterEntity>();
            FactionTwoCharacterEntites = new List<BattleCharacterEntity>();
        }

        public void StartBattle()
        {
            //角色按速度排序
            AllCharacterEntities.Sort();

            //按角色出手顺序开始出手
            int allCount = AllCharacterEntities.Count;
            for (int i = 0; i < allCount; i++)
            {
                Utility.Debug.LogError("玩家" + AllCharacterEntities[i].UniqueID + "的指令是" + AllCharacterEntities[i].BattleCmd + "=>" + AllCharacterEntities[i].ActionID );
            }
        }

        public void Clear()
        {
        }
    }
}
