using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class AlchemyDTO: DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual Dictionary<string, AlchemyStatus> AlchemyStatusDict { get; set; }

        public override void Clear()
        {
            RoleID = -1;
            AlchemyStatusDict.Clear();
        }
    }

    public class AlchemyStatus
    {
        public virtual int Level { get; set; }
        public virtual string Recipe { get; set; }
    }
}
