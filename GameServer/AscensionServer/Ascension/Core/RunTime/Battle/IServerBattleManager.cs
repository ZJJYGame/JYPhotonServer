using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using Cosmos;
namespace AscensionServer
{
    public interface IServerBattleManager:IModuleManager
    {

        #region 针对所有工具
        /// <summary>
        /// 针对战斗中的随机数
        /// </summary>
        /// <param name="ov"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        int RandomManager(int ov, int minValue, int maxValue);

        /// <summary>
        /// 判断技能功法秘术是不是存在json 数据表格里
        /// </summary>
        /// <param name="targetId"></param>
        /// <returns></returns>
        bool IsToSkillForm(int targetId);

        /// <summary>
        /// 返回 一个存在的技能对象
        /// </summary>
        /// <param name="targerId"></param>
        /// <returns></returns>
        object SkillFormToSkillObject(int targerId);
   
        /// <summary>
        /// 针对 道具中得 丹药和符箓
        /// </summary>
        /// <param name="targetId"></param>
        /// <returns></returns>
        object PropsInstrutionFormToObject(int targetId);
     
        /// <summary>
        /// 针对 法宝
        /// </summary>
        /// <param name="targetId"></param>
        /// <returns></returns>
        MagicWeaponData MagicWeaponFormToObject(int targetId);

        /// <summary>
        /// 不同技能行为的Cmd
        /// </summary>
        /// <param name="battleCmd"></param>
        /// <param name="roleId"></param>
        /// <param name="roomId"></param>
        void SkillActionDifferentCmd(BattleCmd battleCmd, int roleId, int roomId);
        #region 2020.11.06 11:29 
        /// <summary>
        /// 判断释放的技能是不是存在json中
        /// </summary>
        /// <param name="targetId"></param>
        /// <returns></returns>
        BattleSkillData SkillFormToObject(int targetId);
        /// <summary>
        /// 判断ai 是不是存在json中
        /// </summary>
        MonsterDatas MonsterFormToObject(int targetId);
        /// <summary>
        /// 返回给客户端的计算伤害
        /// </summary>
        List<TargetInfoDTO> ServerToClientResult(TargetInfoDTO targetInfo);
        /// <summary>
        /// 计算多段伤害用的
        /// </summary>
        /// <param name="targetInfo"></param>
        /// <returns></returns>
        TargetInfoDTO ServerToClientResults(TargetInfoDTO targetInfo);
        #endregion
        #endregion

        #region 处理战斗中所有单人的情况
        /// <summary>
        /// 2020.11.06 12:00
        /// 筛选出来存活的Ai
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="info"></param>
        /// <param name="skillGongFa"></param>
        void AlToSurvival(BattleTransferDTO battleTransferDTOs, int roleId, int info, BattleSkillData battleSkillData);
        /// <summary>
        ///玩家出手 释放技能
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="special"></param>
        /// <param name="petId"></param>
        void PlayerToSKillRelease(BattleTransferDTO battleTransferDTOs, int roleId, int currentId, int special = 0);
        /// <summary>
        /// 2020 . 11 . 06 13：17
        /// 针对功法  玩家释放  不同技能类型的技能计算伤害
        /// </summary>
        void PlayerToSkillDamage(BattleTransferDTO battleTransferDTOs, int roleId, int currentId, BattleSkillData battleSkillData, int special = 0);
        #endregion

        #region Init

        /// <summary>
        ///  角色id 和对应的 房间数据对象  缓存数据  2020 11：30 09.36
        /// </summary>
        Dictionary<int, BattleInitDTO> TeamIdToBattleInitData { get; set; } 
        /// <summary>
        ///  角色id 和对应的 房间数据对象
        /// </summary>
        Dictionary<int, BattleInitDTO> TeamIdToBattleInit { get; set; } 
        /// <summary>
        /// 房间id， 每回合战斗传输数据对象
        /// </summary>
        Dictionary<int, List<BattleTransferDTO>> RoomidToBattleTransfer { get; set; } 
        /// <summary>
        /// 房间id，对应每回合的倒计时     这个是应对 每回合的战斗
        /// </summary>
        Dictionary<int, TimerToManager> RoomidToTimer { get; set; } 
        /// <summary>
        /// 队伍id， 对应每个队伍的倒计时
        /// </summary>
        Dictionary<int, TimerToManager> TeamidToTimer { get; set; }  
        /// <summary>
        /// 回收房间
        /// </summary>
        List<int> OldBattleList { get; set; } 
        /// <summary>
        /// 收集每个回合的传输数据
        /// </summary>
        List<BattleTransferDTO> TeamSet { get; set; }
        
        /// <summary>
        /// 代表的是倒计时  毫秒
        /// </summary>
        int RoleBattleTime { get; set; }

        /// <summary>
        /// 记录单人每回合 房间id
        /// </summary>
        Queue<int> RecordRoomId { get; set; }  
        /// <summary>
        /// 记录队伍id
        /// </summary>
        Queue<int> RecordTeamId { get; set; }  
        /// <summary>
        /// 记录组队的时候房间id
        /// </summary>
        Queue<int> RecordTeamRooomId { get; set; }  
        /// <summary>
        /// 队伍id 和 队伍成员id
        /// </summary>
        Dictionary<int, List<int>> TeamIdToMemberDict { get; set; } 
        /// <summary>
        /// 队伍id 和 房间id
        /// </summary>
        Dictionary<int, int> TeamIdToRoomId { get; set; }  
        /// <summary>
        /// 房间id 对应的每个回合的buff前
        /// </summary>
        List<BattleBuffDTO> BuffToRoomIdBefore { get; set; }
        /// <summary>
        /// 房间id 对应的每个回合的buff后
        /// </summary>
        List<BattleBuffDTO> BuffToRoomIdAfter { get; set; }
        /// <summary>
        /// 映射  Msq
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="roleId"></param>
        /// <returns></returns>
        T MsqInfo<T>(int roleId);
        /// <summary>
        /// 针对宠物  ID  PetID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="roleId"></param>
        /// <returns></returns>
        T MsqInfoPet<T>(int petId, string petParams);

        /// <summary>
        /// 判断是不是在队伍中
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        TeamDTO IsTeamDto(int roleId);
        /// <summary>
        /// 返回初始化 数据对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="roleId"></param>
        /// <returns></returns>
        BattleInitDTO BattleInitObject(int roleId);
        /// <summary>
        /// 返回初始化 数据对象 缓存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="roleId"></param>
        /// <returns></returns>
        BattleInitDTO BattleInitDataObject(int roleId);

        #region 战斗进入初始化
        /// <summary>
        /// 获取玩家 并初始化
        /// </summary>
        /// <param name="roleId"></param>
        List<RoleBattleDataDTO> RoleInfo(int roleId);
        /// <summary>
        /// 获取宠物  并初始化
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        List<PetBattleDataDTO> PetInfo(int roleId);
        /// <summary>
        /// 获取AI 并初始化
        /// </summary>
        /// <param name="enemyId"></param>
        /// <returns></returns>
        List<EnemyBattleDataDTO> EnemyInfo(List<EnemyBattleDataDTO> enemyId);
        /// <summary>
        /// 玩家宠物AI  初始化所有信息  应对出手速度
        /// </summary>
        /// <returns></returns>
        List<BattleDataBase> AllBattleDataDTOsInfo(int roleId, BattleInitDTO battleInitDTO);
        #endregion


        #region 战斗结束 同步数据

        /// <summary>
        /// 针对 玩家战斗结束的同步数据
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roleStatusSever"></param>
        void PlayerBattleEndInfo(int roleId, RoleStatusDTO roleStatusSever);
        #endregion


        #region 服务器返回给客户端   多参数的处理方式
        /// <summary>
        /// 针对 单人和组队  服务器给客户端 发送初始化数据
        /// </summary>
        /// <param name="roleId"></param>
        void InitBattle(int roleId);
        /// <summary>
        /// 战斗初始化 参数服务器 返回给客户端
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Dictionary<byte, object> InitServerToClient(int roleId);
        /// <summary>
        /// 每回合 战斗 技能计算 参数服务器 返回给客户端
        /// </summary>
        /// <returns></returns>
        Dictionary<byte, object> RoundServerToClient();
        /// <summary>
        /// 每回合 战斗 技能逃跑计算 参数服务器返回给客户端
        /// </summary>
        /// <returns></returns>
        Dictionary<byte, object> RoundRunAwayServerToClient(bool isRunAway);
        #endregion
        #endregion

        #region 处理战斗中所有的组队情况
        /// <summary>
        /// /AI伤害计算公式
        /// </summary>
        /// <returns></returns>
        int DamageCalculation(int roleId, int cuurentId, EnemyBattleDataDTO enemySetObject, int mulitity);
 

        /// <summary>
        /// /玩家伤害计算公式
        /// </summary>
        /// <returns></returns>
        int DamageCalculation(int roleId, int cuurentId, int mulitity, RoleBattleDataDTO owerSetObject = null);




        #region 统一技能的修改

        /// <summary>
        /// 2020.11.06 20:06
        /// 统一计算技能 单段 多段
        /// </summary>
        void SkillSingleOrStaged(BattleTransferDTO battleTransferDTOs, int roleId, int currentId, BattleSkillData battleSkillData, int special = 0);

        /// <summary>
        /// 技能 添加buff
        /// </summary>
        List<BufferBattleDataDTO> AddBufferMethod(List<BattleSkillAddBuffData> addBuffDataSet, int roleId, int currentId, EnemyBattleDataDTO enemySetObject, int index, List<TargetInfoDTO> targetInfoDTOsSet = null);
        /// <summary>
        /// 技能 移除buff
        /// </summary>
        List<int> RemoveBufferMethod(List<BattleSkillRemoveBuffData> removeBuffDataSet, int roleId, int currentId, int index);
        /// <summary>
        ///技能触发时机
        /// </summary>
        void BattleSkillEventDataMethod(List<BattleSkillEventData> battleSkillEvents, int roleId, int currentId, EnemyBattleDataDTO enemySetObject, int index, List<TargetInfoDTO> targetInfoDTOsSet);
        /// <summary>
        ///  技能 触发来源
        /// </summary>
        /// <param name="battleSkillEvents"></param>
        /// <param name="roleId"></param>
        /// <param name="currentId"></param>
        /// <param name="enemySetObject"></param>
        /// <param name="targetInfoDTOsSet"></param>
        void battleSkillEventTriggerCondition(BattleSkillEventData battleSkillEvents, int roleId, int currentId, EnemyBattleDataDTO enemySetObject, RoleBattleDataDTO playerSetObject, List<TargetInfoDTO> targetInfoDTOsSet);
        /// <summary>
        /// 技能触发条件
        /// </summary>
        /// <param name="battleSkillEvents"></param>
        /// <param name="roleId"></param>
        /// <param name="currentId"></param>
        /// <param name="enemySetObject"></param>
        /// <param name="targetInfoDTOsSet"></param>
        void battleSkillEventTriggerNumSourceType(BattleSkillEventData battleSkillEvents, int roleId, int currentId, EnemyBattleDataDTO enemySetObject, RoleBattleDataDTO playerSetObject, List<TargetInfoDTO> targetInfoDTOsSet);

        void battleSkillEventTriggerNumSourceTypePhysicDefense(BattleSkillEventData battleSkillEvents, int roleId, int currentId, EnemyBattleDataDTO enemySetObject, RoleBattleDataDTO playerSetObject, List<TargetInfoDTO> targetInfoDTOsSet);

        /// <summary>
        /// 技能触发事件类型
        /// </summary>
        /// <param name="battleSkillEvents"></param>
        /// <param name="roleId"></param>
        /// <param name="currentId"></param>
        /// <param name="enemySetObject"></param>
        /// <param name="targetInfoDTOsSet"></param>
        void battleSkillTriggerEventType(BattleSkillEventData battleSkillEvents, int roleId, int currentId, EnemyBattleDataDTO enemySetObject, RoleBattleDataDTO playerSetObject, List<TargetInfoDTO> targetInfoDTOsSet);
        #endregion


        #region 统一技能治疗术

        /// <summary>
        /// 2020.11.06 20:45
        /// 统一技能治疗术
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="currentId"></param>
        /// <param name="battleSkillData"></param>
        void PlayerToSkillReturnBlood(BattleTransferDTO battleTransferDTOs, int roleId, int currentId, BattleSkillData battleSkillData);
        /// <summary>
        /// 2020 11.07 17:21
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        List<int> PlayerToPetID(int roleId);

        /// <summary>
        /// 2020 11.07  18:00
        /// 选择回血的目标
        /// </summary>
        /// <param name="targetInfoDTOsSet"></param>
        /// <param name="typeName"></param>
        /// <param name="objectOwner"></param>
        /// <param name="bSD"></param>
        /// <param name="ol"></param>
        void SelectToTarget(List<TargetInfoDTO> targetInfoDTOsSet, string typeName, object objectOwner, List<BattleSkillDamageNumData> bSD, int ol, List<BattleSkillAddBuffData> addBuffDataSet, List<BattleSkillRemoveBuffData> removeBuffDataSet, int roleId, int currentId);
        #endregion

        #region 统一 技能分类 法宝的具体处理
        /// <summary>
        /// 针对法宝的使用
        /// </summary>
        void PlayerToMagicWeapen(BattleTransferDTO battleTransferDTOs, int roleId, int currentId);
        #endregion

        #region 统一技能分类  道具的使用 符箓和丹药
        void PlayerToPropslnstruction(BattleTransferDTO battleTransferDTOs, int roleId, int currentId);
        /// <summary>
        /// 符箓的使用
        /// </summary>
        void RunesDataToUser(BattleTransferDTO battleTransferDTOs, int roleId, int currentId, RunesData runesData);
        /// <summary>
        /// 丹药的使用
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="drugData"></param>
        void DrugDataToUser(BattleTransferDTO battleTransferDTOs, int roleId, int currentId, DrugData drugData);
        #region  针对丹药的HP  MP Buffer 复活
        void DrugHP(BattleTransferDTO battleTransferDTOs, int roleId, int currentId, DrugData drugData);
        #endregion


        #endregion

        #region 技能分类具体处理
        /// <summary>
        /// 针对  单人逃跑的 返回计算处理
        /// </summary>
        void PlayerToRunAway(BattleTransferDTO battleTransferDTOs, int roleId);
        /// <summary>
        /// 针对 宠物逃跑的 返回计算处理
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="petId"></param>
        void PetToRunAway(BattleTransferDTO battleTransferDTOs, int roleId, int petId);
        /// <summary>
        /// 组队逃跑   可以和并成一个 和单人逃跑的   需要去队伍中标记一下 是不是存在战斗中还是中途退出啦
        /// 需要继续完善   ///TODO
        /// </summary>
        /// speed = -1 的话 代表这回合计算是宠物逃跑
        void PlayerTeamToRunAway(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, int transfer = 0, int speed = 0);
        #endregion

        #region 统一针对 捕捉
        void PlayerToCatchPet(BattleTransferDTO battleTransferDTOs, int roleId, int currentId, MonsterDatas monsterDatas);
        #endregion

        #region 统一针对 召唤
        void PlayerToSummonPet(BattleTransferDTO battleTransferDTOs, int roleId, int currentId, MonsterDatas monsterDatas = null);
        #endregion

        #region  统一针对 防御
        void PlayerToDefend(BattleTransferDTO battleTransferDTOs, int roleId, int currentId);
        #endregion
        #endregion



        #region 所有关于倒计时的开始和回调事件

        //public TimerManager timer;
        /// <summary>
        ///针对每回合  开始倒计时
        /// </summary>
        void TimestampBattleEnd(int roomId);
        /// <summary>
        /// 针对初始化准备加载 倒计时
        /// </summary>
        /// <param name="teamId"></param>
        void TimestampBattlePrepare(int teamId);
        /// <summary>
        /// 针对组队 开始之前倒计时
        /// </summary>
        /// <param name="teamId"></param>
        void TimestampBattleStart(int teamId);
        /// <summary>
        ///每个回合倒计时 AI 玩家 是否死亡 战斗结束 发起事件 
        /// </summary>
        void BattleIsDieCallBack(int roomId);
  
        /// <summary>
        /// 针对组队 战斗准备阶段倒计时  回调事件
        /// </summary>
        /// <param name="tempTeamId"></param>
        void BattleTimerPrepareCallBack(int tempTeamId);
        /// <summary>
        /// 针对 组队情况下的 不选取指令  随机分配一个默认指令     ??? 需要处理 不发消息的时候怎么办
        /// </summary>
        void RoundTeamMember(int teampRoomId, int tempTeamId, int tempRole);
        /// <summary>
        /// 针对每回合组队 技能释放计算 并返回给客户端
        /// </summary>
        /// <param name="tempRole"></param>
        /// <param name="teampRoomId"></param>
        /// <param name="tempTeamId"></param>
        /// 
        int IsTeamRunAway { get; set; }
        bool IsPetTeamRunAway { get; set; }
        void RoundTeamSkillComplete(int tempRole, int teampRoomId, int tempTeamId);
        #endregion

        /// <summary>
        /// buff的入口
        /// </summary>
        /// <param name="buffId"></param>
        /// <param name="roleId"></param>
        /// <param name="playerSetObject"></param>
        /// <param name="enemySetObject"></param>
        void BuffManagerMethod(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, int og, bool isSelf, List<BufferBattleDataDTO> bufferId, int numRounder);

        /// <summary>
        /// 先判断buff 触发的条件
        /// </summary>
        void BuffConditionMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, bool isSelf);

        /// <summary>
        /// 判断buff 触发事件
        /// </summary>
        /// <param name="buffId"></param>
        /// <param name="roleId"></param>
        /// <param name="currentId"></param>
        /// <param name="playerSetObject"></param>
        /// <param name="enemySetObject"></param>
        /// <param name="buffDict"></param>
        void BuffEventMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, bool isSelf);


        #region RolePropertyChange  角色属性改变

        /// <summary>
        ///变动属性类型
        /// </summary>
        /// <param name="buffId"></param>
        /// <param name="roleId"></param>
        /// <param name="currentId"></param>
        /// <param name="playerSetObject"></param>
        /// <param name="enemySetObject"></param>
        /// <param name="buffDict"></param>
        /// <param name="buffEventSet"></param>
        /// <param name="i"></param>
        void BuffEventRolePropertyChangeMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf);

        /// <summary>
        /// 变动的数值来源
        /// </summary>
        /// <param name="buffId"></param>
        /// <param name="roleId"></param>
        /// <param name="currentId"></param>
        /// <param name="playerSetObject"></param>
        /// <param name="enemySetObject"></param>
        /// <param name="buffDict"></param>
        /// <param name="buffEventSet"></param>
        /// <param name="i"></param>
        void BuffEventSourceDataTypeMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf);

        #endregion

        #region BuffPropertyChange buff属性变动   TODO
        void BuffEventBuffPropertyChangeMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf);
        #endregion

        #region ForbiddenBuff  禁用buff列表   TODO
        void BuffEventForbiddenBuffMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf);
        #endregion

        #region RoleStateChange 角色状态改变  TODO
        void BuffEventRoleStateChangeMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf);
        #endregion

        #region UseDesignateSkill 使用指定技能 TODO
        void BuffEventUseDesignateSkillMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf);
        #endregion

        #region DamageOrHeal 伤害或者治疗

        /// <summary>
        /// 伤害 或者治疗
        /// </summary>
        /// <param name="buffId"></param>
        /// <param name="roleId"></param>
        /// <param name="currentId"></param>
        /// <param name="playerSetObject"></param>
        /// <param name="enemySetObject"></param>
        /// <param name="buffDict"></param>

        void BuffEventDamageOrHealMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf);
        /// <summary>
        /// buff作用于的目标具体计算
        /// </summary>
        /// <param name="buffId"></param>
        /// <param name="roleId"></param>
        /// <param name="currentId"></param>
        /// <param name="playerSetObject"></param>
        /// <param name="enemySetObject"></param>
        /// <param name="buffDict"></param>
        /// <param name="buffEventSet"></param>
        /// <param name="i"></param>
        void BuffEventResultsMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf);
        #endregion

        #region Shield 护盾 TODO
        void BuffEventShieldMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf);
        #endregion

        #region DamageReduce 该次伤害减免 TODO
        void BuffEventDamageReduceMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf);
        #endregion

        #region TakeHurtForOther 替他人承担伤害 TODO
        void BuffEventTakeHurtForOtherMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf);
        #endregion

        #region AddBuff 替他人承担伤害 TODO
        void BuffEventAddBuffMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf);
        #endregion

        #region DispelBuff 驱散 TODO
        void BuffEventDispelBuffMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf);
        #endregion

        #region NotResurgence 无法复活 TODO
        void BuffEventNotResurgenceMothed(int buffId, int roleId, int currentId, RoleBattleDataDTO playerSetObject, EnemyBattleDataDTO enemySetObject, Dictionary<int, BattleBuffData> buffDict, List<BattleBuffEventData> buffEventSet, int i, bool isSelf);
        #endregion




        #region 出手速度 以及出手拥有者 以及对Ai的血量判断
        /// <summary>
        /// 出手速度
        /// </summary>
        void ReleaseToSpeed(int roleId);

        /// <summary>
        /// 返回一个出手拥有者  玩家或者AI或者宠物
        /// </summary>
        /// <returns></returns>
        object ReleaseToOwner(int objectID, int objectId, int roleId);

        /// <summary>
        /// 针对AI  血量 >0
        /// </summary>
        List<EnemyBattleDataDTO> AIToHPMethod(int roleId, List<EnemyBattleDataDTO> enemyBattleDatas);

        /// <summary>
        /// 针对玩家  血量 >0
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roleBattleDatas"></param>
        /// <returns></returns>
        List<RoleBattleDataDTO> PlayerToHPMethod(int roleId, int currentRoleId, List<RoleBattleDataDTO> roleBattleDatas);
        /// <summary>
        /// 针对宠物 血量>0
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="petBattleDataDTOs"></param>
        /// <returns></returns>
        List<PetBattleDataDTO> PetToHPMethod(int roleId, List<PetBattleDataDTO> petBattleDataDTOs);
        #endregion


        /// <summary>
        /// 处理AI 判断玩家是不是死亡 和要选择能出手的Ai                ??? TODO第四个参数有待完善
        /// </summary>
        void AIToRelease(BattleTransferDTO battleTransferDTOs, EnemyStatusDTO enemyStatusData, int roleId, int currentId, int transfer = 0);


        /// <summary>
        /// 注意 每次进去之前先保证之前的数据都清除掉
        /// </summary>
        /// <param name="roleId"></param>
        void BattleClear(int roleId);

        /// <summary>
        /// 初始化战斗数据  
        /// </summary>
        void EntryBattle(BattleInitDTO battleInitDTO);
        /// <summary>
        /// 准备指令战斗 
        /// </summary>
        void PrepareBattle(int roleId, int roomId);
        void BattleStart(int roleId, int roomId, BattleTransferDTO battleTransferDTOs);

        /// 战斗结束
        /// </summary>
        void BattleEnd(int RoomId);
        /// <summary>
        /// 战斗逃跑
        /// </summary>
        void BattleRunAway(int roleId, int roomId, BattleTransferDTO battleTransferDTOs);
        /// <summary>
        /// 战斗道具
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roomId"></param>
        /// <param name="battleTransferDTOs"></param>
        void BattlePropsInstrution(int roleId, int roomId, BattleTransferDTO battleTransferDTOs);
        /// <summary>
        /// 战斗法宝
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roomId"></param>
        /// <param name="battleTransferDTOs"></param>
        void BattleMagicWeapen(int roleId, int roomId, BattleTransferDTO battleTransferDTOs);
        /// <summary>
        /// 战斗捕捉
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roomId"></param>
        /// <param name="battleTransferDTOs"></param>
        void BattleCatchPet(int roleId, int roomId, BattleTransferDTO battleTransferDTOs);
        /// <summary>
        /// 战斗召唤
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roomId"></param>
        /// <param name="battleTransferDTOs"></param>
        void BattleSummonPet(int roleId, int roomId, BattleTransferDTO battleTransferDTOs);
        /// <summary>
        /// 战斗防御指令
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roomId"></param>
        /// <param name="battleTransferDTOs"></param>
        void BattleDefend(int roleId, int roomId, BattleTransferDTO battleTransferDTOs);
    }
}
;

