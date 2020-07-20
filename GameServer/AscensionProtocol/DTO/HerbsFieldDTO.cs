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
        //public virtual string  AllHerbs { get; set; }//list下标为灵田下标
        public virtual List<HerbFieldStatus> AllHerbs { get; set; }//list下标为灵田下标

        public override void Clear()
        {
            RoleID = -1;
            jobLevel = 0;
            AllHerbs=null;
        }
    }
    [Serializable]
    public class HerbFieldStatus
    {
        public virtual bool IsPick { get; set; }
        public virtual int HerbsID { get; set; }
        public virtual int HerbsYear { get; set; }//生长年份
        public virtual int FieldLevel { get; set; }
        public virtual string RemainingTime { get; set; }//记录灵草成熟剩余时间
    }

}
