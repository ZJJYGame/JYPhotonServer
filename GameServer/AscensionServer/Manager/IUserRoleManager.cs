/*
*Author : xianrenZhang
*Since 	:2020-04-28
*Description  : 用户角色中间表管理类接口
*/
using AscensionServer.Model;

namespace AscensionServer
{
    public interface IUserRoleManager
    {
        void AddStr(UserRole str);
        void UpdateStr(UserRole str);
        string GetArray(string uuid);
    }
}
