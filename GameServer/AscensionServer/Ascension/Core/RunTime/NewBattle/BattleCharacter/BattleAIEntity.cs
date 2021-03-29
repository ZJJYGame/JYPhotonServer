﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;

namespace AscensionServer
{
    public class BattleAIEntity:BattleCharacterEntity
    {
        public void InitAI(int aIID, int uniqueID,BattleFactionType battleFactionType)
        {
            Init();
            CharacterBattleData = CosmosEntry.ReferencePoolManager.Spawn<CharacterBattleData>();
            GameEntry.DataManager.TryGetValue<Dictionary<int, MonsterDatas>>(out var monsterDict);
            if (monsterDict.ContainsKey(aIID))
                CharacterBattleData.Init(monsterDict[aIID]);
            UniqueID = uniqueID;
            GlobalID = aIID;
            BattleFactionType= battleFactionType;
            Name = monsterDict[aIID].Monster_Name;
        }

        public override T ToBattleDataBase<T>()
        {
            T t = new EnemyBattleDataDTO()
            {
                GlobalId = GlobalID,
                ObjectName = Name,
                EnemyStatusDTO = new EnemyStatusDTO
                {
                    EnemyId = UniqueID,
                    EnemyMaxHP = CharacterBattleData.MaxHp,
                    EnemyHP = CharacterBattleData.Hp,
                    EnemyMaxMP = CharacterBattleData.MaxMp,
                    EnemyMP = CharacterBattleData.Mp,
                    EnemyMaxSoul = CharacterBattleData.MaxSoul,
                    EnemySoul = CharacterBattleData.Soul,
                    EnemyBest_Blood = (short)CharacterBattleData.BestBloodMax,
                }
            } as T;
            return t;
        }

        public override void Clear()
        {
        }
    }
}