/*
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
    public class GongFa:Model
    {
        public virtual int ID { get; set; }
        public virtual int GongFaID { get; set; }
        public virtual int GongFaExp { get; set; }
        public virtual short GongFaLevel { get; set; }
        public virtual string GongFaSkillArray { get; set; }
        public override void Clear()
        {
            ID = -1;
            GongFaID = 0;
            GongFaExp = 0;
            GongFaLevel = 0;
            GongFaSkillArray = null;
        }
    }
}

