using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using UnityEngine;
namespace AscensionServer
{
    /// <summary>
    /// 没统一的服务器战斗功能
    /// </summary>
    [CustomeModule]
    public partial class ServerBattleManager : Module<ServerBattleManager>
    {
        /*
         * 
         * 
         * 
         * */

        /// <summary>
        /// 初始化战斗数据  
        /// </summary>
        public void EntryBattle(BattleInitDTO battleInitDTO)
        {
            var team = GameManager.CustomeModule<ServerTeamManager>()._teamTOModel.Values.ToList().Find(x => x.TeamMembers.Find(q => q.RoleID == battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID) != null);
            if (team != null && team.LeaderId != battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID)
            {
                Utility.Debug.LogInfo("有自己的队伍,但不是队长！！");
                return;
            }

            BattleInitDTO battleInit;
            if (_oldBattleList.Count > 0 )
            {
                //int roomid = _oldBattleList[0];
                //battleInit = _teamIdToBattleInit[roomid];
            }
            else
            {
                battleInit = new BattleInitDTO();
                battleInit.RoomId = _roomId++;
                battleInit.countDownSec = 15;
                battleInit.roundCount = battleInitDTO.roundCount;
                battleInit.playerUnits = RoleInfo(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID);
                battleInit.petUnits = PetInfo(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID);
                battleInit.enemyUnits = EnemyInfo(battleInitDTO.enemyUnits);
                battleInit.enemyPetUnits = PetInfo(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID);
                battleInit.battleUnits = battleInitDTO.battleUnits;
                battleInit.maxRoundCount = battleInitDTO.maxRoundCount;
                _teamIdToBattleInit.Add(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, battleInit);
                _roomidToBattleTransfer.Add(battleInit.RoomId, new List<BattleTransferDTO>());

            }
        }


        /// <summary>
        /// 准备指令战斗 
        /// </summary>
        public void PrepareBattle(int roleId)
        {
            if (IsTeamDto(roleId) == null)
                return;
            else
            {

            }
        }

        /// <summary>
        /// 开始战斗   -->  开始战斗的回合
        /// </summary>
        public void BattleStart(int roomId,BattleTransferDTO battleTransferDTOs)
        {
           

            if (_roomidToBattleTransfer.ContainsKey(roomId))
            {
                teamSet.Add(battleTransferDTOs);
                //缺少一个倒计时
                isFinish = true;
            }



            for (int i = 0; i < teamSet.Count; i++)
            {
                switch (GetSendSkillReactionCmd(roomId, i))
                {
                    case BattleTransferDTO.SkillReactionCmd.BeatBack:
                        break;
                    case BattleTransferDTO.SkillReactionCmd.Guard:
                        break;
                    case BattleTransferDTO.SkillReactionCmd.Dodge:
                        break;
                    case BattleTransferDTO.SkillReactionCmd.Shock:
                        break;
                    case BattleTransferDTO.SkillReactionCmd.Parry:
                        break;
                    default:
                        break;
                }

                //_roomidToBattleTransfer[roomId].Add(teamSet[i]);
            }
            teamSet.Clear();
        }


        private BattleTransferDTO.SkillReactionCmd GetSendSkillReactionCmd(int roomId, int i)
        {
            return _roomidToBattleTransfer[roomId][i].SendSkillReactionCmd;
        }
        private int GetSkillReactionValue(int roomId, int i)
        {
            return _roomidToBattleTransfer[roomId][i].SkillReactionValue;
        }


        /// <summary>
        /// 处理每回合
        /// </summary>
        public void isFinishMethod()
        {

        }
        /// <summary>
        /// 战斗结束
        /// </summary>
        public void BattleEnd()
        {

        }

        public int TotalTime = 15;

        public bool isFinish = false;
        /// <summary>
        /// 每回合 倒计时
        /// </summary>
        public override void OnRefresh()
        {
            //var now =  Utility.Time.SecondNow();
            if (isFinish)
            {
                TotalTime--;
                if (TotalTime == 0)
                {
                    TotalTime = 15;
                    isFinish = false;
                }
            }

        }

    }
}
