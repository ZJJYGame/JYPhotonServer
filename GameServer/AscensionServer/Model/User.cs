using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/* Time  2020.4.28
   Content：用户名
   Host: xianrenZhang
     */
namespace AscensionServer.Model
{
    public class User
    {
        public virtual string UUID { get; set; }
        public virtual string Account { get; set; }
        public virtual string Password { get; set; }
    }

}
