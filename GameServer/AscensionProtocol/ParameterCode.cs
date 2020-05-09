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
        RoleNameList=67
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

        public enum LingGen : byte
        {
            LingGenList, //灵根列表
            Gold,//金
            Wood,//木
            Water,//水
            Fire,//火
            Soil//土
        }
    }
}
