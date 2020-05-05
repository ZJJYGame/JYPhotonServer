/*
*Author : xianrenZhang
*Since 	:2020-04-28
*Description  : 用户管理类接口
*/
using System.Collections.Generic;
using AscensionServer.Model;
namespace AscensionServer
{
    public interface IUserManager
    {
        void Add(User user);
        void Update(User user);
        void Remove(User user);
        User GetById(int id);
        User GetByUsername(string username);
        ICollection<User> GetAllUsers();

        bool VerifyUser(string username, string password);//验证用户名和密码

         IList<User> GetUserList();
    }

    
}
