//***********************************************************
// 描述：
// 作者：Don  
// 创建时间：2020-10-29 20:10:32
// 版 本：1.0
//***********************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    /// <summary>
    /// 技能类型：
    /// 移动，譬如加速；
    /// </summary>
    //public readonly static byte _SkillType_Movement = 32;
    /// <summary>
    /// 技能类型；
    /// 数值影响，譬如增加隐匿值，增加神魂值；
    /// </summary>
    //public readonly static byte _SkillType_StatsAffect = 33;
    /// <summary>
    /// 技能类型：
    /// 位移、闪现一类；
    /// </summary>
    //public readonly static byte _SkillType_Teleport = 34;
    /// <summary>
    /// 技能类型：
    /// 隐匿、对自己使用；
    /// </summary>
    //public readonly static byte _SkillType_Cloak = 35;
    /// <summary>
    /// 技能类型：
    /// 破隐；对其他玩家使用；
    /// </summary>
    //public readonly static byte _SkillType_Unearthed = 36;
    /// <summary>
    /// 技能类型定义类；
    /// 原本会使用枚举，但是枚举可扩展性不高，因此选用只读数据；
    /// 若扩展，枚举不可继承，但define类可进行继承，且包含所有父类的只读数据；
    /// </summary>
    public class SkillDefine
    {
        #region [0~31],Count:32; 可修改类型；
        /// <summary>
        /// 可影响类型：既可以增加也可以消耗；
        /// 红条     
        /// </summary>
        public const byte Affectable_Qixue = 2;
        /// <summary>
        /// 可影响类型：既可以增加也可以消耗；
        /// 蓝条；
        /// </summary>
        public const byte Affectable_Zhenyuan = 3;
        /// <summary>
        /// 可影响类型：既可以增加也可以消耗；
        ///  黄条
        /// </summary>
        public const byte Affectable_Shenhun = 4;
        /// <summary>
        /// 可影响类型：既可以增加也可以消耗；
        ///  紫条
        /// </summary>
        public const byte Affectable_Jingxue = 5;
        /// <summary>
        /// 可影响类型：既可以增加也可以消耗；
        /// 隐匿值；
        /// </summary>
        public const byte Affectable_Cloak = 6;
        /// <summary>
        /// 可影响类型：既可以增加也可以消耗；
        /// 移动速度；
        /// </summary>
        public const byte Affectable_Speed = 7;
        /// <summary>
        /// 可影响类型：特殊类型；
        /// 影响位置；
        /// </summary>
        public const byte Affectable_Transform = 8;
        #endregion

        #region [32~47],Count:16;目标作用类型
        /// <summary>
        /// 影响的目标类型；
        /// 对自己产生作用；
        /// </summary>
        public const byte AffectTarget_Self = 32;
        /// <summary>
        /// 影响的目标类型；
        /// 对他人产生作用；
        /// </summary>
        public const byte AffectTarget_Others = 33;
        /// <summary>
        /// 影响的目标类型；
        /// 既可以对他人，也可以对自己作用；
        /// </summary>
        public const byte AffectTarget_Both = 34;
        #endregion

        #region[48~63],Count:16;技能类型
        /// <summary>
        ///  技能类型：
        ///  主动类型技能；
        /// </summary>
        public const byte TSKILL_Active = 48;
        /// <summary>
        ///  技能类型：
        /// 被动技能类型；
        /// </summary>
        public const byte TSKILL_Passive = 49;
        /// <summary>
        ///  技能类型：
        ///  按钮开关类型，按下触发，再按下停止；
        /// </summary>
        public const byte TSKILL_ActiveToggle = 50;
        /// <summary>
        ///  技能类型：
        /// 被动技能开关类型；
        /// </summary>
        public const byte TSKILL_PassiveToggle = 51;
        /// <summary>
        ///  技能类型：
        ///  Basically any skill that continues to cast as long as you hold down the button is a channeled skill
        /// 持续释放类型，要求需要按住按钮进行持续释放；
        /// </summary>
        public const byte TSKILL_Channeled = 52;
        #endregion
    }
    /// <summary>
    /// 非公共常量命名规则师范类；
    /// </summary>
    class SkillDefineExample
    {
        //Pascal规范用于Public常量； 
        //Camel规范用于Internal常量;
        //只有1或2个字符的缩写需全部字符大写。
        //缩写需字符大写。

        //Const：
        /// <summary>
        ///表示SkillType,T为Type缩写；
        ///若命名为缩写，则全大写；
        /// </summary>
        public const byte TSKILL = 30;
        public const byte TSKILL_Active = 31;
        public const byte SkillType_A = 32;
        internal const byte skillType_B = 33;
        private const byte skillType_C = 34;
        public const float PI = 3.1415926f;

        //Readonly
        /// <summary>
        /// public类型遵循Pascal；
        /// 若命名为缩写，则全大写；
        /// </summary>
        public static readonly byte _TSKILL = 35;
        public static readonly byte _TSKILL_Active = 36;
        public static readonly byte _SkillType_D = 37;
        internal static readonly byte _skillType_E = 38;
        private static readonly byte _skillType_F = 39;
        public static readonly float _PI = 3.1415926f;
    }
}
