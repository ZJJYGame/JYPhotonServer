using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class AllianceJobData
    {
        public int JobID { get; set; }
        public string AllianceJob { get; set; }
        public int JobStations { get; set; }
    }
}
