﻿namespace AscensionProtocol
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
        OnlineState = 67,
        RoleAssets = 68,

    }
    /// <summary>
    /// 单条数据参数码
    /// </summary>
    public enum ParameterCode : byte
    {
        Account = 0,
        Password = 1,
        RoleName = 2,
        RoleIDList = 3,
        RolePosition = 4,
        x = 5, y = 6, z = 6,
        RoleList = 7,
        PlayerDataList = 8,
        HeartBeat = 9,
        RoleID = 10,
        GongFaExp = 11,
        MiShuExp = 12
    }
}
