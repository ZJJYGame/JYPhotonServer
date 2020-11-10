using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using NHibernate.Linq.Clauses;
using NHibernate.Util;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// 处理战斗中所有的组队情况
/// </summary>
namespace AscensionServer
{
    public partial class ServerBattleManager
    {
        /// <summary>
        /// 组队逃跑   可以和并成一个 和单人逃跑的   需要去队伍中标记一下 是不是存在战斗中还是中途退出啦
        /// 需要继续完善
        /// </summary>
        /// speed = -1 的话 代表这回合计算是宠物逃跑
        public void PlayerTeamToRunAway(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, int transfer = 0, int speed = 0)
        {
            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
            tempTrans.TargetID = currentRole;
            if (speed == -1)
                tempTrans.TargetHPDamage = _teamIdToBattleInit[roleId].petUnits[transfer].PetStatusDTO.PetHP > 0 ? 1 : 0;
            else
                tempTrans.TargetHPDamage = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP > 0 ? 1 : 0;
            if (tempTrans.TargetHPDamage == 1)
            {
                if (speed == -1)
                {
                    isPetTeamRunAway = true;
                    _teamIdToBattleInit[roleId].petUnits.Remove(_teamIdToBattleInit[roleId].petUnits.Find(x => x.PetStatusDTO.PetID == currentRole));
                    //_teamIdToBattleInit[roleId].battleUnits.RemoveAt(_teamIdToBattleInit[roleId].battleUnits.FindIndex(x => x.ObjectId == currentRole));
                    _roomidToBattleTransfer[_teamIdToBattleInit[roleId].RoomId].RemoveAt(_roomidToBattleTransfer[_teamIdToBattleInit[roleId].RoomId].FindIndex(x => x.petBattleTransferDTO.RoleId == currentRole));
                }
                else
                {
                    isTeamRunAway++;
                   // _teamIdToBattleInit[roleId].playerUnits.RemoveAt(_teamIdToBattleInit[roleId].playerUnits.FindIndex(x => x.RoleStatusDTO.RoleID == currentRole));
                    //TODO   需要下午该一下
                    //_roomidToBattleTransfer[_teamIdToBattleInit[roleId].RoomId].RemoveAt(_roomidToBattleTransfer[_teamIdToBattleInit[roleId].RoomId].FindIndex(x => x.RoleId == currentRole));

                    //var petTempId=  _teamIdToBattleInit[roleId].petUnits.Find(x => x.RoleId == currentRole);
                    // _teamIdToBattleInit[roleId].petUnits.RemoveAt(_teamIdToBattleInit[roleId].petUnits.FindIndex(x => x.RoleId == currentRole));
                    //_teamIdToBattleInit[roleId].battleUnits.RemoveAt(_teamIdToBattleInit[roleId].battleUnits.FindIndex(x => x.ObjectId == currentRole));
                    //_teamIdToBattleInit[roleId].battleUnits.RemoveAt(_teamIdToBattleInit[roleId].battleUnits.FindIndex(x => x.ObjectId == petTempId.PetStatusDTO.PetID));
                }
            }
            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
            TargetInfosSet.Add(tempTrans);
            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
        }

    }
}
