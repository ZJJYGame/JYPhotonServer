namespace AscensionProtocol
{
    /// <summary>
    ///对象数据参数码
    /// </summary>
    public enum ParameterCode : byte
    {
        /// <summary>
        /// 服务器会话ID；
        /// </summary>
        ClientPeer=2,
        /// <summary>
        /// 包含账户信息以及设备号的数据
        /// </summary>
        UserInfo = 3,
        Token = 4,
        ///////////////////前16为系统预留////////////////////////

        User = 17,
        RoleStatus = 18,
        /// <summary>
        /// 角色状态的集合
        /// </summary>
        RoleStatusSet = 19,
        Role = 20,
        /// <summary>
        /// 角色的集合
        /// </summary>
        RoleSet = 21,
        RoleAssets = 22,
        GongFa = 23,
        GongFaSet = 24,
        MiShu = 25,
        MiShuSet = 26,
        OnOffLine = 27,
        Inventory = 28,
        RoleMiShu=29,
        RoleGongFa=30,
        RolePet=31,
        Pet=32,
        PetSet=33,
        PetStatus=34,
        PetAptitude= 35,
        PetAbility=36,

        Task =39,
        RoleBottleneck=40,
        TreasureAttic=42,
        SutrasAtticm=43,
        School =44,
        RoleSchool=45,

        /// <summary>
        /// 副职业占用码
        /// </summary>
        JobAlchemy = 56,
        JobForge=57,
        JobSpiritualRunes=58,
        JobTacticFormation=59,
        JobPuppet=60,
        JobHerbsField = 62,
        /// <summary>
        /// 宗門藏寶閣藏經閣簽到刷新
        /// </summary>
        SchoolRefresh=70,
        GetWeapon=71,
        GetPuppetUnit=72,
        ShoppingMall =73,
        RoleTemInventory =74,
        RolePurchase=75,
        /// <summary>
        /// 杂货铺
        /// </summary>
        VareityShop=76,
        VareityPurchase=77,


        /// <summary>
        /// 拍卖行
        /// </summary>
        Auction=79,
        AddAuctionGoods=80,
        /// <summary>
        /// 仙盟
        /// </summary>
        Alliances = 78,
        RoleAlliance=81,
        AllianceMember=82,
        ApplyForAlliance=83,
        RoleAuctionItems=84,
        AllianceConstruction=85,
        RoleAllianceSkill=86,
        RoleAllianceCave=87,
        AllianceSignin=88,
        RoleAllianceAlchemy = 89,
        AllianceName = 90,
        /// <summary>
        /// 组队
        /// </summary>
        RoleTeam =91,
        RoleRingMagic = 92,
        /// <summary>
        /// 广播的日常消息
        /// </summary>
        DailyMessage=93,
        /// <summary>
        /// 战斗
        /// </summary>
        RoleBattle = 94 ,
        RoleBattleCmd = 95,
        /// <summary>
        /// 固定的时间
        /// </summary>
        RoleBattleTime = 96,
        /// <summary>
        /// 时间戳
        /// </summary>
        RoleBattleTimeStamp = 97,
        /// <summary>
        /// 创建阵法
        /// </summary>
        CreatTactical=99,
        /// <summary>
        /// 上架拍卖品
        /// </summary>
        PutAwayAuctionGoods=100,
        /// <summary>
        /// 下架拍卖品
        /// </summary>
        SoldOutAuctionGoods=101,
        /// <summary>
        /// 战斗之前
        /// </summary>
        RoleBattleBefore = 102,
        /// <summary>
        /// 战斗之后
        /// </summary>
        RoleBattleAfter = 103,
        /// <summary>
        /// 获取飞行法器
        /// </summary>
        RoleFlyMagicTool=104,
        /// <summary>
        /// 妖灵精魄
        /// </summary>
        DemonicSoul = 105,
        /// <summary>
        /// 副职业
        /// </summary>
        SecondaryJob=106,
        /// <summary>
        /// 人物加点
        /// </summary>
        RoleStatusPoint=107,
        /// <summary>
        /// 宗门展示数据
        /// </summary>
        AllianceStatus=108,
        /// <summary>
        /// 宗門申請人員
        /// </summary>
        ApplyMember=109,
        /// <summary>
        /// 兑换物品
        /// </summary>
        ExchangeGoods=110,
        /// <summary>
        /// 成员职位数
        /// </summary>
        MemberJobNum = 111,
        /// <summary>
        /// 获得傀儡个体
        /// </summary>
        GetPuppetIndividual=112,
        /// <summary>
        /// 角色所有傀儡
        /// </summary>
        RolePuppet=113,
        /// <summary>
        /// 角色装备信息
        /// </summary>
        RoleEquipment=114,
        /// <summary>
        /// 宠物丹药刷新
        /// </summary>
        PetDrugFresh=115,
    }
}
