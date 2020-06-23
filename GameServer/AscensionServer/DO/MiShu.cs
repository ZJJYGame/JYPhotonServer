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
    public class MiShu:DataObject
    {
        public MiShu()
        {
            MiShuID = 13001;
            MiShuExp = 0;
            MiShuLevel = 1;
            MiShuSkillArry = "";
        }
        public virtual int ID { get; set; }
        public virtual int MiShuID { get; set; }
        public virtual int MiShuExp { get; set; }
        public virtual short MiShuLevel { get; set; }
        public virtual string MiShuSkillArry { get; set; }
        public override void Clear()
        {
            ID = -1;
            MiShuID = 0;
            MiShuExp = 0;
            MiShuLevel = 0;
            MiShuSkillArry = null;
        }
    }
}