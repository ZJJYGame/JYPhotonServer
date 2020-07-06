using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class HerbsFieldDTO : DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual int jobLevel { get; set; }
         public virtual Dictionary<int, HerbsStatus> AllHerbs { get; set; }//以灵田下标为key值

        public override void Clear()
        {
            RoleID = -1;
            jobLevel = 0;
            AllHerbs.Clear();
        }
    }
    [Serializable]
    public class HerbsStatus
    {
        public virtual int HerbsID { get; set; }
        public virtual int HerbsCount { get; set; }
        public virtual string PlantTime { get; set; }
    }

}
