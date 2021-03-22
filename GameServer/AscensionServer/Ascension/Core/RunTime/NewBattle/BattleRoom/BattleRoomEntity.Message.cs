using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol;
using AscensionProtocol.DTO;
using Cosmos;
using AscensionProtocol;

namespace AscensionServer
{
    public partial class BattleRoomEntity
    {
        /// <summary>
        /// 发送战斗初始化信息给客户端
        /// </summary>
        public void SendInitDataS2C()
        {
            BattleInitDTO battleInitDTO = new BattleInitDTO();
            battleInitDTO.playerUnits = new List<RoleBattleDataDTO>();
            battleInitDTO.enemyUnits = new List<EnemyBattleDataDTO>();
            battleInitDTO.petUnits = new List<PetBattleDataDTO>();
            for (int i = 0; i < battleController.FactionOneCharacterEntites.Count; i++)
            {
                if (battleController.FactionOneCharacterEntites[i].GetType().Name == typeof(BattlePlayerEntity).Name)
                {
                    battleInitDTO.playerUnits.Add(battleController.FactionOneCharacterEntites[i].ToBattleDataBase<RoleBattleDataDTO>());
                }
                else if (battleController.FactionOneCharacterEntites[i].GetType().Name == typeof(BattleAIEntity).Name)
                {
                    battleInitDTO.enemyUnits.Add(battleController.FactionOneCharacterEntites[i].ToBattleDataBase<EnemyBattleDataDTO>());
                }
                else if (battleController.FactionOneCharacterEntites[i].GetType().Name == typeof(BattlePetEntity).Name)
                {
                    battleInitDTO.petUnits.Add(battleController.FactionOneCharacterEntites[i].ToBattleDataBase<PetBattleDataDTO>());
                }
            }
            for (int i = 0; i < battleController.FactionTwoCharacterEntites.Count; i++)
            {
                if (battleController.FactionTwoCharacterEntites[i].GetType().Name == typeof(BattlePlayerEntity).Name)
                {
                    battleInitDTO.playerUnits.Add(battleController.FactionTwoCharacterEntites[i].ToBattleDataBase<RoleBattleDataDTO>());
                }
                else if (battleController.FactionTwoCharacterEntites[i].GetType().Name == typeof(BattleAIEntity).Name)
                {
                    battleInitDTO.enemyUnits.Add(battleController.FactionTwoCharacterEntites[i].ToBattleDataBase<EnemyBattleDataDTO>());
                }
                else if (battleController.FactionTwoCharacterEntites[i].GetType().Name == typeof(BattlePetEntity).Name)
                {
                    battleInitDTO.petUnits.Add(battleController.FactionTwoCharacterEntites[i].ToBattleDataBase<PetBattleDataDTO>());
                }
            }
            battleInitDTO.countDownSec = 15;
            battleInitDTO.roundCount = 1;
            battleInitDTO.maxRoundCount = 30;
            Dictionary<byte, object> subResponseParametersDict = new Dictionary<byte, object>();
            subResponseParametersDict.Add((byte)ParameterCode.Role, Utility.Json.ToJson(battleInitDTO));
            OperationData opData = new OperationData();
            opData.DataMessage = subResponseParametersDict;
            opData.OperationCode = (byte)OperationCode.SyncBattleMessageRole;
            SendMessageToAllPlayerS2C(opData);
        }
        void SendBattleMessagePrepareData()
        {
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncBattleMessagePrepare;
            SendMessageToAllPlayerS2C(opData);
        }
        /// <summary>
        /// 发送数据给战斗房间中所有玩家
        /// </summary>
        void SendMessageToAllPlayerS2C(OperationData opData)
        {
            playerSendMsgEvent?.Invoke(opData);
        }
    }
}
