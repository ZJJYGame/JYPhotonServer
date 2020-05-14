using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    /// <summary>
    /// 在线状态DTO，记录当前所在的场景，登录的用户，等等
    /// </summary>
    [Serializable]
  public  class OnlineStateDTO
    {
        public bool IsLogined { get; set; }
        public string PreviousScene { get; set; }
        public string CurrentScene { get; set; }
        public UserDTO User { get; set; }
        public RoleDTO Role { get; set; }
    }
}
