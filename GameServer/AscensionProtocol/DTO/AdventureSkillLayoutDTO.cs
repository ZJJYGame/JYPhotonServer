using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionProtocol;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class AdventureSkillLayoutDTO : DataTransferObject
    {
        public Dictionary<string, int> SkillLayoutDict { get; set; }
        public Dictionary<string, int> PropLayoutDict { get; set; }
        public override void Clear()
        {
            SkillLayoutDict.Clear();
            PropLayoutDict.Clear();
        }
    }
}
