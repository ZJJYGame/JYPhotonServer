﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class SchoolDTO : DataTransferObject
    {
        public virtual int ID { get; set; }
        public virtual int SchoolID { get; set; }
        public virtual int ContributionNow { get; set; }
        public virtual int Hatred { get; set; }
        public virtual int TreasureAtticID { get; set; }
        public virtual int SutrasAtticID { get; set; }
        public virtual int RankingListID { get; set; }
        public virtual int SchoolJob { get; set; }
        public virtual SchoolAward GetAwards { get; set; }
        public virtual int ContributionHistory { get; set; }
        public virtual bool IsSignin { get; set; }
        public override void Clear()
        {
            ID = -1;
            SchoolID = 0;
            ContributionNow = 0;
            Hatred = 0;
            TreasureAtticID = 0;
            SutrasAtticID = 0;
            RankingListID = 0;
            SchoolJob = 0;
            GetAwards = null;
            ContributionHistory = 0;
            IsSignin = false;
        }
        [Serializable]
        public class SchoolAward
        {
            public virtual int SpiritStones { get; set; }
            public virtual int Contribution { get; set; }
        }

    }
}

