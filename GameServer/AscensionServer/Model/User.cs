﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    public class User
    {
        public virtual int Id { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual string UUID { get; set; }

        public virtual string RoleName { get; set; }
        public virtual string Gender { get; set; }
        public virtual string GoldSpiritRoot { get; set; }
        public virtual string WoodSpiritRoot { get; set; }
        public virtual string WaterSpiritRoot { get; set; }
        public virtual string FireSpiritRoot { get; set; }
        public virtual string SoilSpiritRoot { get; set; }
    }

}
