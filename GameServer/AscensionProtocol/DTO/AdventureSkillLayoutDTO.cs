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
        public Dictionary<string, int> LayoutDict { get; set; }

        public override void Clear()
        {
            LayoutDict = null;
        }
    }
}
