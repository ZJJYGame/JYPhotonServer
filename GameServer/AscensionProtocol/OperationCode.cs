﻿using System;
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
        SyncRoleTransform = 5,
        /// <summary>
        /// 同步自己当前账号的所有角色信息
        /// </summary>
        SyncSelfRoles=6,
        SyncSelfRoleTransform=7,
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
        /// <summary>
        /// 拍卖行
        /// </summary>
        SyncAuction=45,
        EnterAdventureScene = 111,
        ExitAdventureScene=112,
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
        /// 测试消息队列
        /// </summary>
        MessageQueue =137,
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
