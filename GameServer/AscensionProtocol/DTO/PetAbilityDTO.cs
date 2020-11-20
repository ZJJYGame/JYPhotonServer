using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class PetAbilityDTO : DataTransferObject
    {
        public virtual int Strength { get; set; }
        public virtual int Power { get; set; }
        public virtual int Stamina { get; set; }
        public virtual int Corporeity { get; set; }
        public virtual int Soul { get; set; }
        public virtual int Agility { get; set; }
        public virtual int AllAptitudePoint { get; set; }

        public PetAbilityDTO()
        {
            Strength = 0;
            Power = 0;
            Stamina = 0;
            Corporeity = 0;
            Soul = 0;
            Agility = 0;
            AllAptitudePoint = 0;
        }
        public override void Clear()
        {
            Strength = 0;
            Power = 0;
            Stamina = 0;
            Corporeity = 0;
            Soul = 0;
            Agility = 0;
            AllAptitudePoint = 0;
        }
    }
}
