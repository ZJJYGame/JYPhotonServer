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
