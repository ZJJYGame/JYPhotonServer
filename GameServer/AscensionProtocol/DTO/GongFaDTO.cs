using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class GongFaDTO:ProtocolDTO
    {
        public virtual int ID { get; set; }
        public virtual byte GongFaID { get; set; }
        public virtual int GongFaExp { get; set; }
        public virtual short GongFaLevel { get; set; }
        public virtual string GongFaSkillArray { get; set; }
    }
}
