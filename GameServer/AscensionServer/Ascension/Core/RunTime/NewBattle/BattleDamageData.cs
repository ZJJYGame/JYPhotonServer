using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 战斗伤害数据
    /// </summary>
    public class BattleDamageData
    {
        //伤害目标ID
        public int TargetID;
        //攻击段数
        public int attackSection;
        //行为类型
        public BattleSkillActionType battleSkillActionType;
        //伤害类型
        public BattleSkillDamageType damageType;
        //基础伤害是否暴击,是=>暴击，否=>不暴击
        public bool isCrit;
        /// <summary>
        /// 基础伤害的目标属性（血量，真元，神魂）
        /// </summary>
        public BattleSkillDamageTargetProperty baseDamageTargetProperty;
        //伤害数字
        public int damageNum;
        /// <summary>
        /// 额外伤害的目标属性（血量，真元，神魂）
        /// </summary>
        public BattleSkillDamageTargetProperty extraDamageTargetProperty;
        //额外伤害
        public int extraDamageNum;
    }
}
