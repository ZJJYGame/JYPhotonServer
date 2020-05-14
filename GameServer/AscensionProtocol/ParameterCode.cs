namespace AscensionProtocol
{
    /// <summary>
    ///对象数据参数码
    /// </summary>
    public enum ObjectParameterCode : byte
    {
        Account = 63,
        User = 64,
        RoleStatus = 65,
        Role = 66,
        RoleIDList = 68,
        OnlineState = 69
    }
    /// <summary>
    /// 单条数据参数码
    /// </summary>
    public enum ParameterCode : byte //区分传送数据的时候，参数的类型
    {
        Account,
        Username,
        Password,
        Rolename,
        RoleID,
        Roleposition,
        x, y, z,
        Usernamelist,
        RoleList,
        Playerdatalist
    }
}
