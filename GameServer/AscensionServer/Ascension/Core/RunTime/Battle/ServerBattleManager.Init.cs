using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using NHibernate.Linq.Clauses;

namespace AscensionServer
{
    /// <summary>
    /// 用于初始化 战斗数据 和管理 战斗的数据缓存
    /// </summary>
    public partial class ServerBattleManager
    {
        /// <summary>
        ///  角色id 和对应的 房间数据对象
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
        /// 收集每个回合的传输数据
        /// </summary>
        public List<BattleTransferDTO> teamSet = new List<BattleTransferDTO>();
        /// <summary>
        /// 收集每个回合 每个目标的传输数据
        /// </summary>
        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
        /// <summary>
        /// 收集每个回合 玩家传输数据
        /// </summary>
        List<BattleTransferDTO.TargetInfoDTO> PlayerInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
        /// <summary>
        /// 收集准备战斗指令
        /// </summary>
        List<int> teamIdList = new List<int>();
        /// <summary>
        ///玩家的行动目标  怪物的唯一id 对应全局id
        /// </summary>
        Dictionary<int, int> TargetID = new Dictionary<int, int>();
        /// <summary>
        /// 记录过程伤害
        /// </summary>
        //public List<int> ProcessDamageSet = new List<int>();
        /// <summary>
        /// 房间id
        /// </summary>
        int _roomId = 1000;

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
        /// 判断是不是在队伍中
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public TeamDTO IsTeamDto(int roleId)
        {
            return GameManager.CustomeModule<ServerTeamManager>()._teamTOModel.Values.ToList().Find(x => x.TeamMembers.Find(q => q.RoleID == roleId) != null);
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
                roleData.Add(new RoleBattleDataDTO()
                {
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
                }) ;
            }
            else
            {
                for (int i = 0; i < team.TeamMembers.Count; i++)
                {
                    var status = MsqInfo<RoleStatus>(roleId);
                    roleData.Add(new RoleBattleDataDTO()
                    {
                        ObjectName = team.TeamMembers[i].RoleName,
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
            }
            return roleData;
        }

        /// <summary>
        /// 获取宠物
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<PetBattleDataDTO> PetInfo(int roleId)
        {
            List<PetBattleDataDTO> petData = new List<PetBattleDataDTO>();
            var team = GameManager.CustomeModule<ServerTeamManager>()._teamTOModel.Values.ToList().Find(x => x.TeamMembers.Find(q => q.RoleID == roleId) != null);
            var statusPet = MsqInfo<PetStatus>(roleId);

            //当前 是不是组队
            if (team == null)
            {
                petData.Add(new PetBattleDataDTO()
                {
                    ObjectName = MsqInfo<Pet>(roleId).PetName,
                     RoleId = roleId,
                    PetStatusDTO = new PetStatusDTO()
                    {
                        PetID = statusPet.PetID,
                        PetAbilityPower = statusPet.PetAbilityPower,
                        PetAttackDamage = statusPet.PetAttackDamage,
                        PetHP = statusPet.PetHP,
                        PetMaxHP = statusPet.PetMaxHP,
                        PetMaxMP = statusPet.PetMaxMP,
                        PetMaxShenhun = statusPet.PetMaxShenhun,
                        PetMP = statusPet.PetMP,
                        PetResistanceAttack = statusPet.PetResistanceAttack,
                        PetResistancePower = statusPet.PetResistancePower,
                        PetShenhun = statusPet.PetShenhun,
                        PetShenhunDamage = statusPet.PetShenhunDamage,
                        PetShenhunResistance = statusPet.PetShenhunResistance,
                        PetSpeed = statusPet.PetSpeed,
                        PetTalent = statusPet.PetTalent
                    }
                });
            }
            else
            {
                for (int i = 0; i < team.TeamMembers.Count; i++)
                {
                    petData.Add(new PetBattleDataDTO()
                    {
                        ObjectName = team.TeamMembers[i].RoleName,
                        RoleId = roleId,
                        PetStatusDTO = new PetStatusDTO()
                        {
                            PetID = statusPet.PetID,
                            PetAbilityPower = statusPet.PetAbilityPower,
                            PetAttackDamage = statusPet.PetAttackDamage,
                            PetHP = statusPet.PetHP,
                            PetMaxHP = statusPet.PetMaxHP,
                            PetMaxMP = statusPet.PetMaxMP,
                            PetMaxShenhun = statusPet.PetMaxShenhun,
                            PetMP = statusPet.PetMP,
                            PetResistanceAttack = statusPet.PetResistanceAttack,
                            PetResistancePower = statusPet.PetResistancePower,
                            PetShenhun = statusPet.PetShenhun,
                            PetShenhunDamage = statusPet.PetShenhunDamage,
                            PetShenhunResistance = statusPet.PetShenhunResistance,
                            PetSpeed = statusPet.PetSpeed,
                            PetTalent = statusPet.PetTalent
                        }
                    });
                }
            }
            return petData;
        }

        int enemyGlobleId = 10000000;
        public List<EnemyBattleDataDTO> EnemyInfo(List<EnemyBattleDataDTO> enemyId)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, MonsterDatas>>(out var monsterDict);
            List<EnemyBattleDataDTO> enemyData = new List<EnemyBattleDataDTO>();
            for (int i = 0; i < enemyId.Count; i++)
            {
                if (monsterDict.ContainsKey(enemyId[i].GlobalId))
                {
                    enemyData.Add(new EnemyBattleDataDTO()
                    {
                        GlobalId = enemyId[i].GlobalId,
                        ObjectName = monsterDict[enemyId[i].GlobalId].Monster_name,
                        EnemyStatusDTO = new EnemyStatusDTO()
                        {
                            EnemyId = enemyGlobleId++,
                            EnemyHP = monsterDict[enemyId[i].GlobalId].Role_HP,
                            EnemyMP = monsterDict[enemyId[i].GlobalId].Role_MP,
                            EnemyShenHun = monsterDict[enemyId[i].GlobalId].Role_soul,
                            EnemyJingXue = 0
                        }
                    });
                }
            }
            return enemyData;
        }

    }
}
