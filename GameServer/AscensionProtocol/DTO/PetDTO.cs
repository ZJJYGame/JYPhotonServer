using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class PetDTO: DataTransferObject
    {
        public virtual int ID { get; set; }
        public virtual int PetLevel { get; set; }
        public virtual int PetID { get; set; }
        public virtual int PetExp { get; set; }
        public virtual string PetName { get; set; }
        public virtual List<int> PetSkillArray { get; set; }
        public virtual Dictionary<int,int> PetExtraSkill { get; set; }
        public virtual PetOperationalOrder PetOrderType { get; set; }
        public override void Clear()
        {
            ID = -1;
            PetLevel = 0;
            PetID = 0;
            PetExp = 0;
            PetName = null;
            PetSkillArray.Clear();
            PetExtraSkill.Clear();
        }
        public enum PetOperationalOrder
        {
            PetLevelUP = 1,//宠物升级
            PetResetAbilitySln = 2,//宠物加点重置
            PetResetStatus = 3,//宠物洗练
            PetEvolution = 4,//宠物进阶
            PetStudtSkill = 5,//宠物学习技能书
            PetCultivate =6,//宠物培养
            PetGetStatus = 7,//单个宠物所有数据
        }
    }
}
