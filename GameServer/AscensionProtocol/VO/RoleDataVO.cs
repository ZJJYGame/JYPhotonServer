using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/// <summary>
/// 这两个对象需要混合使用
/// </summary>
namespace AscensionProtocol.VO
{
    /// <summary>
    /// 这个数据类型是用来展示其他玩家信息的
    /// </summary>
    public class RoleDataVO:ViewObject
    {
        public int RoleID { get; set; }
        public  byte RoleFaction { get; set; }
        public  string RoleName { get; set; }
        public short GongFaLevel { get; set; }
        public string PersonalizedSignature { get; set; }
        public override void Clear()
        {
            RoleFaction = 0;
            RoleName = null;
            GongFaLevel = 0;
            RoleID = -1;
            PersonalizedSignature = null;
        }
        public RoleDataVO()
        {
            RoleID = -1;
            RoleFaction = 0;
            RoleName= null;
            GongFaLevel = 0;
            PersonalizedSignature = null;
        }
        public override string ToString()
        {
            return "###### RoleID : " + RoleID+ " ; RoleFaction : " +RoleFaction
                + "; RoleName: " + RoleName + "; GongFaLevel: " + GongFaLevel 
                + "; PersonalizedSignature: " + PersonalizedSignature + "######";
        }
    }

}
