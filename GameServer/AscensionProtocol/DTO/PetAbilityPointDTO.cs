using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class PetAbilityPointDTO : DataTransferObject
    {
        public virtual int ID { get; set; }
        public virtual int SlnNow { get; set; }
        public virtual bool IsUnlockSlnThree { get; set; }
        public virtual Dictionary<int, PetAbilityDTO> AbilityPointSln { get; set; }
        public override void Clear()
        {
            ID = -1;
            SlnNow = 0;
            IsUnlockSlnThree = false;
            AbilityPointSln.Clear();
        }

    }
}
