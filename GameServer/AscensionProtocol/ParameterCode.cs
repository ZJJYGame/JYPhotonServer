namespace AscensionProtocol
{
    /// <summary>
    ///对象数据参数码
    /// </summary>
    public enum ParameterCode : byte
    {
        HeartBeat = 0,
        ForcedOffline = 1,
        MessageQueue=2,
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
        PetStatusSet=35,
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
        OccupiedUnit= 53,
        /// <summary>
        /// 表示资源单位的集合；
        /// 一个全局ID下存储了当前这个全局ID的对象集合
        /// </summary>
        ResourcesUnitSet = 54,
                

        /// <summary>
        /// 副职业占用码
        /// </summary>
        JobAlchemy = 55,
        JobHerbsField=62
    }
}
