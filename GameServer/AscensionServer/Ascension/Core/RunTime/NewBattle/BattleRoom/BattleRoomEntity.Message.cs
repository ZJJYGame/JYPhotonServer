using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            for (int i = 0; i < BattleController.FactionOneCharacterEntites.Count; i++)
            {
                if (BattleController.FactionOneCharacterEntites[i].GetType().Name == typeof(BattlePlayerEntity).Name)
                {
                    battleInitDTO.playerUnits.Add(BattleController.FactionOneCharacterEntites[i].ToBattleDataBase<RoleBattleDataDTO>());
                }
                else if (BattleController.FactionOneCharacterEntites[i].GetType().Name == typeof(BattleAIEntity).Name)
                {
                    battleInitDTO.enemyUnits.Add(BattleController.FactionOneCharacterEntites[i].ToBattleDataBase<EnemyBattleDataDTO>());
                }
                else if (BattleController.FactionOneCharacterEntites[i].GetType().Name == typeof(BattlePetEntity).Name)
                {
                    battleInitDTO.petUnits.Add(BattleController.FactionOneCharacterEntites[i].ToBattleDataBase<PetBattleDataDTO>());
                }
            }
            for (int i = 0; i < BattleController.FactionTwoCharacterEntites.Count; i++)
            {
                if (BattleController.FactionTwoCharacterEntites[i].GetType().Name == typeof(BattlePlayerEntity).Name)
                {
                    battleInitDTO.playerUnits.Add(BattleController.FactionTwoCharacterEntites[i].ToBattleDataBase<RoleBattleDataDTO>());
                }
                else if (BattleController.FactionTwoCharacterEntites[i].GetType().Name == typeof(BattleAIEntity).Name)
                {
                    battleInitDTO.enemyUnits.Add(BattleController.FactionTwoCharacterEntites[i].ToBattleDataBase<EnemyBattleDataDTO>());
                }
                else if (BattleController.FactionTwoCharacterEntites[i].GetType().Name == typeof(BattlePetEntity).Name)
                {
                    battleInitDTO.petUnits.Add(BattleController.FactionTwoCharacterEntites[i].ToBattleDataBase<PetBattleDataDTO>());
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
        /// <summary>
        /// 告知客户端所有玩家准备完成
        /// </summary>
        void SendBattleMessagePrepareDataS2C()
        {
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncBattleMessagePrepare;
            SendMessageToAllPlayerS2C(opData);
        }
        /// <summary>
        /// 告知客户端这回合的表演信息
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        public void SendBattlePerformDataS2C(List<BattleTransferDTO> battleTransferDTOs)
        {
            Utility.Debug.LogError(Utility.Json.ToJson(battleTransferDTOs));
            Dictionary<byte, object> subResponseParametersDict = new Dictionary<byte, object>();
            subResponseParametersDict.Add((byte)ParameterCode.RoleBattle, Utility.Json.ToJson(battleTransferDTOs));
            subResponseParametersDict.Add((byte)ParameterCode.RoleBattleTimeStamp, Utility.Json.ToJson(Utility.Time.MillisecondTimeStamp()));
            subResponseParametersDict.Add((byte)ParameterCode.RoleBattleTime, Utility.Json.ToJson(GameEntry.ServerBattleManager.RoleBattleTime));
            subResponseParametersDict.Add((byte)ParameterCode.RoleBattleBefore, Utility.Json.ToJson(new List<BattleBuffDTO>()));
            subResponseParametersDict.Add((byte)ParameterCode.RoleBattleAfter, Utility.Json.ToJson(new List<BattleBuffDTO>()));
            OperationData opData = new OperationData();
            opData.DataMessage = subResponseParametersDict;
            opData.OperationCode = (byte)OperationCode.SyncBattleTransfer;
            SendMessageToAllPlayerS2C(opData);
        }
        /// <summary>
        /// 告知客户端新回合开始
        /// </summary>
        public void SendNewRoundStartMsgS2C()
        {
            Utility.Debug.LogError("发送新回合开始的消息");
            OperationData opData = new OperationData();
            opData.DataMessage = "服务器通知=>新回合开始";
            opData.OperationCode = (byte)OperationCode.SyncBattleRound;
            SendMessageToAllPlayerS2C(opData);
        }
        /// <summary>
        /// 告知客户端战斗结束
        /// </summary>
        public void SendBattleEndMsgS2C()
        {
            Utility.Debug.LogError("发送战斗结束的消息");
            OperationData opData = new OperationData();
            opData.DataMessage = "服务器通知=>战斗结束";
            opData.OperationCode = (byte)OperationCode.SyncBattleMessageEnd;
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
