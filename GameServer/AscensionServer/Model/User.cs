/*
*Author : xianrenZhang
*Since :	2020-04-28
*Description :  用户名
*/

using System.Collections.Generic;
using System;
namespace AscensionServer.Model
{
    [Serializable]
    public class User
    {
        public virtual string Account { get; set; }
        public virtual string Password { get; set; }
        public virtual string UUID { get; set; }
    }

}
