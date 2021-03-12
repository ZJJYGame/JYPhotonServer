using System.Collections;
using System.Collections.Generic;
namespace AscensionServer
{
    /// <summary>
    /// 游戏模块分区
    /// </summary>
    public enum GameArea
    {
        None = 0x0,
        /// <summary>
        /// 游戏应用设置；
        /// 十进制：1
        /// </summary>
        GameApplication = 0x1,
        /// <summary>
        /// 登录；
        /// 十进制：2
        /// </summary>
        Login = 0x2,
        /// <summary>
        /// 聊天；
        /// 十进制：3
        /// </summary>
        Chat = 0x3,
        /// <summary>
        /// 角色数据状态；
        /// 十进制：4
        /// </summary>
        RoleStatus = 0x4,
        /// <summary>
        /// 历练；
        /// 十进制：5
        /// </summary>
        Cultivate = 0x5,
        /// <summary>
        /// 灵兽；
        /// 十进制：6
        /// </summary>
        SpiritBeasts = 0x6,
        /// <summary>
        /// 新手引导；
        /// 十进制：7
        /// </summary>
        NoviceGuide = 0x7,
        /// <summary>
        /// 战斗；
        /// 十进制：8
        /// </summary>
        Battle = 0x8,
        /// <summary>
        /// 新宗门；
        /// 十进制：9
        /// </summary>
        Gangs_School = 0x8,
        /// <summary>
        /// 副职业；
        /// 十进制：10
        /// </summary>
        SecondaryJob = 0xA,
        /// <summary>
        /// 背包；
        /// 十进制：11
        /// </summary>
        Inventory = 0xB,
        /// <summary>
        /// 历练；
        /// 十进制：12
        /// </summary>
        Adventure = 0xC,
        /// <summary>
        /// 秘境；
        /// 十进制：13
        /// </summary>
        SecretArea = 0xD,
        /// <summary>
        /// 飞行法器；
        /// 十进制：14
        /// </summary>
        FlyMagicTool = 0xE,
        /// <summary>
        /// 活动；
        /// 十进制：15
        /// </summary>
        Activity = 0xF,
        /// <summary>
        /// 商场；
        /// 十进制：16
        /// </summary>
        Shop = 0x10,
        /// <summary>
        /// 好友；
        /// 十进制：17
        /// </summary>
        Friend = 0x11,
        /// <summary>
        /// 匹配/切磋；
        /// 十进制：18
        /// </summary>
        Match_CompareNotes = 0x12,
        /// <summary>
        /// 仓库；
        /// 十进制：19
        /// </summary>
        Repository = 0x13,
        /// <summary>
        /// 组队；
        /// 十进制：20
        /// </summary>
        Team = 0x14,
        /// <summary>
        /// 货币系统；
        /// 十进制：21
        /// </summary>
        Currency = 0x15,
    }
}