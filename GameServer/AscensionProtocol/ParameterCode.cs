namespace AscensionProtocol
{
    /// <summary>
    ///对象参数码
    /// </summary>
    public enum ObjectParameterCode:byte
    {
        User = 64,
        RoleStatus = 65,
        Role=66,
        //完整的角色Json
        RoleData=67,
        PeerID=68,
        RoleID=69
    }
    public class ParameterCode //区分传送数据的时候，参数的类型
    {
        public enum UserCode : byte
        {
            Account,
            Username,
            Password,
            Rolename,
            Roleposition,
            x,y,z,
            Usernamelist,
            RoleList,
            Playerdatalist
        }
    }
}
