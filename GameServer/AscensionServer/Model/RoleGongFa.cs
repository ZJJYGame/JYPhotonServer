/*
*Author : xianrenZhang
*Since :	2020-04-28
*Description :  功法
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class RoleGongFa:Model
    {
        public virtual int RoleID { get; set; }
        public virtual string GongFaIDArray { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            GongFaIDArray = null;
        }
    }
}
