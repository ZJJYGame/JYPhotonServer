using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AscensionServer.Model;
namespace AscensionServer
{
    /// <summary>
    /// 用户登录后的缓存
    /// </summary>
  public  class PeerCache
    {
        public bool IsLogged { get; set; }
        public string PreviousScene { get; set; }
        public string CurrentScene { get; set; }
        public string Account { get; set; }
        public string Password{ get; set; }
        public string UUID { get; set; }
        public virtual int RoleID { get; set; }
        public bool EqualUser(object obj)
        {
            User user = obj as User;
            return this.Account == user.Account 
                && this.Password == user.Password 
                && this.UUID == user.UUID;
        }
    }
}
