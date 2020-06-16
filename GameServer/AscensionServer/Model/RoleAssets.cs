﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer.Model
{
    [Serializable]
    public class RoleAssets:Model
    {
        public RoleAssets()
        {
            this.SpiritStonesLow = 0;
            this.SpiritStonesHigh = 0;
            this.SpiritStonesMedium = 0;
            this.XianYu = 0;
        }
        public virtual int RoleID { get; set; }
        public virtual long SpiritStonesLow { get; set; }
        public virtual long SpiritStonesMedium { get; set; }
        public virtual long SpiritStonesHigh { get; set; }
        public virtual long XianYu { get; set; }
        public override string ToString()
        {
            return Utility.Text.Format("{ RoleID : " + RoleID + " ; " + " SpiritStonesLow : " + SpiritStonesLow 
                + ";SpiritStonesMedium" + SpiritStonesMedium+ ";SpiritStonesHigh"+ SpiritStonesHigh+ " ; XianYu"+ XianYu+"}");
        }
        public override void Clear()
        {
            RoleID = -1;
            SpiritStonesLow = 0;
            SpiritStonesMedium = 0;
            SpiritStonesHigh = 0;
            XianYu = 0;
        }
    }
}
