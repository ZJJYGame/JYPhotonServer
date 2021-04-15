using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    public enum OperationCode:byte//区分请求和响应
    {
        #region 修改完成的区域码
        Default = 0,
        Login = 1,
        Logoff=2,
        /// <summary>
        /// 注册账户；
        /// </summary>
        Signup = 3,
        /// <summary>
        /// 游戏中登录模块区域的区域码
        /// </summary>
        LoginArea = 4,
        /// <summary>
        /// 同步当前角色的位置信息，position&rotation 
        /// </summary>
        RoleStatus = 5,
    /// <summary>
    /// 登录角色
    /// </summary>
        LoginRole=6,
        /// <summary>
        /// 登出角色
        /// </summary>
        LogoffRole=7,
        SyncRoleStatus=8,
        SyncRoleAssets=9,
        SyncInventory=10,
        SyncTask=11,
        SyncGameDate=12,
        SyncMiShu=13,
        SyncGongFa=14,
        SyncOnOffLine=15,
        SyncPet=16,
        SyncPetStatus=17,
        SyncRolePet=18,
        SyncBottleneck=19,
        SyncMoveStatus =20,

        /// <summary>
        /// 多人在线区域码；
        /// </summary>
        MultiplayArea=110,
        /// <summary>
        /// 历练区域码；
        /// </summary>
        AdventureArea = 111,
        /// <summary>
        /// 秘境区域码；
        /// </summary>
        SecretArea = 112,

        #endregion


        SyncAlchemy = 21,
        SyncForge=22,
        SyncHerbsField=23,
        SyncSpiritualRunes=24,
        SyncTacticFormation=25,
        SyncPuppet=26,

        /// <summary>
        /// 同步宗门数据，藏宝阁，藏经阁，排行榜
        /// </summary>
        SyncTreasureattic=29,
        SyncSutrasAtticm=30,
        SyncSchool =31,
        SyncRoleSchool=32,
        /// <summary>
        /// 同步资源
        /// </summary>
        SyncResources = 33,
        /// <summary>
        /// 拾取&占用资源
        /// </summary>
        TakeUpResource = 34,
        /// <summary>
        /// 同步怪物位置
        /// </summary>
        SyncMonsterTransform = 35,
        /// <summary>
        ///  历练等高同步场景使用技能码；
        ///  勿动！（Don）
        /// </summary>
        SyncRoleAdventureSkill=36,
        /// <summary>
        /// 刷新宗門藏寶閣藏經閣
        /// </summary>
        SyncSchoolRefresh=37,
        UpdateSchoolRefresh = 38,
        SyncWeapon=39,
        SyncShoppingMall=40,
        /// <summary>
        /// 临时背包
        /// </summary>
        SyncTemInventory = 41,
        /// <summary>
        /// 宠物资质
        /// </summary>
        SyncPetaPtitude= 42,
        /// <summary>
        /// 杂货铺
        /// </summary>
        SyncVareityShop=43,
        /// <summary>
        /// 仙盟列表
        /// </summary>
        SyncImmortalsAlliance=44,
        SyncRoleAlliance=47,
        /// <summary>
        /// 拍卖行
        /// </summary>
        SyncAuction =48,
        SyncApplyForAlliance=49,
        SyncAllianceMember = 50,
        SyncAllianceConstruction=52,
        /// <summary>
        /// 同步玩家个人拍卖物品
        /// </summary>
        SyncRoleAuctionItems =51,
        /// <summary>
        ///仙盟技能修炼
        /// </summary>
        SyncRoleAllianceSkill = 53,
        /// <summary>
        /// 仙盟洞府修炼
        /// </summary>
        SyncRoleAllianceCave = 54,
        /// <summary>
        /// 仙盟丹药兑换
        /// </summary>
        SyncRoleAllianceAlchemy = 55,
        /// <summary>
        /// 仙盟签到
        /// </summary>
        SyncAllianceSignin=56,
        /// <summary>
        /// 修改仙盟名称
        /// </summary>
       SyncAllianceName=57,
       /// <summary>
       /// 同步个人拍卖行关注列表
       /// </summary>
       SyncRoleAuctionAttention=58,
        /// <summary>
        /// 发送日常通知的消息
        /// </summary>
        SyncDailyMessage=59,
        SyncAdventurePlayerInfo=60,
        /// <summary>
        /// 记录历练技能布局
        /// </summary>
        RefreshSkillLayout=61,
        /// <summary>
        /// 同步阵法的创建
        /// </summary>
        SyncCreatTactical = 62,
        /// <summary>
        /// 同步新创建的阵法
        /// </summary>
        SyncGetNewTactical = 63,
        /// <summary>
        /// 同步单个角色的功法
        /// </summary>
        SyncRoleGongfa = 64,
        /// <summary>
        /// 刷新角色所有属性点
        /// </summary>
        RoleStatusGet = 65,
        /// <summary>
        /// 角色属性全回复
        /// </summary>
        RoleStatusFullRecovery = 66,
        /// <summary>
        /// 学习第一本功法
        /// </summary>
        AddFirstGongfa = 67,
        /// <summary>
        ///同步角色所有飞行法器
        /// </summary>
        SyncRoleFlyMagicTool = 68,
        /// <summary>
        /// 同步妖灵精魄
        /// </summary>
        SyncDemonical=69,
        /// <summary>
        /// 同步宠物使用丹药
        /// </summary>
        SyncPetDrugFresh=70,
        /// <summary>
        /// 同步角色副职业
        /// </summary>
        SyncSecondaryJob=71,
        /// <summary>
        /// 加入战斗
        /// </summary>
        JoinBattle = 113,
        /// <summary>
        /// 离开战斗
        /// </summary>
        ExitBattle = 114,
        /// <summary>
        /// 战斗匹配
        /// </summary>
        SyncRoleBattleMatch=115,
        #region 角色宠物
        /// <summary>
        /// 获得角色所有宠物
        /// </summary>
        SyncGetRoleAllPet = 116,
        /// <summary>
        /// 设置宠物出战
        /// </summary>
        SyncSetPetBattle = 117,
        /// <summary>
        /// 放生角色宠物
        /// </summary>
        SyncRemoveRolePet=118,
        /// <summary>
        ///       添加新宠物
        /// </summary>
        SyncAddRolePet = 119,
        #endregion
        /// <summary>
        /// 创建队伍
        /// </summary>
        SyncRoleTeam = 125,
        #region 战斗中用到的Message通知
        /// <summary>
        /// 初始化战斗
        /// </summary>
        SyncBattle = 126,
        /// <summary>
        /// 战斗数据传输
        /// </summary>
        SyncBattleTransfer = 127,
        /// <summary>
        /// 战斗倒计时每回合
        /// </summary>
        SyncBattleRound = 128,
        /// <summary>
        /// 战斗消息通知玩家
        /// </summary>
        SyncBattleMessageRole = 129,
        /// <summary>
        /// 战斗消息通知数据
        /// </summary>
        SyncBattleMessageData = 130,
        /// <summary>
        /// 战斗消息通知结束
        /// </summary>
        SyncBattleMessageEnd= 131,
        /// <summary>
        /// 战斗消息通知开始
        /// </summary>
        SyncBattleMessageStart = 132,
        /// <summary>
        /// 战斗消息通知准备
        /// </summary>
        SyncBattleMessagePrepare = 133,
        /// <summary>
        /// 战斗消息通知逃跑
        /// </summary>
        SyncBattleMessageRunAway = 134,
        /// <summary>
        /// 战斗消息通知道具
        /// </summary>
        SyncBattleMessagePropsInstruction = 135,
        /// <summary>
        /// 战斗消息通知法宝指令
        /// </summary>
        SyncBattleMessageMagicWeapon = 136,
        /// <summary>
        /// 战斗消息通知捕捉指令
        /// </summary>
        SyncBattleMessageCatchPet = 137,
        /// <summary>
        /// 战斗消息通知召唤指令
        /// </summary>
        SyncBattleMessageSummonPet = 138,
        /// <summary>
        /// 战斗消息通知阵法指令
        /// </summary>
        SyncBattleMessageTactical  = 139,
        #endregion
        #region 组队中用到的Message通知
        /// <summary>
        /// 初始化
        /// </summary>
        SyncTeamMessageInit = 140,
        /// <summary>
        /// 加入队伍
        /// </summary>
        SyncTeamMessageJoin = 141,
        /// <summary>
        /// 同意队伍
        /// </summary>
        SyncTeamMessageApply = 142,
        /// <summary>
        /// 拒绝队伍
        /// </summary>
        SyncTeamMessageRefused = 143,
        /// <summary>
        /// 解散队伍
        /// </summary>
        SyncTeamMessageDissolveTeam = 144,
        /// <summary>
        /// 创建队伍
        /// </summary>
        SyncTeamMessageCreate = 145,
        /// <summary>
        /// 队伍踢人
        /// </summary>
        SyncTeamMessageKick = 146,
        /// <summary>
        /// 离开队伍
        /// </summary>
        SyncTeamMessageLevel = 147,
        /// <summary>
        ///  发送消息
        /// </summary>
        SyncTeamMessage = 148 ,
        /// <summary>
        /// 转让队长
        /// </summary>
        SyncTeamMessageTransfer = 149,
        /// <summary>
        /// 加好友
        /// </summary>
        SyncTeamMessageFriend = 150,
        /// <summary>
        /// 调整站位
        /// </summary>
        SyncTeamMessagePosition = 151,
        /// <summary>
        /// 委任指挥
        /// </summary>
        SyncTeamMessageCommand = 152,
        /// <summary>
        /// 退出队伍
        /// </summary>
        SyncTeamMessageExit = 153,
        /// <summary>
        /// 自动匹配
        /// </summary>
        SyncTeamMessageMatch = 154,
        #endregion
        /// <summary>
        /// 同步统一修炼
        /// </summary>
        SyncPractice =160,
        /// <summary>
        /// 测试消息队列
        /// </summary>
        MessageQueue = 187,
        /// <summary>
        /// 网关token
        /// </summary>
        Token=243,
        /// <summary>
        /// 心跳
        /// </summary>
        HeartBeat = 244,
        /// <summary>
        /// 子操作码
        /// </summary>
        SubOpCodeData = 254,
        SubOperationCode = 255
         
    }
}
