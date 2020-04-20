using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionServerProtocol.Model
{

    public class User
    {
        public virtual int ID { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual string UUID { get; set; }
        public virtual string Age { get; set; }

    }
    
}
