﻿/*
*Author : yingduan
*Since :	2020-05-14
*Description :  秘术
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class MiShu
    {
        public virtual int ID { get; set; }
        public virtual byte MiShuID { get; set; }
        public virtual int MiShuExp { get; set; }
        public virtual short MiShuLevel { get; set; }
        public virtual string MiShuSkillArry { get; set; }
    }
}