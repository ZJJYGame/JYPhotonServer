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
    public class ServerBattleManager : Module<ServerBattleManager>
    {
        //public Dictionary<int,int> 
        /*
         * 
         * 3.
         * 2.加载resource json数据表
         * 1.需要怎么存，怎么管理*/

        /// <summary>
        /// 队伍id， 战斗初始化对象
        /// </summary>
        public Dictionary<int, BattleInitDTO> _teamIdToBattleInit = new Dictionary<int, BattleInitDTO>();
        /// <summary>
        /// 房间id， 每回合战斗传输数据对象
        /// </summary>
        public Dictionary<int, List<BattleTransferDTO>> _roomidToBattleTransfer = new Dictionary<int, List<BattleTransferDTO>>();
        /// <summary>
        /// 回收房间
        /// </summary>
        public List<int> _oldBattleList = new List<int>();
        /// <summary>
        /// 房间id
        /// </summary>
        int _roomId = 1000;


        /// <summary>
        /// 初始化战斗数据
        /// </summary>
        public void EntryBattle(BattleInitDTO battleInitDTO,TeamDTO teamid)
        {
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
                _teamIdToBattleInit.Add(teamid.TeamId, battleInit);
                _roomidToBattleTransfer.Add(battleInit.RoomId, new List<BattleTransferDTO>());
            }
        }

        /// <summary>
        /// 玩家id
        /// </summary>
        /// <param name="roleId"></param>
        public List<RoleBattleDataDTO> RoleInfo(int roleId)
        {
            List<RoleBattleDataDTO> roleData = new List<RoleBattleDataDTO>();
            var team = GameManager.CustomeModule<ServerTeamManager>()._teamTOModel.Values.ToList().Find(x => x.TeamMembers.Find(q => q.RoleID == roleId) != null);
            //当前 是不是组队
            if (team == null)
            {
                roleData[0].ObjectName = MsqInfo<Role>(roleId).RoleName;
                var status = MsqInfo<RoleStatus>(roleId);
                roleData[0].RoleStatusDTO = new RoleStatusDTO()
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
                };
            }
            else
            {
                for (int i = 0; i < team.TeamMembers.Count; i++)
                {
                    roleData[i].ObjectName = team.TeamMembers[i].RoleName;
                    var status = MsqInfo<RoleStatus>(roleId);
                    roleData[i].RoleStatusDTO = new RoleStatusDTO()
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
                    };
                }
            }
            return roleData;
        }
        /// <summary>
        /// 映射  Msq
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public T MsqInfo<T>(int roleId)
        {
            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleId);
            bool existStatus = NHibernateQuerier.Verify<T>(nHCriteriaRoleID);
            return NHibernateQuerier.CriteriaSelect<T>(nHCriteriaRoleID);
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
        public void BattleStart(int roomId)
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
