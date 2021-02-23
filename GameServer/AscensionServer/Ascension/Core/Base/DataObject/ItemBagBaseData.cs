using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class ItemBagBaseData
    {
        /// <summary>
        ///物品的唯一id
        /// </summary>
        public int ItemID { get; set; }
        /// <summary>
        ///  全局id
        /// </summary>
        public int Id { get; set; }
        public string ItemName { get; set; }
        public ItemType Itemtype { get; set; }
        public ItemQuality Itemquality { get; set; }
        public int ItemHeld { get; set; }
        public string ItemDescription { get; set; }
        public int ItemSellPrice { get; set; }
        public int ItemBuyPrice { get; set; }
        public string ItemImg { get; set; }
        public string ItemContent { get; set; }
        public string ItemTime { get; set; }
        public string ItemAdorn { get; set; }
        public EquipType ItemSign { get; set; }
        public int ItemMax { get; set; }
        public int ItemShop { get; set; }
        public int ItemSale { get; set; }
    }

    /// <summary>
    /// 物品种类
    /// 1.全部
    /// 2.法宝
    /// 3.消耗品
    /// 4.材料
    /// 5.最近获取
    /// </summary>
    public enum ItemType
    {
        Default,
        Weapon,
        Consumable,
        Material,
        LastAble,
    }

    public enum UseItemType
    {
        Drug,
        Rune,
        Tactic
    }

    /// <summary>
    /// 物品品质
    /// 1.普通的
    /// 2.稀有的
    /// 3.非常稀有的
    /// 4.史诗的
    /// 5.传说的
    /// </summary>
    public enum ItemQuality
    {
        Green,
        Blue,
        Purple,
        Orange,
        Red,
    }
}


