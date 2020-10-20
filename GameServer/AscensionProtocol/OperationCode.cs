using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    public enum OperationCode:byte//区分请求和响应
    {
        Default =0,
        Login = 1,
        Logoff=2,
        Register=3,
        /// <summary>
        /// 同步当前这个角色的数据
        /// </summary>
        SyncRole=4,
        /// <summary>
        /// 同步当前角色的位置信息，position&rotation 
        /// </summary>
        PlayerInputCommand = 5,
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


        SyncAlchemy=21,
        SyncForge=22,
        SyncHerbsField=23,
        SyncSpiritualRunes=24,
        SyncTacticFormation=25,
        SyncPuppet=26,
        /// <summary>
        /// 同步自身位置的集合，参考消息队列
        /// </summary>
        SyncSelfRoleTransformQueue = 28,
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
        /// 占用资源单位
        /// </summary>
        OccupiedResourceUnit = 34,
        /// <summary>
        /// 同步怪物位置
        /// </summary>
        SyncMonsterTransform = 35,
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
        /// <summary>
        /// 进入高同步场景的指令
        /// </summary>
        EnterLevelCommand= 111,
        /// <summary>
        /// 离开高同步场景的指令
        /// </summary>
        ExitLevelCommand=112,
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
        /// <summary>
        /// 创建队伍
        /// </summary>
        SyncRoleTeam = 125,
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
        /// 测试消息队列
        /// </summary>
        MessageQueue =137,
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
