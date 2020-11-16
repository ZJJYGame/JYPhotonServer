﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// Postfix(后缀);
    /// Perfix(前缀)； 
    /// </summary>
    public class RedisKeyDefine
    {
        /// <summary>
        /// 类历练技能槽按钮持久化key前缀；
        /// </summary>
        public static readonly string _SkillLayoutPerfix = "SkillLayout";
        /// <summary>
        /// 功法的key前缀
        /// </summary>
        public static readonly string _CultivatiionMethodPerfix = "Gongfa";
        /// <summary>
        /// 秘术的Key前缀
        /// </summary>
        public static readonly string _MiShuPerfix = "MiShu";
        /// <summary>
        /// 角色秘术的Key前缀
        /// </summary>
        public static readonly string _RoleMiShuPerfix = "RoleMiShu";
        /// <summary>
        /// 宠物的Key前缀
        /// </summary>
        public static readonly string _PetPerfix = "Pet";
        /// <summary>
        /// 宠物属性的Key前缀
        /// </summary>
        public static readonly string _PetStatusPerfix = "PetStatus";
        /// <summary>
        /// 宠物资质的Key前缀
        /// </summary>
        public static readonly string _PetAptitudePerfix = "PetAptitude";
        /// <summary>
        /// 傀儡的Key前缀
        /// </summary>
        public static readonly string _PuppetPerfix = "Puppet";
        /// <summary>
        /// 炼丹的Key前缀
        /// </summary>
        public static readonly string _AlchemyPerfix = "Alchemy";
        /// <summary>
        ///锻造的Key前缀
        /// </summary>
        public static readonly string _ForgePerfix = "Forge";
        /// <summary>
        ///阵法的Key前缀
        /// </summary>
        public static readonly string _ArrayPerfix = "Array";
        /// <summary>
        ///制符的Key前缀
        /// </summary>
        public static readonly string _RunesPerfix = "Runes";
        /// <summary>
        ///灵田的Key前缀
        /// </summary>
        public static readonly string _FieldPerfix = "Field";
        /// <summary>
        ///商店的Key前缀
        /// </summary>
        public static readonly string _ShopPerfix = "Shop";
        /// <summary>
        ///杂货铺的Key前缀
        /// </summary>
        public static readonly string _VareityShopPerfix = "VareityShop";
        /// <summary>
        ///仙盟列表的Key前缀
        /// </summary>
        public static readonly string _AllianceListPerfix = "AllianceList";
        /// <summary>
        ///仙盟的Key前缀
        /// </summary>
        public static readonly string _AlliancePerfix = "Alliance";
        /// <summary>
        ///个人仙盟数据的Key前缀
        /// </summary>
        public static readonly string _RoleAlliancePerfix = "RoleAnlliance";
        /// <summary>
        ///仙盟建设的Key前缀
        /// </summary>
        public static readonly string _AllianceConstructionPerfix = "AllianceConstruction";
        /// <summary>
        ///仙盟内广播的消息内容的Key前缀
        /// </summary>
        public static readonly string _DailyMessagePerfix = "DailyMessage";
        /// <summary>
        /// 阵法记录删除回调的key前缀
        /// </summary>
        public static readonly string _DeldteTacticalPerfix = "Tactical";
        /// <summary>
        /// 角色属性值的key前缀
        /// </summary>
        public static readonly string _RoleStatsuPerfix = "RoleStatus";
        /// <summary>
        /// 角色功法的key前缀
        /// </summary>
        public static readonly string _RoleGongfaPerfix = "RoleGongfa";
        /// <summary>
        /// 功法的key前缀
        /// </summary>
        public static readonly string _GongfaPerfix = "Gongfa";
    }
}
