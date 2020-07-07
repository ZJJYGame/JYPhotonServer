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
        RoleStatusSet = 19,
        Role = 20,
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
        JobAlchemy=38,
        Task =38,
        RoleBottleneck=39,
        RoleMoveStatus =40,
        TreasureAttic=41,
        School=42,
        RoleSchool=43,
   
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
        RoleTransformQueue= 52
    }
}
