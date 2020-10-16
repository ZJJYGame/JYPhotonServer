using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using Google.Protobuf.WellKnownTypes;
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
        /// 房间id，对应每回合的倒计时
        /// </summary>
        public Dictionary<int, TimerToManager> _roomidToTimer = new Dictionary<int, TimerToManager>();
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
        /// 代表的是倒计时  毫秒
        /// </summary>
        public int RoleBattleTime = 20000;

        /// <summary>
        /// 记录房间id
        /// </summary>
        public Queue<int> RecordRoomId = new System.Collections.Generic.Queue<int>();
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

        public List<EnemyBattleDataDTO> EnemyInfo(List<EnemyBattleDataDTO> enemyId)
        {
            int enemyGlobleId = 10000000;
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
                            EnemyAlert_Area = monsterDict[enemyId[i].GlobalId].Alert_area,
                            EnemyAttact_Physical = monsterDict[enemyId[i].GlobalId].Attact_physical,
                            EnemyAttact_Power = monsterDict[enemyId[i].GlobalId].Attact_power,
                            EnemyAttact_Soul = monsterDict[enemyId[i].GlobalId].Attact_soul,
                            EnemyAttact_Speed = monsterDict[enemyId[i].GlobalId].Attact_speed,
                            EnemyBest_Blood = monsterDict[enemyId[i].GlobalId].Best_blood,
                            EnemyDefence_Physical = monsterDict[enemyId[i].GlobalId].Defence_physical,
                            EnemyDefend_Power = monsterDict[enemyId[i].GlobalId].Defence_power,
                            EnemyDefend_Soul = monsterDict[enemyId[i].GlobalId].Defence_soul,
                            EnemyDescribe = monsterDict[enemyId[i].GlobalId].Monster_describe,
                            EnemyDown_Double = monsterDict[enemyId[i].GlobalId].Down_double,
                            EnemyDrop_Array = monsterDict[enemyId[i].GlobalId].Drop_array,
                            EnemyDrop_Rate = monsterDict[enemyId[i].GlobalId].Drop_rate,
                            EnemyLevel = monsterDict[enemyId[i].GlobalId].Monster_level,
                            EnemyMonster_Icon = monsterDict[enemyId[i].GlobalId].Monster_icon,
                            EnemyMoster_Model = monsterDict[enemyId[i].GlobalId].Moster_model,
                            EnemyMove_Speed = monsterDict[enemyId[i].GlobalId].Move_speed,
                            EnemyName = monsterDict[enemyId[i].GlobalId].Monster_name,
                            EnemyPet_ID = monsterDict[enemyId[i].GlobalId].Pet_ID,
                            EnemyPet_Level_ID = monsterDict[enemyId[i].GlobalId].Pet_Level_ID,
                            EnemySkill_Array = monsterDict[enemyId[i].GlobalId].Skill_array,
                            EnemyUp_Double = monsterDict[enemyId[i].GlobalId].UP_double,
                            EnemyValue_Flow = monsterDict[enemyId[i].GlobalId].Value_flow,
                            EnemyValue_Hide = monsterDict[enemyId[i].GlobalId].Value_hide
                        }
                    });
                }
            }
            return enemyData;
        }



        /// <summary>
        /// 初始化所有信息
        /// </summary>
        /// <returns></returns>
        public List<BattleDataBase> AllBattleDataDTOsInfo(int roleId, BattleInitDTO battleInitDTO)
        {
            int enemyGlobleId = 10000000;
            List<BattleDataBase> allDataBase = new List<BattleDataBase>();
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, MonsterDatas>>(out var monsterDict);
            var status = MsqInfo<RoleStatus>(roleId);
            if (IsTeamDto(roleId) == null)
            {
                allDataBase.Add(new BattleDataBase() { ObjectName = MsqInfo<Role>(roleId).RoleName, ObjectHP = status.RoleHP, ObjectID = status.RoleID, ObjectMP = status.RoleMP, ObjectSpeed = status.RoleSpeedAttack });
                for (int i = 0; i < battleInitDTO.enemyUnits.Count; i++)
                {
                    allDataBase.Add(new BattleDataBase()
                    {
                        ObjectId = enemyGlobleId++,
                        ObjectID = monsterDict[battleInitDTO.enemyUnits[i].GlobalId].Monster_ID,
                        ObjectHP = monsterDict[battleInitDTO.enemyUnits[i].GlobalId].Role_HP,
                        ObjectMP = monsterDict[battleInitDTO.enemyUnits[i].GlobalId].Role_MP,
                        ObjectName = monsterDict[battleInitDTO.enemyUnits[i].GlobalId].Monster_name,
                        ObjectSpeed = (int)monsterDict[battleInitDTO.enemyUnits[i].GlobalId].Attact_speed,
                    });
                }
            }
            return allDataBase;
        }


        /// <summary>
        /// 出手速度
        /// </summary>
        public void ReleaseToSpeed(int roleId)
        {
            if (_teamIdToBattleInit.ContainsKey(roleId))
                _teamIdToBattleInit[roleId].battleUnits = _teamIdToBattleInit[roleId].battleUnits.OrderByDescending(t => t.ObjectSpeed).ToList();
        }

        /// <summary>
        /// 返回一个出手拥有者 
        /// </summary>
        /// <returns></returns>
        public object ReleaseToOwner(int objectID, int objectId, int roleId)
        {
            //Utility.Debug.LogInfo("<出手速度>" + objectID + "<>" + objectId + "<>" + roleId);
            if (_teamIdToBattleInit[roleId].playerUnits.Find(t => (t.RoleStatusDTO.RoleID == objectID)) != null)
                return _teamIdToBattleInit[roleId].playerUnits.Find(t => (t.RoleStatusDTO.RoleID == objectID)).RoleStatusDTO;
            if (_teamIdToBattleInit[roleId].enemyUnits.Find(t => (t.EnemyStatusDTO.EnemyId == objectId)) != null)
                return _teamIdToBattleInit[roleId].enemyUnits.Find(t => (t.EnemyStatusDTO.EnemyId == objectId)).EnemyStatusDTO;
            return null;
        }

        //public TimerManager timer;
        /// <summary>
        /// 开始倒计时
        /// </summary>
        public void Timestamp(int roomId)
        {
            _roomidToTimer[roomId].StartTimer();
        }

    }
}

/// <summary>
///针对 倒计时结束 回调方法 
/// </summary>
public delegate void MyDelegateHandle();
