﻿/*
*Author : yingduan
*Since :	2020-05-14
*Description :  功法
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class GongFa
    {
        public virtual int ID { get; set; }
        public virtual byte GongFaID { get; set; }
        public virtual int GongFaExp { get; set; }
        public virtual short GongFaLevel { get; set; }
        public virtual string GongFaSkillArray { get; set; }
    }
}

