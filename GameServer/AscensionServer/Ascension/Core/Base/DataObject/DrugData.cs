using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 丹药的实体类
    /// </summary>
    [Serializable]
    [ConfigData]
    public class DrugData
    {
        public int Drug_ID { get; set; }
        public string Drug_Name { get; set; }
        public string Drug_Describe { get; set; }
        public string Drug_Icon { get; set; }
        public byte Can_Use_In_Lilian { get; set; }
        public int Drug_Use_Target { get; set; }
        public DrugType Drug_Type { get; set; }
        public int Drug_Value { get; set; }
        public int Drug_Value_Time { get; set; }
        public int Drug_CD { get; set; }
        public int Drug_Property { get; set; }
        public int Need_Level_ID { get; set; }
        public int Max_Level_ID { get; set; }
    }

    /// <summary>
    /// 1.恢复气血
    /// 2.恢复真元
    /// 3.增加宠物经验
    /// 4.自己修炼速度增加
    /// 5.提高突破概率
    /// 6.增加修炼数值
    /// 7.增加BUFF
    /// 8.复活
    /// 9.增加宠物资质
    /// </summary>
    public enum DrugType
    {
        RoleHP,
        RoleMP,
        PetExp,
        RoleCultivationSpeed,
        RoleBreakthrough,
        RoleCultivatinValue,
        RoleBuff,
        RoleResurgence,
        AddPetTalent
    }
}
