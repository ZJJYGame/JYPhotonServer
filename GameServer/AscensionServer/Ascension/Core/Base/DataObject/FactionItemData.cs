using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    //宗门藏宝阁兑换
    [Serializable]
    [ConfigData]
    public class FactionItemData
    {
        public int FactionItemId { get; set; }
        public List<FactionItem> FactionItem { get; set; }

    }

    [Serializable]
    [ConfigData]
    public class FactionItem
    {
        public int FactionItem_ID { get; set; }
        public string FactionItem_Name { get; set; }
        public int School_ID { get; set; }
        public string FactionItem_ImgName { get; set; }
        public FactionItemType FactionItem_Type { get; set; }
        public int FactionItem_LevelID { get; set; }
        public int OnlyExchangeOnce { get; set; }
        public int FactionItem_TodayNum { get; set; }
        public int FactionItem_Number { get; set; }
        public bool Is_Can_Exchange { get; set; }
        public string FactionItem_LevelName { get; set; }
        public Faction_LevelType FactionItem_Level { get; set; }
        public int FactionItem_TodayIndex = 0;
    }

    /// <summary>
    /// 门派弟子
    /// 外门弟子  931
    /// 内门弟子   932
    ///// 执法弟子//EnforceDisciples,
    /// 真传弟子  933
    /// 长老          934
    /// 太上长老   935
    ///// 护法//UpholdTheConstitution,
    ///// 供奉//Consecrate
    /// </summary>
    public enum Faction_LevelType:byte
    {
        OuterDisciples,
        InnerDisciples,
        CoreDisciples,
        SectElders,
        OnTheElders
    }

    /// <summary>
    /// 藏宝阁主库
    /// 丹药区
    /// 图纸区
    /// 灵兽区
    /// 其他区
    /// </summary>
    public enum FactionItemType:byte
    {
        Drug,
        Material,
        Formula
    }
}
