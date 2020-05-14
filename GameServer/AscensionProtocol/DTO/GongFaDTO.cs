using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class GongFaDTO:ProtocolDTO
    {
        public virtual int Id { get; set; }
        public virtual string SkillArray { get; set; }
        public virtual int GongFaId { get; set; }
    }
}
