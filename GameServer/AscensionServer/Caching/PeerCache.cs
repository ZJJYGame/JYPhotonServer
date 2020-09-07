﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AscensionServer.Model;
using AscensionProtocol.VO;
using AscensionProtocol.DTO;

namespace AscensionServer
{
    /// <summary>
    /// 用户登录后的缓存
    /// </summary>
    [Serializable]
  public  class PeerCache
    {
        //TODO 需要修改嵌套
        // 测试阶段
        public bool IsLogged { get; set; }
        public string Account { get { return User.Account; } set { User.Account = value; } }
        public string Password{ get { return User.Password; } set { User.Password = value; } }
        public string UUID { get { return User.UUID; }set { User.UUID = value; } }
        public int RoleID { get { return Role.RoleID; }set { Role.RoleID = value; } }
        public User User { get; set; }
        public Role Role { get; set; }
        public RoleTransformQueueDTO RoleTransformQueue { get; set; }
        public RoleMoveStatusDTO RoleMoveStatus { get; set; }
        public RoleAdventureSkillDTO RoleAdventureSkill { get; set; }
        public PeerCache()
        {
            User = new User();
            Role = new Role();
            RoleTransformQueue = new RoleTransformQueueDTO();
            RoleMoveStatus = new RoleMoveStatusDTO();
            IsLogged = false;
            Account = null;
            Password = null;
            UUID = null;
            RoleID = -1;
        }
        public bool EqualUser(object obj)
        {
            User user = obj as User;
            return this.Account == user.Account 
                && this.Password == user.Password 
                && this.UUID == user.UUID;
        }
       
    }
}
