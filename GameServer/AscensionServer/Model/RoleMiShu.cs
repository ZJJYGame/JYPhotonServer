/*
*Author : yingduan
*Since :	2020-05-14
*Description :  角色秘术 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class RoleMiShu:Model
    {
        public virtual int RoleID { get; set; }
        public virtual string MiShuIDArray { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            MiShuIDArray = null;
        }
    }
}
