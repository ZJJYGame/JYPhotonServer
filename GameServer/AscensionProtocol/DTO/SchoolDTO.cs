using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class SchoolDTO : DataTransferObject
    {
        public int ID { get; set; }
        public int SchoolID { get; set; }
        public int Contribution { get; set; }
        public int Hatred { get; set; }
        public int TreasureAtticID { get; set; }
        public int SutrasAtticID { get; set; }
        public int RankingListID { get; set; }
        public int SchoolJob { get; set; }
        public int SchoolMember { get; set; }


        public override void Clear()
        {
            ID = -1;
            SchoolID = 0;
            Contribution = 0;
            Hatred = 0;
            TreasureAtticID = 0;
            SutrasAtticID = 0;
            RankingListID = 0;
            SchoolJob = 0;
            SchoolMember = 0;
        }
    }
}
