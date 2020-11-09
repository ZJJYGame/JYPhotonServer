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

        /// <summary>
        /// 针对 组队道具 符箓和丹药
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        public void PlayerTeamToPropslnstruction(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, int transfer)
        {
            if (PropsInstrutionFormToObject(battleTransferDTOs.ClientCmdId) == null)
                return;
            var objectOwner = PropsInstrutionFormToObject(battleTransferDTOs.ClientCmdId);
            var typeName = objectOwner.GetType().Name;
            switch (typeName)
            {
                case "DrugData":
                    var drugData = objectOwner as DrugData;
                    DrugDataTeamToUser(battleTransferDTOs, roleId, currentRole, transfer, drugData);
                    break;
                case "RunesData":
                    var runesData = objectOwner as RunesData;
                    RunesDataTeamToUser(battleTransferDTOs, roleId, currentRole, runesData, transfer);
                    break;
            }
        }

        /// <summary>
        /// 针对组队的符箓的使用
        /// </summary>
        public void RunesDataTeamToUser(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, RunesData runesData, int transfer)
        {
            if (!IsToSkillForm(runesData.Runes_Skill))
                return;
            battleTransferDTOs.ClientCmdId = runesData.Runes_Skill;
            //PlayerTeamToRelease(battleTransferDTOs, roleId, currentRole, transfer, runesData.Runes_ID);
        }


        /// <summary>
        /// 针对组队的丹药的使用
        /// </summary>
        public void DrugDataTeamToUser(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, int transfer, DrugData drugData)
        {
            switch (drugData.Drug_Type)
            {
                case DrugType.RoleHP:
                    DrugTeamHP(battleTransferDTOs, roleId, currentRole, transfer, drugData);
                    break;
                case DrugType.RoleMP:

                    break;
                case DrugType.RoleBuff:

                    break;

                case DrugType.RoleResurgence:
                    break;
            }
        }
        /// <summary>
        /// 针对丹药的HP  回血    //缺少宠物的判断
        /// </summary>
        public void DrugTeamHP(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, int transfer, DrugData drugData)
        {
            if (drugData.Drug_Use_Target == 1)
            {
                if (_teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP + drugData.Drug_Value >= _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP)
                    _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP;
                else
                    _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP += drugData.Drug_Value;
                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                tempTrans.TargetID = currentRole;
                tempTrans.TargetHPDamage = drugData.Drug_Value;
                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                TargetInfosSet.Add(tempTrans);
                teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
            }
            else
            {
                if (drugData.Drug_Use_Target >= PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits).Count)
                {
                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();

                    for (int ot = 0; ot < _teamIdToBattleInit[roleId].playerUnits.Count; ot++)
                    {
                        if (_teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleHP > 0)
                        {
                            if (_teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleHP + drugData.Drug_Value >= _teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleMaxHP)
                                _teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleMaxHP;
                            else
                                _teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleHP += drugData.Drug_Value;
                            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                            tempTrans.TargetID = _teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleID;
                            tempTrans.TargetHPDamage = drugData.Drug_Value;
                            TargetInfosSet.Add(tempTrans);
                        }
                    }
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                }
                else
                {
                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();

                    if (_teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP + drugData.Drug_Value >= _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP)
                        _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP;
                    else
                        _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP += drugData.Drug_Value;
                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                    tempTrans.TargetID = currentRole;
                    tempTrans.TargetHPDamage = drugData.Drug_Value;
                    TargetInfosSet.Add(tempTrans);
                    for (int oc = 0; oc < drugData.Drug_Use_Target - 1; oc++)
                    {
                        //PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits).Count
                        var IndexOl = new Random().Next(0, PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits).Count);
                        var tempRoleObject = _teamIdToBattleInit[roleId].playerUnits.Find(x => x.RoleStatusDTO.RoleID == PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits)[IndexOl].RoleStatusDTO.RoleID);
                        if (tempRoleObject.RoleStatusDTO.RoleHP + drugData.Drug_Value >= tempRoleObject.RoleStatusDTO.RoleMaxHP)
                            tempRoleObject.RoleStatusDTO.RoleHP = tempRoleObject.RoleStatusDTO.RoleMaxHP;
                        else
                            tempRoleObject.RoleStatusDTO.RoleHP += drugData.Drug_Value;
                        BattleTransferDTO.TargetInfoDTO tempTransOl = new BattleTransferDTO.TargetInfoDTO();
                        tempTransOl.TargetID = tempRoleObject.RoleStatusDTO.RoleID;
                        tempTransOl.TargetHPDamage = drugData.Drug_Value;
                        TargetInfosSet.Add(tempTransOl);
                    }
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                }
            }
        }

    }
}
