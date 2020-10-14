namespace AscensionProtocol
{
    /// <summary>
    ///对象数据参数码
    /// </summary>
    public enum ParameterCode : byte
    {
        HeartBeat = 0,
        ForcedOffline = 1,
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
        PetPtitude= 35,
        /// <summary>
        /// 单个的位置信息
        /// 表示一个角色的单个位置点
        /// </summary>
        RoleTransfrom=36,
        /// <summary>
        /// 位置信息的集合
        /// RoleTransfrom为此集合的子集
        /// </summary>
        RoleTransfromSet = 37,
        Task =39,
        RoleBottleneck=40,
        RoleMoveStatus =41,
        TreasureAttic=42,
        SutrasAtticm=43,
        School =44,
        RoleSchool=45,
   
        RoleMoveStatusSet =50,
        /// <summary>
        /// 位置信息的集合队列
        /// 表示单位时间内记录的位置点队列集合
        /// 集合元素为 RoleTransformQueue 
        /// </summary>
        RoleTransformQueueSet = 51,
        /// <summary>
        /// 单个位置信息的队列
        /// 表示单个角色在单位时间内所记录的位置点队列
        /// </summary>
        RoleTransformQueue= 52,
        /// <summary>
        /// 资源单位
        /// </summary>
        ResourcesUnit = 53,
        /// <summary>
        /// 占用的单元
        /// 全局ID+分配的ID
        /// </summary>
        OccupiedUnit= 54,
        /// <summary>
        /// 表示资源单位的集合；
        /// 一个全局ID下存储了当前这个全局ID的对象集合
        /// </summary>
        ResourcesUnitSet = 55,
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
        /// 位置信息的集合队列
        /// 表示单位时间内记录的位置点队列集合
        /// 集合元素为 RoleTransformQueue 
        /// </summary>
        MonsterTransformQueueSet = 63,
        /// <summary>
        /// 单个位置信息的队列
        /// 表示单个角色在单位时间内所记录的位置点队列
        /// </summary>
        MonsterTransformQueue =64,
        /// <summary>
        /// 解除占用
        /// 派发刷新资源
        /// </summary>
        RelieveUnit=66,
        /// <summary>
        /// 历练技能cd
        /// </summary>
        RoleAdventureStartSkill = 67,
        RoleAdventureEndSkill = 68,
        RoleAdventureSkillCD = 69,
        /// <summary>
        /// 宗門藏寶閣藏經閣簽到刷新
        /// </summary>
        SchoolRefresh=70,
        GetWeapon=71,
        GetWeaponindex=72,
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
        ImmortalsAlliance = 78,
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
        RoleBattleTimeStamp = 97
    }
}
