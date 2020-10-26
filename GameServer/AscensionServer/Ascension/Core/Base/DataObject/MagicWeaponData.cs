using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 法宝的实体类
    /// </summary>
    [Serializable]
    [ConfigData]
    public class MagicWeaponData
    {
        public int Magic_ID { get; set; }
        public string Magic_Name { get; set; }
        public string Magic_Describe { get; set; }
        public string Magic_Icon { get; set; }
        public int Magic_Quality { get; set; }
        public int Need_Level_ID { get; set; }
        public int Magic_Attack { get; set; }
        public int Value_Flow { get; set; }
        public int Magic_Power { get; set; }
        public int Magic_Durable { get; set; }
        public int Magic_Skill { get; set; }
        public string Magic_Effect { get; set; }
        public EquipType Magic_Type { get; set; }
    }
    
    /// <summary>
    /// 装备的分类
    /// 1.武器
    /// 2.外衣
    /// 3.内甲
    /// 4、鞋子
    /// 5.装备栏
    /// 6.装备栏
    /// 7.储物袋
    /// 8.灵兽袋
    /// 9.法宝
    /// 10.默认值
    /// </summary>
    public enum EquipType
    {
        PlayerEquip,
        PlayerOuterWear,
        PlayerUnderWear,
        PlayerShoe,
        Player1,
        Player2,
        PlayerStorageBag,
        PlayerSpiritBeastBag,
        MagicWeapon,
        Default
    }
}
