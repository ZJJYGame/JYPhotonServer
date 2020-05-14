﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class GongFaDTO
    {
        public virtual int Id { get; set; }
        public virtual string SkillArray { get; set; }
        public virtual string GongFaId { get; set; }
    }
}
