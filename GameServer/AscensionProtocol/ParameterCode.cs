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
        RoleTransfrom=36,
        RoleTransfromSet=37,

        Task =38,
        RoleBottleneck=39,
        RoleMoveStatus =40,
        SingleRoleTransformSet=41
    }
}
