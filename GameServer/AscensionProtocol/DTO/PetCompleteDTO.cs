using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class PetCompleteDTO : DataTransferObject
    {
        public virtual PetDTO PetDTO { get; set; }
        public virtual PetStatusDTO PetStatusDTO { get; set; }
        public virtual PetAbilityPointDTO PetAbilityPointDTO { get; set; }
        public virtual PetAptitudeDTO PetaPtitudeDTO { get; set; }
        public virtual PetOperationalOrder PetOrderType { get; set; }
        public override void Clear()
        {
            PetDTO.Clear();
            PetStatusDTO.Clear();
            PetAbilityPointDTO.Clear();
            PetaPtitudeDTO.Clear();
            PetOrderType = PetOperationalOrder.None;
        }
        public enum PetOperationalOrder
        {
            None=0,
            PetLevelUP = 1,//宠物升级
            PetResetAbilitySln = 2,//宠物加点重置
            PetResetStatus = 3,//宠物洗练
            PetEvolution = 4,//宠物进阶
            PetStudtSkill = 5,//宠物学习技能书
            PetCultivate = 6,//宠物培养
            PetGetStatus = 7,//单个宠物所有数据
        }
    }
}
