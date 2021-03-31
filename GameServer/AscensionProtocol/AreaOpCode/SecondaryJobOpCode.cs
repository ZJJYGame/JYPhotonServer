using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
   public enum SecondaryJobOpCode : byte
    {
        /// <summary>
        /// 获得炼丹数据
        /// </summary>
        GetAlchemyStatus=1,
        /// <summary>
        /// 学习炼丹配方
        /// </summary>
        UpdateAlchemy=2,
        /// <summary>
        /// 开始炼丹
        /// </summary>
        CompoundAlchemy=3,
        /// <summary>
        /// 获得傀儡数据
        /// </summary>
        GetPuppetStatus =4,
        /// <summary>
        /// 学习傀儡配方
        /// </summary>
        UpdatePuppet=5,
        /// <summary>
        /// 炼制傀儡
        /// </summary>
        CompoundPuppet = 6,
        /// <summary>
        /// 获得锻造数据
        /// </summary>
        GetWeaponStatus=7,
        /// <summary>
        /// 学习锻造配方
        /// </summary>
        UpdateWeapon=8,
        /// <summary>
        /// 开始锻造
        /// </summary>
        CompoundWeapon = 9,
    }
}
