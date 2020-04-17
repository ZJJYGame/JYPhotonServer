/*
*Author   Don
*Since 	2020-04-17
*Description 玩家用映射模型
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.Model
{
    public class User
    {
        public virtual int ID { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual string UUID { get; set; }
    }
}
