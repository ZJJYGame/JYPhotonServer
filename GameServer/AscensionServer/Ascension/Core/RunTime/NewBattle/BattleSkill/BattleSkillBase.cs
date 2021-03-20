using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class BattleSkillBase
    {
        //角色的属性
        CharacterBattleData CharacterBattleData;

        public int SkillID { get; protected set; }
        public int DamgeAddition { get; protected set; }
        public int CritProp { get; protected set; }
        public int CritDamage { get; protected set; }
        public int IgnoreDefensive { get; protected set; }

        //技能攻击目标数
        public int TargetNumber { get; protected set; }
        //技能攻击段数
        public int AttackSectionNumber { get; protected set; }

        /// <summary>
        /// 获取该技能的伤害
        /// </summary>
        /// <param name="index">第几段伤害</param>
        public BattleDamageData GetDamageData(int index)
        {
            BattleDamageData battleDamageData = new BattleDamageData();
            return battleDamageData;
        }
    }
}
