using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class AlliancesDTO : DataTransferObject
    {
        public virtual int ID { get; set; }
        public virtual List<int> AllianceList { get; set; }
        public virtual int Index { get; set; }
        public virtual int AllIndex { get; set; }

        public override void Clear()
        {
            ID = -1;
            AllianceList = null;
        }
    }
}
