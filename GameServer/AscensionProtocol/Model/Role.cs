using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.Model.UserClient
{
    public class Role
    {
        public virtual string RoleName { get; set; }
        public virtual string Gender { get; set; }
        public virtual string GoldSpiritRoot { get; set; }
        public virtual string WoodSpiritRoot { get; set; }
        public virtual string WaterSpiritRoot { get; set; }
        public virtual string FireSpiritRoot { get; set; }
        public virtual string SoilSpiritRoot { get; set; }
    }
}
