using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
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
        /// 映射  Msq
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public T MsqInfo<T>(int roleId)
        {
            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleId);
            return NHibernateQuerier.CriteriaSelect<T>(nHCriteriaRoleID);
        }

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
                battleInit.petUnits = battleInitDTO.petUnits;
                battleInit.enemyUnits = battleInitDTO.enemyUnits;
                battleInit.battleUnits = battleInitDTO.battleUnits;
                battleInit.maxRoundCount = battleInitDTO.maxRoundCount;
                _teamIdToBattleInit.Add(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, battleInit);
                _roomidToBattleTransfer.Add(battleInit.RoomId, new List<BattleTransferDTO>());

            }
        }

        /// <summary>
        /// 获取玩家
        /// </summary>
        /// <param name="roleId"></param>
        public List<RoleBattleDataDTO> RoleInfo(int roleId)
        {
            List<RoleBattleDataDTO> roleData = new List<RoleBattleDataDTO>();
            var team = GameManager.CustomeModule<ServerTeamManager>()._teamTOModel.Values.ToList().Find(x => x.TeamMembers.Find(q => q.RoleID == roleId) != null);
            //当前 是不是组队
            if (team == null)
            {
                var status = MsqInfo<RoleStatus>(roleId);
                roleData.Add(new RoleBattleDataDTO(){  
                    ObjectName = MsqInfo<Role>(roleId).RoleName,
                    RoleStatusDTO = new RoleStatusDTO()
                    {
                        RoleID = status.RoleID,
                        RoleHP = status.RoleHP,
                        RoleAttackDamage = status.RoleAttackDamage,
                        RoleAttackPower = status.RoleAttackPower,
                        RoleCrit = (byte)status.RoleCrit,
                        RoleCritResistance = (byte)status.RoleCritResistance,
                        RoleDormant = status.RoleDormant,
                        RoleJingXue = status.RoleJingXue,
                        RoleKillingIntent = status.RoleKillingIntent,
                        RoleMaxHP = status.RoleMaxHP,
                        RoleMaxJingXue = status.RoleMaxJingXue,
                        RoleMaxMP = status.RoleMaxMP,
                        RoleMaxShenhun = status.RoleMaxShenhun,
                        RoleMP = status.RoleMP,
                        RoleResistanceDamage = status.RoleResistanceDamage,
                        RoleResistancePower = status.RoleResistancePower,
                        RoleShenhun = status.RoleShenhun,
                        RoleShenHunDamage = status.RoleShenHunDamage,
                        RoleShenHunResistance = status.RoleShenHunResistance,
                        RoleSpeedAttack = status.RoleSpeedAttack,
                        RoleVileSpawn = status.RoleVileSpawn,
                        RoleVitality = status.RoleVitality
                    }
                });
            }
            else
            {
                for (int i = 0; i < team.TeamMembers.Count; i++)
                {
                    var status = MsqInfo<RoleStatus>(roleId);
                    roleData.Add(new RoleBattleDataDTO()
                    {
                        ObjectName = team.TeamMembers[i].RoleName,
                         RoleStatusDTO  = new RoleStatusDTO()
                         {
                             RoleID = status.RoleID,
                             RoleHP = status.RoleHP,
                             RoleAttackDamage = status.RoleAttackDamage,
                             RoleAttackPower = status.RoleAttackPower,
                             RoleCrit = (byte)status.RoleCrit,
                             RoleCritResistance = (byte)status.RoleCritResistance,
                             RoleDormant = status.RoleDormant,
                             RoleJingXue = status.RoleJingXue,
                             RoleKillingIntent = status.RoleKillingIntent,
                             RoleMaxHP = status.RoleMaxHP,
                             RoleMaxJingXue = status.RoleMaxJingXue,
                             RoleMaxMP = status.RoleMaxMP,
                             RoleMaxShenhun = status.RoleMaxShenhun,
                             RoleMP = status.RoleMP,
                             RoleResistanceDamage = status.RoleResistanceDamage,
                             RoleResistancePower = status.RoleResistancePower,
                             RoleShenhun = status.RoleShenhun,
                             RoleShenHunDamage = status.RoleShenHunDamage,
                             RoleShenHunResistance = status.RoleShenHunResistance,
                             RoleSpeedAttack = status.RoleSpeedAttack,
                             RoleVileSpawn = status.RoleVileSpawn,
                             RoleVitality = status.RoleVitality
                         }
                    });
                }
            }
            return roleData;
        }
        
        /// <summary>
        /// 获取宠物
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<PetaPtitudeDTO> PetInfo(int roleId)
        {
            List<PetaPtitudeDTO> petData = new List<PetaPtitudeDTO>();
            var team = GameManager.CustomeModule<ServerTeamManager>()._teamTOModel.Values.ToList().Find(x => x.TeamMembers.Find(q => q.RoleID == roleId) != null);
            //当前 是不是组队

            return null;
        }

        /// <summary>
        /// 准备指令战斗 
        /// </summary>
        public void PrepareBattle(int roomId)
        {
            if (_roomidToBattleTransfer.ContainsKey(roomId))
            {
                _roomidToBattleTransfer[roomId][0].IsStart= true;
            }
        }

        /// <summary>
        /// 开始战斗   -->  开始战斗个回合
        /// </summary>
        public void BattleStart(int roomId,List<BattleTransferDTO> battleTransferDTOs)
        {

        }
        /// <summary>
        /// 战斗结束
        /// </summary>
        public void BattleEnd()
        {

        }

    }
}
