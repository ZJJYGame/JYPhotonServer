using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using Google.Protobuf.WellKnownTypes;
using NHibernate.Linq.Clauses;
using Protocol;

namespace AscensionServer
{
    public partial class ServerBattleManager
    {
        #region 出手速度 以及出手拥有者 以及对Ai的血量判断
        /// <summary>
        /// 出手速度
        /// </summary>
        public void ReleaseToSpeed(int roleId)
        {
            if (_teamIdToBattleInit.ContainsKey(roleId))
                _teamIdToBattleInit[roleId].battleUnits = _teamIdToBattleInit[roleId].battleUnits.OrderByDescending(t => t.ObjectSpeed).ToList();

            foreach (var item in _teamIdToBattleInit[roleId].battleUnits)
            {
                Utility.Debug.LogInfo("老陆 ，出手速度" + item.ObjectSpeed + "<>" + item.ObjectName);
            }
        }

        /// <summary>
        /// 返回一个出手拥有者  玩家或者AI或者宠物
        /// </summary>
        /// <returns></returns>
        public object ReleaseToOwner(int objectID, int objectId, int roleId)
        {
            //Utility.Debug.LogInfo("<出手速度>" + objectID + "<>" + objectId + "<>" + roleId);
            if (_teamIdToBattleInit[roleId].playerUnits.Find(t => (t.RoleStatusDTO.RoleID == objectID)) != null)
                return _teamIdToBattleInit[roleId].playerUnits.Find(t => (t.RoleStatusDTO.RoleID == objectID)).RoleStatusDTO;
            if (_teamIdToBattleInit[roleId].enemyUnits.Find(t => (t.EnemyStatusDTO.EnemyId == objectId)) != null)
                return _teamIdToBattleInit[roleId].enemyUnits.Find(t => (t.EnemyStatusDTO.EnemyId == objectId)).EnemyStatusDTO;
            if (_teamIdToBattleInit[roleId].petUnits.Find(t => (t.ObjectId == objectId)) != null)
                return _teamIdToBattleInit[roleId].petUnits.Find(t => (t.ObjectId == objectId)).PetStatusDTO;
            return null;
        }

        /// <summary>
        /// 针对AI  血量 >0
        /// </summary>
        public List<EnemyBattleDataDTO> AIToHPMethod(int roleId, List<EnemyBattleDataDTO> enemyBattleDatas)
        {
            List<EnemyBattleDataDTO> tempDataSet = new List<EnemyBattleDataDTO>();
            for (int i = 0; i < enemyBattleDatas.Count; i++)
            {
                if (_teamIdToBattleInit[roleId].enemyUnits[i].EnemyStatusDTO.EnemyHP > 0)
                    tempDataSet.Add(_teamIdToBattleInit[roleId].enemyUnits[i]);
            }
            return tempDataSet;
        }

        /// <summary>
        /// 针对玩家  血量 >0
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roleBattleDatas"></param>
        /// <returns></returns>
        public List<RoleBattleDataDTO> PlayerToHPMethod(int roleId, int currentRoleId, List<RoleBattleDataDTO> roleBattleDatas)
        {
            List<RoleBattleDataDTO> tempDataSet = new List<RoleBattleDataDTO>();
            for (int i = 0; i < roleBattleDatas.Count; i++)
            {
                if (_teamIdToBattleInit[roleId].playerUnits[i].RoleStatusDTO.RoleID == currentRoleId)
                    continue;
                if (_teamIdToBattleInit[roleId].playerUnits[i].RoleStatusDTO.RoleHP > 0)
                    tempDataSet.Add(_teamIdToBattleInit[roleId].playerUnits[i]);
            }
            return tempDataSet;
        }
        /// <summary>
        /// 针对宠物 血量>0
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="petBattleDataDTOs"></param>
        /// <returns></returns>
        public List<PetBattleDataDTO> PetToHPMethod(int roleId, List<PetBattleDataDTO> petBattleDataDTOs)
        {
            List<PetBattleDataDTO> tempDataSet = new List<PetBattleDataDTO>();
            for (int i = 0; i < petBattleDataDTOs.Count; i++)
                if (_teamIdToBattleInit[roleId].petUnits[i].PetStatusDTO.PetHP > 0)
                    tempDataSet.Add(_teamIdToBattleInit[roleId].petUnits[i]);
            return tempDataSet;
        }
        #endregion


        /// <summary>
        /// 处理AI 判断玩家是不是死亡 和要选择能出手的Ai                ??? TODO第四个参数有待完善
        /// </summary>
        public void AIToRelease(BattleTransferDTO battleTransferDTOs, EnemyStatusDTO enemyStatusData, int roleId,int  transfer = 0)
        {
            BattleTransferDTO.TargetInfoDTO tempTransEnemy = new BattleTransferDTO.TargetInfoDTO();
            //Utility.Debug.LogInfo("<enemyStatusData  老陆>" + _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP);
            if ((IsTeamDto(roleId) == null))
            {
                //Utility.Debug.LogInfo("我来随机啦====>>>" + isPetRunAway);
                ///TODO  需要怪物的技能表格 释放技能
                if (_teamIdToBattleInit[roleId].petUnits.Count !=0&& _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP>0&& !isPetRunAway)
                {
                    //Utility.Debug.LogInfo("我来随机啦+++++>>>" + new Random((int)DateTime.Now.Ticks+transfer).Next(0, 2));
                    var RandomTarget = RandomManager(transfer,0,2);
                    //if ()
                    //    RandomTarget= RandomTarget == 0 ? 1 : 0;
                    var target = RandomTarget == 0 ? _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP -= 100 : _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP -= 100;
                    tempTransEnemy.TargetID = RandomTarget == 0 ? roleId : _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetID;
                    tempTransEnemy.TargetHPDamage = -100; //-skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                    List<BattleTransferDTO.TargetInfoDTO> PlayerInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    PlayerInfosSet.Add(tempTransEnemy);
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = enemyStatusData.EnemyId, ClientCmdId = 21001, TargetInfos = PlayerInfosSet });
                }
                else
                {
                    _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP -= 100;
                    tempTransEnemy.TargetID = roleId;
                    tempTransEnemy.TargetHPDamage = -100; //-skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                    List<BattleTransferDTO.TargetInfoDTO> PlayerInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    PlayerInfosSet.Add(tempTransEnemy);
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = enemyStatusData.EnemyId, ClientCmdId = 21001, TargetInfos = PlayerInfosSet });
                }
               
            }
            else
            {
                //Utility.Debug.LogInfo("老陆 ，=>" + _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleID);
                var petObject = _teamIdToBattleInit[roleId].petUnits.Find(x => x.PetStatusDTO.PetHP > 0);
                if (_teamIdToBattleInit[roleId].petUnits.Count != 0 && petObject != null&& !isPetTeamRunAway)
                {
                    isPetTeamRunAway = false;
                    var RandomTarget = RandomManager(transfer, 0, 2);
                    var target = RandomTarget == 0 ? _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP -= 100 : petObject.PetStatusDTO.PetHP -= 100;
                    tempTransEnemy.TargetID = RandomTarget == 0 ? _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleID : petObject.PetStatusDTO.PetID;
                    tempTransEnemy.TargetHPDamage = -100;
                    List<BattleTransferDTO.TargetInfoDTO> PlayerInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    PlayerInfosSet.Add(tempTransEnemy);
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = enemyStatusData.EnemyId, ClientCmdId = 21001, TargetInfos = PlayerInfosSet });
                }
                else
                {
                    _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP -= 100;
                    tempTransEnemy.TargetID = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleID;
                    tempTransEnemy.TargetHPDamage = -100;//-skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                    List<BattleTransferDTO.TargetInfoDTO> PlayerInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    PlayerInfosSet.Add(tempTransEnemy);
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = enemyStatusData.EnemyId, ClientCmdId = 21001, TargetInfos = PlayerInfosSet });
                }
            }
        }

    }
}
