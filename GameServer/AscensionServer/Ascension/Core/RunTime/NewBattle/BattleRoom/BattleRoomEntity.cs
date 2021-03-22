using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;
using Protocol;

namespace AscensionServer
{
    public partial class BattleRoomEntity : IReference
    {
        //房间内所有玩家的ID集合
        public HashSet<int> AllPlayerID { get; private set; } = new HashSet<int>() ;
        //已收集到指令的玩家ID
        public HashSet<int> HasGetCmdRoleID { get; private set; } = new HashSet<int>();
        //战斗角色字典，key=>角色ID，value=>战斗角色实体对象
        public Dictionary<int, BattleCharacterEntity> battleCharacterEntityDict { get; private set; } = new Dictionary<int, BattleCharacterEntity>();
        BattleController battleController;
        Action<OperationData> playerSendMsgEvent;
        public event Action<OperationData> PlayerSendMsgEvent
        {
            add { playerSendMsgEvent += value; }
            remove { playerSendMsgEvent -= value; }
        }

        /// <summary>
        /// 初始化房间
        /// </summary>
        public void InitRoom(int roomId, BattleInitDTO battleInitDTO)
        {
            battleController = CosmosEntry.ReferencePoolManager.Spawn<BattleController>();
            battleController.InitController();
            //添加玩家
            BattlePlayerEntity battlePlayerEntity = GameEntry.BattleCharacterManager.AddPlayerCharacter(roomId,battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, BattleFactionType.FactionOne);
            if (battlePlayerEntity != null)
            {
                battleController.AddCharacterEntity(battlePlayerEntity);
                AllPlayerID.Add(battlePlayerEntity.UniqueID);
                PlayerSendMsgEvent += battlePlayerEntity.SendMessage;
                battleCharacterEntityDict[battlePlayerEntity.UniqueID] = battlePlayerEntity;
            }
            //添加宠物
            BattlePetEntity battlePetEntity = GameEntry.BattleCharacterManager.AddPetCharacter(roomId,battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, BattleFactionType.FactionOne);
            if (battlePetEntity != null)
            {
                battleController.AddCharacterEntity(battlePetEntity);
                battleCharacterEntityDict[battlePetEntity.UniqueID] = battlePetEntity;
            }
            //添加ai
            BattleAIEntity battleAIEntity;
            for (int i = 0; i < battleInitDTO.enemyUnits.Count; i++)
            {
                battleAIEntity = GameEntry.BattleCharacterManager.AddAICharacter(roomId,battleInitDTO.enemyUnits[i].GlobalId, BattleFactionType.FactionTwo);
                if (battleAIEntity != null)
                {
                    battleController.AddCharacterEntity(battleAIEntity);
                    battleCharacterEntityDict[battleAIEntity.UniqueID] = battleAIEntity;
                }
            }

            SendInitDataS2C();
            StartPrepareWait();
        }
        /// <summary>
        /// 指定角色准备完成
        /// </summary>
        public void CharacterPrepare(int roleID)
        {
            HasGetCmdRoleID.Add(roleID);

            //判断是否房间所有角色准备完成
            if(HasGetCmdRoleID.Count==AllPlayerID.Count)//准备完成
            {
                Utility.Debug.LogError("所有角色准备完成");
                //移除准备计时事件
                GameEntry.BattleRoomManager.TimeAction -= PrepareWait;
                //开始收集指令倒计时
                SendBattleMessagePrepareData();
            }
        }

        public void GetCharacterCmd(int roleID,BattleCmd battleCmd,BattleTransferDTO battleTransferDTO)
        {
            HasGetCmdRoleID.Add(roleID);
            battleCharacterEntityDict[roleID].SetBattleAction(battleCmd, battleTransferDTO);
            //判断是否房间所有角色都接受指令
            if (HasGetCmdRoleID.Count == AllPlayerID.Count)//准备完成
            {
                Utility.Debug.LogError("所有角色指令接收完成");
                //移除战斗指令计时事件
                GameEntry.BattleRoomManager.TimeAction -= GetBattleCmdWait;
                //todo战斗计算
                battleController.StartBattle();
            }
        }
        //开始准备倒计时等待 
        void StartPrepareWait()
        {
            //清理已准备玩家ID容器
            HasGetCmdRoleID.Clear();
            //开始战斗等待计时
            countDownTargetTime = DateTime.Now.AddSeconds(GameEntry.BattleRoomManager.PrepareWaitTime);
            GameEntry.BattleRoomManager.TimeAction += PrepareWait;
        }
        DateTime countDownTargetTime;   
        void PrepareWait()
        {
            if (DateTime.Compare(DateTime.Now,countDownTargetTime)>0)//倒计时到了
            {
                Utility.Debug.LogInfo("准备倒计时结束");
                GameEntry.BattleRoomManager.TimeAction -= PrepareWait;
                //开始收集指令倒计时
                SendBattleMessagePrepareData();
            }
        }
        /// <summary>
        /// 开始收集战斗指令倒计时等待
        /// </summary>
        void StartGetBattleCmdWait()
        {
            //清理已收集到指令的玩家ID容器
            HasGetCmdRoleID.Clear();
            //开始战斗结算等待计时
            countDownTargetTime = DateTime.Now.AddSeconds(GameEntry.BattleRoomManager.RoundTIme);
            GameEntry.BattleRoomManager.TimeAction += GetBattleCmdWait;
        }
        void GetBattleCmdWait()
        {
            if (DateTime.Compare(DateTime.Now, countDownTargetTime) > 0)//倒计时到了
            {
                Utility.Debug.LogInfo("收集战斗指令倒计时结束");
                GameEntry.BattleRoomManager.TimeAction -= GetBattleCmdWait;
                //todo战斗计算
                battleController.StartBattle();
            }
        }
        public void Clear()
        {

        }
    }
}
