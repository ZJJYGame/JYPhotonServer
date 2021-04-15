using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    /// <summary>
    /// 角色装备模块操作码
    /// </summary>
    public enum EquipmentOpCode
    {
        /// <summary>
        /// 装备武器
        /// </summary>
        EquipWeapon=1,
        /// <summary>
        /// 装备法宝
        /// </summary>
        EquipMagicWeapon=2,
        /// <summary>
        /// 卸下武器
        /// </summary>
        RemoveWeapon=3,
        /// <summary>
        /// 卸下法宝
        /// </summary>
        RemoveMagicWeapon=4,
        /// <summary>
        /// 获取角色装备数据
        /// </summary>
        GetEquipStatus=5,
    }
}
