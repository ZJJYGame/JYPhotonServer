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
        /// 房间id，对应每回合的倒计时     这个是应对 每回合的战斗
        /// </summary>
        public Dictionary<int, TimerToManager> _roomidToTimer = new Dictionary<int, TimerToManager>();
        /// <summary>
        /// 队伍id， 对应每个队伍的倒计时
        /// </summary>
        public Dictionary<int, TimerToManager> _teamidToTimer = new Dictionary<int, TimerToManager>();
        /// <summary>
        /// 回收房间
        /// </summary>
        public List<int> _oldBattleList = new List<int>();
        /// <summary>
        /// 收集每个回合的传输数据
        /// </summary>
        public List<BattleTransferDTO> teamSet = new List<BattleTransferDTO>();
        /// <summary>
        /// 收集准备战斗指令
        /// </summary>
        List<int> teamIdList = new List<int>();
        /// <summary>
        ///玩家的行动目标  怪物的唯一id 对应全局id
        /// </summary>
        Dictionary<int, int> TargetID = new Dictionary<int, int>();
        /// <summary>
        /// 房间id
        /// </summary>
        int _roomId = 1000;
        /// <summary>
        /// 代表的是倒计时  毫秒
        /// </summary>
        public int RoleBattleTime = 10000;

        /// <summary>
        /// 记录单人每回合 房间id
        /// </summary>
        public Queue<int> RecordRoomId = new System.Collections.Generic.Queue<int>();
        /// <summary>
        /// 记录队伍id
        /// </summary>
        public Queue<int> RecordTeamId = new Queue<int>();
        /// <summary>
        /// 记录组队的时候房间id
        /// </summary>
        public Queue<int> RecordTeamRooomId = new Queue<int>();
        /// <summary>
        /// 队伍id 和 队伍成员id
        /// </summary>
        public Dictionary<int, List<int>> _teamIdToMemberDict = new Dictionary<int, List<int>>();
        /// <summary>
        /// 队伍id 和 房间id
        /// </summary>
        public Dictionary<int, int> _teamIdToRoomId = new Dictionary<int, int>();

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
        /// 针对宠物  ID  PetID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public T MsqInfoPet<T>(int petId,string petParams)
        {
            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue(petParams, petId);
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

      


        #region 战斗进入初始化
        /// <summary>
        /// 获取玩家 并初始化
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
                        RoleMaxHP = status.RoleMaxHP,
                        RoleMaxMP = status.RoleMaxMP,
                        RoleMP = status.RoleMP,
                        AttackPower = status.AttackPower,
                        AttackPhysical = status.AttackPhysical,
                        DefendPhysical = status.DefendPhysical,
                        DefendPower = status.DefendPower,
                        AttackSpeed = status.AttackSpeed,
                        BestBlood = status.BestBlood,
                        ValueHide = status.ValueHide,
                        BestBloodMax = status.BestBloodMax,
                        FreeAttributes = status.FreeAttributes,
                        MagicCritDamage = status.MagicCritDamage,
                        MagicCritProb = status.MagicCritProb,
                        MoveSpeed = status.MoveSpeed,
                        PhysicalCritDamage = status.PhysicalCritDamage,
                        PhysicalCritProb= status.PhysicalCritProb,
                        ReduceCritDamage= status.ReduceCritDamage,
                        ReduceCritProb= status.ReduceCritProb,
                        RoleMaxPopularity= status.RoleMaxPopularity,
                        RoleMaxSoul= status.RoleMaxSoul,
                        RolePopularity= status.RolePopularity,
                        RoleSoul= status.RoleSoul,
                    },
                    AmplifyDamage = 0,
                    BasalEvasionRate = 0,
                    CriteRate = 0,
                    DamageFluctuations = 0,
                    DamageReduction = 0,
                    HitRate = 0,
                    IgnoreDefense = 0,
                    PhysicalDamage = 0,
                    SpellRate = 0,
                    SpellEvasionRate = 0,
                    PhysicalEvasionRate = 0,
                    SpellCrit = 0,
                    PhysicalRate = 0,
                    ReducedDamage = 0
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
                            RoleID = team.TeamMembers[i].RoleID,
                            RoleHP = status.RoleHP,
                            RoleMaxHP = status.RoleMaxHP,
                            RoleMaxMP = status.RoleMaxMP,
                            RoleMP = status.RoleMP,
                            AttackPower = status.AttackPower,
                            AttackPhysical = status.AttackPhysical,
                            DefendPhysical = status.DefendPhysical,
                            DefendPower = status.DefendPower,
                            AttackSpeed = status.AttackSpeed,
                            BestBlood = status.BestBlood,
                            ValueHide = status.ValueHide,
                            BestBloodMax = status.BestBloodMax,
                            FreeAttributes = status.FreeAttributes,
                            MagicCritDamage = status.MagicCritDamage,
                            MagicCritProb = status.MagicCritProb,
                            MoveSpeed = status.MoveSpeed,
                            PhysicalCritDamage = status.PhysicalCritDamage,
                            PhysicalCritProb = status.PhysicalCritProb,
                            ReduceCritDamage = status.ReduceCritDamage,
                            ReduceCritProb = status.ReduceCritProb,
                            RoleMaxPopularity = status.RoleMaxPopularity,
                            RoleMaxSoul = status.RoleMaxSoul,
                            RolePopularity = status.RolePopularity,
                            RoleSoul = status.RoleSoul
                        },
                        AmplifyDamage = 0,
                        BasalEvasionRate = 0,
                        CriteRate = 0,
                        DamageFluctuations = 0,
                        DamageReduction = 0,
                        HitRate = 0,
                        IgnoreDefense = 0,
                        PhysicalDamage = 0,
                        SpellRate = 0,
                        SpellEvasionRate = 0,
                        PhysicalEvasionRate = 0,
                        SpellCrit = 0,
                        PhysicalRate = 0,
                        ReducedDamage = 0
                    });
                }
            }
            return roleData;
        }

        /// <summary>
        /// 获取宠物  并初始化
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<PetBattleDataDTO> PetInfo(int roleId)
        {
            List <PetBattleDataDTO> petData = new List<PetBattleDataDTO>();
            var team = GameManager.CustomeModule<ServerTeamManager>()._teamTOModel.Values.ToList().Find(x => x.TeamMembers.Find(q => q.RoleID == roleId) != null);
            //先判断是否出战
            //Utility.Debug.LogInfo("宠物名字" + MsqInfo<RolePet>(roleId).PetIsBattle);
           
            //当前 是不是组队
            if (team == null)
            {
                if (MsqInfo<RolePet>(roleId).PetIsBattle == 0)
                    return petData;
                //Utility.Debug.LogInfo(MsqInfoPet<Pet>(MsqInfo<RolePet>(roleId).PetIsBattle, "ID").ID);
                var petObject = MsqInfoPet<Pet>(MsqInfo<RolePet>(roleId).PetIsBattle, "ID");
                var statusPet = MsqInfoPet<PetStatus>(petObject.ID, "PetID");

                petData.Add(new PetBattleDataDTO()
                {
                    ObjectName = petObject.PetName,
                    RoleId = roleId,
                    ObjectID = petObject.PetID,
                    ObjectId = petObject.ID,
                    PetStatusDTO = new PetStatusDTO()
                    {
                        PetID = statusPet.PetID,
                        //PetAbilityPower = statusPet.PetAbilityPower,
                        //PetAttackDamage = statusPet.PetAttackDamage,
                        PetHP = statusPet.PetHP,
                        PetMaxHP = statusPet.PetMaxHP,
                        PetMaxMP = statusPet.PetMaxMP,
                        PetMaxShenhun = statusPet.PetMaxShenhun,
                        PetMP = statusPet.PetMP,
                        //PetResistanceAttack = statusPet.PetResistanceAttack,
                        //PetResistancePower = statusPet.PetResistancePower,
                        PetShenhun = statusPet.PetShenhun,
                        //PetShenhunDamage = statusPet.PetShenhunDamage,
                        //PetShenhunResistance = statusPet.PetShenhunResistance,
                        //PetSpeed = statusPet.PetSpeed,
                        //PetTalent = statusPet.PetTalent
                    }
                });
            }
            else
            {
                for (int i = 0; i < team.TeamMembers.Count; i++)
                {
                    if (MsqInfo<RolePet>(team.TeamMembers[i].RoleID).PetIsBattle == 0)
                        continue;
                    var petObject = MsqInfoPet<Pet>(MsqInfo<RolePet>(team.TeamMembers[i].RoleID).PetIsBattle, "ID");
                    var statusPet = MsqInfoPet<PetStatus>(petObject.ID, "PetID");
                    petData.Add(new PetBattleDataDTO()
                    {
                        ObjectName = petObject.PetName,
                         ObjectID = petObject.PetID,
                          ObjectId = petObject.ID,
                        RoleId = team.TeamMembers[i].RoleID,
                        PetStatusDTO = new PetStatusDTO()
                        {
                            PetID = statusPet.PetID,
                            //PetAbilityPower = statusPet.PetAbilityPower,
                            //PetAttackDamage = statusPet.PetAttackDamage,
                            PetHP = statusPet.PetHP,
                            PetMaxHP = statusPet.PetMaxHP,
                            PetMaxMP = statusPet.PetMaxMP,
                            PetMaxShenhun = statusPet.PetMaxShenhun,
                            PetMP = statusPet.PetMP,
                            //PetResistanceAttack = statusPet.PetResistanceAttack,
                            //PetResistancePower = statusPet.PetResistancePower,
                            PetShenhun = statusPet.PetShenhun,
                            //    PetShenhunDamage = statusPet.PetShenhunDamage,
                            //    PetShenhunResistance = statusPet.PetShenhunResistance,
                            //    PetSpeed = statusPet.PetSpeed,
                            //    PetTalent = statusPet.PetTalent
                        }
                    });
                }
            }
            return petData;
        }

       /// <summary>
       /// 获取AI 并初始化
       /// </summary>
       /// <param name="enemyId"></param>
       /// <returns></returns>
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
                        ObjectName = monsterDict[enemyId[i].GlobalId].Monster_Name,
                        EnemyStatusDTO = new EnemyStatusDTO()
                        {
                            EnemyId = enemyGlobleId++,
                            EnemyHP = monsterDict[enemyId[i].GlobalId].Monster_HP,
                            EnemyMP = monsterDict[enemyId[i].GlobalId].Monster_MP,
                            EnemySoul = monsterDict[enemyId[i].GlobalId].Monster_Soul,
                            EnemyAlert_Area = monsterDict[enemyId[i].GlobalId].Alert_Area,
                            EnemyAttact_Physical = monsterDict[enemyId[i].GlobalId].Attact_Physical,
                            EnemyAttact_Power = monsterDict[enemyId[i].GlobalId].Attact_Power,
                            EnemyAttact_Speed = monsterDict[enemyId[i].GlobalId].Attact_Speed,
                            EnemyBest_Blood = monsterDict[enemyId[i].GlobalId].Best_Blood,
                            EnemyDefence_Physical = monsterDict[enemyId[i].GlobalId].Defend_Physical,
                            EnemyDefend_Power = monsterDict[enemyId[i].GlobalId].Defend_Power,
                            EnemyDescribe = monsterDict[enemyId[i].GlobalId].Monster_Describe,
                            EnemyDrop_Array = monsterDict[enemyId[i].GlobalId].Drop_Array,
                            EnemyDrop_Rate = monsterDict[enemyId[i].GlobalId].Drop_Rate,
                            EnemyLevel = monsterDict[enemyId[i].GlobalId].Monster_Level,
                            EnemyMonster_Icon = monsterDict[enemyId[i].GlobalId].Monster_Icon,
                            EnemyMoster_Model = monsterDict[enemyId[i].GlobalId].Moster_Model,
                            EnemyMove_Speed = monsterDict[enemyId[i].GlobalId].Move_Speed,
                            EnemyName = monsterDict[enemyId[i].GlobalId].Monster_Name,
                            EnemyPet_ID = monsterDict[enemyId[i].GlobalId].Pet_ID,
                            EnemyPet_Level_ID = monsterDict[enemyId[i].GlobalId].Pet_Level_ID,
                            EnemySkill_Array = monsterDict[enemyId[i].GlobalId].Skill_Array,
                            EnemyValue_Flow = monsterDict[enemyId[i].GlobalId].Value_Flow,
                            EnemyValue_Hide = monsterDict[enemyId[i].GlobalId].Value_Hide,
                            EnemyMaxHP = monsterDict[enemyId[i].GlobalId].Monster_HP,
                            EnemyMaxMP = monsterDict[enemyId[i].GlobalId].Monster_MP,
                            EnemyMaxSoul = monsterDict[enemyId[i].GlobalId].Monster_Soul,
                            EnemyMagicCritDamage = monsterDict[enemyId[i].GlobalId].MagicCritDamage,
                            EnemyMagicCritProb = monsterDict[enemyId[i].GlobalId].MagicCritProb,
                            EnemyPhysicalCritDamage = monsterDict[enemyId[i].GlobalId].PhysicalCritDamage,
                            EnemyPhysicalCritProb = monsterDict[enemyId[i].GlobalId].PhysicalCritProb,
                            EnemyReduceCritDamage = monsterDict[enemyId[i].GlobalId].ReduceCritDamage,
                            EnemyReduceCritProb = monsterDict[enemyId[i].GlobalId].ReduceCritProb,
                             EnemyGig_Level = monsterDict[enemyId[i].GlobalId].Monster_Gig_Level
                        }
                    });
                }
            }
            return enemyData;
        }

        /// <summary>
        /// 玩家宠物AI  初始化所有信息  应对出手速度
        /// </summary>
        /// <returns></returns>
        public List<BattleDataBase> AllBattleDataDTOsInfo(int roleId, BattleInitDTO battleInitDTO)
        {
            int enemyGlobleId = 10000000;
            List<BattleDataBase> allDataBase = new List<BattleDataBase>();
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, MonsterDatas>>(out var monsterDict);
           
            if (IsTeamDto(roleId) == null)
            {
                ///TODO 应该
                var status = MsqInfo<RoleStatus>(roleId);
                allDataBase.Add(new BattleDataBase() { ObjectName = MsqInfo<Role>(roleId).RoleName, ObjectHP = status.RoleHP, ObjectID = status.RoleID, ObjectMP = status.RoleMP, ObjectSpeed = status.AttackSpeed });
                for (int i = 0; i < battleInitDTO.enemyUnits.Count; i++)
                {
                    allDataBase.Add(new BattleDataBase()
                    {
                        ObjectId = enemyGlobleId++,
                        ObjectID = monsterDict[battleInitDTO.enemyUnits[i].GlobalId].Monster_ID,
                        ObjectHP = monsterDict[battleInitDTO.enemyUnits[i].GlobalId].Monster_HP,
                        ObjectMP = monsterDict[battleInitDTO.enemyUnits[i].GlobalId].Monster_MP,
                        ObjectName = monsterDict[battleInitDTO.enemyUnits[i].GlobalId].Monster_Name,
                        ObjectSpeed = (int)monsterDict[battleInitDTO.enemyUnits[i].GlobalId].Attact_Speed,
                    });
                }
                Utility.Debug.LogInfo("老陆 ，开始战斗的时候收集客户端一个请求");
                if (MsqInfo<RolePet>(roleId).PetIsBattle == 0)
                    return allDataBase;
                var petObject = MsqInfoPet<Pet>(MsqInfo<RolePet>(roleId).PetIsBattle, "ID");
                var statusPet = MsqInfoPet<PetStatus>(petObject.ID, "PetID");
                allDataBase.Add(new BattleDataBase() {  ObjectID = petObject.PetID, ObjectId = petObject.ID, ObjectName = petObject.PetName, ObjectHP = statusPet.PetHP, ObjectMP = statusPet.PetMP, ObjectSpeed = statusPet.AttackSpeed });
            }
            else
            {
                for (int oc = 0; oc < IsTeamDto(roleId).TeamMembers.Count; oc++)
                {
                    var status = MsqInfo<RoleStatus>(IsTeamDto(roleId).TeamMembers[oc].RoleID);
                    allDataBase.Add(new BattleDataBase() { ObjectName = MsqInfo<Role>(IsTeamDto(roleId).TeamMembers[oc].RoleID).RoleName, ObjectHP = status.RoleHP, ObjectID = status.RoleID, ObjectMP = status.RoleMP, ObjectSpeed = status.AttackSpeed });
                }
                for (int i = 0; i < battleInitDTO.enemyUnits.Count; i++)
                {
                    allDataBase.Add(new BattleDataBase()
                    {
                        ObjectId = enemyGlobleId++,
                        ObjectID = monsterDict[battleInitDTO.enemyUnits[i].GlobalId].Monster_ID,
                        ObjectHP = monsterDict[battleInitDTO.enemyUnits[i].GlobalId].Monster_HP,
                        ObjectMP = monsterDict[battleInitDTO.enemyUnits[i].GlobalId].Monster_MP,
                        ObjectName = monsterDict[battleInitDTO.enemyUnits[i].GlobalId].Monster_Name,
                        ObjectSpeed = (int)monsterDict[battleInitDTO.enemyUnits[i].GlobalId].Attact_Speed,
                    });
                }
                
                for (int op = 0; op < IsTeamDto(roleId).TeamMembers.Count; op++)
                {
                    if (MsqInfo<RolePet>(IsTeamDto(roleId).TeamMembers[op].RoleID).PetIsBattle == 0)
                        continue;
                    var petObject = MsqInfoPet<Pet>(MsqInfo<RolePet>(IsTeamDto(roleId).TeamMembers[op].RoleID).PetIsBattle, "ID");
                    var statusPet = MsqInfoPet<PetStatus>(petObject.ID, "PetID");
                    allDataBase.Add(new BattleDataBase() {  ObjectID = petObject.PetID, ObjectId = petObject.ID, ObjectHP = statusPet.PetHP, ObjectMP = statusPet.PetMP, ObjectSpeed = statusPet.AttackSpeed, ObjectName = petObject.PetName});
                }
            }
            return allDataBase;
        }
        #endregion


        #region 战斗结束 同步数据

        /// <summary>
        /// 针对 玩家战斗结束的同步数据
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roleStatusSever"></param>
        public void PlayerBattleEndInfo(int roleId,RoleStatusDTO roleStatusSever)
        {
            if (roleStatusSever.RoleHP<0)
                roleStatusSever.RoleHP = 0;
            if (roleStatusSever.RoleMP < 0)
                roleStatusSever.RoleMP = 0;
            if (roleStatusSever.RoleSoul < 0)
                roleStatusSever.RoleSoul = 0;
            NHibernateQuerier.Update(new RoleStatus()
            {
                RoleID = roleId,
                RoleHP = roleStatusSever.RoleHP,
                RoleMaxMP = roleStatusSever.RoleMP,
                RoleMaxHP = roleStatusSever.RoleMaxHP,
                RoleMP = roleStatusSever.RoleMP,
                AttackPower = roleStatusSever.AttackPower,
                AttackPhysical = roleStatusSever.AttackPhysical,
                DefendPhysical = roleStatusSever.DefendPhysical,
                DefendPower = roleStatusSever.DefendPower,
                AttackSpeed = roleStatusSever.AttackSpeed,
                BestBlood = (short)roleStatusSever.BestBlood,
                ValueHide = roleStatusSever.ValueHide,
                BestBloodMax = (short)roleStatusSever.BestBloodMax,
                FreeAttributes = roleStatusSever.FreeAttributes,
                MagicCritDamage = roleStatusSever.MagicCritDamage,
                MagicCritProb = roleStatusSever.MagicCritProb,
                MoveSpeed = roleStatusSever.MoveSpeed,
                PhysicalCritDamage = roleStatusSever.PhysicalCritDamage,
                PhysicalCritProb = roleStatusSever.PhysicalCritProb,
                ReduceCritDamage = roleStatusSever.ReduceCritDamage,
                ReduceCritProb = roleStatusSever.ReduceCritProb,
                RoleMaxPopularity = roleStatusSever.RoleMaxPopularity,
                RoleMaxSoul = roleStatusSever.RoleMaxSoul,
                RolePopularity = roleStatusSever.RolePopularity,
                RoleSoul = roleStatusSever.RoleSoul
            });
        }

        #endregion


        #region 服务器返回给客户端   多参数的处理方式
        /// <summary>
        /// 针对 单人和组队  服务器给客户端 发送初始化数据
        /// </summary>
        /// <param name="roleId"></param>
        public void InitBattle(int roleId)
        {
            if (IsTeamDto(roleId) == null)
            {
                OperationData opData = new OperationData();
                opData.DataMessage = InitServerToClient(roleId);
                opData.OperationCode = (byte)OperationCode.SyncBattleMessageRole;
                GameManager.CustomeModule<RoleManager>().SendMessage(roleId, opData);
            }
            else
            {
                if (GameManager.CustomeModule<ServerTeamManager>()._teamTOModel.ContainsKey(IsTeamDto(roleId).TeamId))
                {
                    for (int ov = 0; ov < GameManager.CustomeModule<ServerTeamManager>()._teamTOModel[IsTeamDto(roleId).TeamId].TeamMembers.Count; ov++)
                    {
                        OperationData opData = new OperationData();
                        opData.DataMessage = InitServerToClient(roleId);
                        opData.OperationCode = (byte)OperationCode.SyncBattleMessageRole;
                        GameManager.CustomeModule<RoleManager>().SendMessage(GameManager.CustomeModule<ServerTeamManager>()._teamTOModel[IsTeamDto(roleId).TeamId].TeamMembers[ov].RoleID, opData);
                    }
                }
            }
        }

        /// <summary>
        /// 战斗初始化 参数服务器 返回给客户端
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Dictionary<byte,object> InitServerToClient(int roleId)
        {
            Dictionary<byte, object> subResponseParametersDict = new Dictionary<byte, object>();
            subResponseParametersDict.Add((byte)ParameterCode.Role, Utility.Json.ToJson(GameManager.CustomeModule<ServerBattleManager>()._teamIdToBattleInit[roleId]));
            return subResponseParametersDict;
        }

        /// <summary>
        /// 每回合 战斗 技能计算 参数服务器 返回给客户端
        /// </summary>
        /// <returns></returns>
        public Dictionary<byte, object> RoundServerToClient()
        {
            ///返回给客户端
            Dictionary<byte, object> subResponseParametersDict = new Dictionary<byte, object>();
            subResponseParametersDict.Add((byte)ParameterCode.RoleBattle, Utility.Json.ToJson(GameManager.CustomeModule<ServerBattleManager>().teamSet));
            subResponseParametersDict.Add((byte)ParameterCode.RoleBattleCmd, (byte)BattleCmd.SkillInstruction);
            subResponseParametersDict.Add((byte)ParameterCode.RoleBattleTimeStamp, Utility.Json.ToJson(Utility.Time.MillisecondTimeStamp()));
            subResponseParametersDict.Add((byte)ParameterCode.RoleBattleTime, Utility.Json.ToJson(GameManager.CustomeModule<ServerBattleManager>().RoleBattleTime));

            return subResponseParametersDict;
        }

        /// <summary>
        /// 每回合 战斗 技能逃跑计算 参数服务器返回给客户端
        /// </summary>
        /// <returns></returns>
        public Dictionary<byte, object> RoundRunAwayServerToClient(bool isRunAway)
        {
            ///返回给客户端
            Dictionary<byte, object> subResponseParametersDict = new Dictionary<byte, object>();
            subResponseParametersDict.Add((byte)ParameterCode.RoleBattle, Utility.Json.ToJson(GameManager.CustomeModule<ServerBattleManager>().teamSet));
            subResponseParametersDict.Add((byte)ParameterCode.RoleBattleCmd, (byte)BattleCmd.SkillInstruction);
            subResponseParametersDict.Add((byte)ParameterCode.RoleBattleTimeStamp, Utility.Json.ToJson(Utility.Time.MillisecondTimeStamp()));
            subResponseParametersDict.Add((byte)ParameterCode.RoleBattleTime, Utility.Json.ToJson(GameManager.CustomeModule<ServerBattleManager>().RoleBattleTime));

            return subResponseParametersDict;
        }

        #endregion


    }
}

