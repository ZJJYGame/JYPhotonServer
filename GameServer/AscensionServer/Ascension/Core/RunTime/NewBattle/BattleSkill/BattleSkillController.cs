using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 战斗角色技能控制器,每个角色实体持有，用来管理角色技能
    /// </summary>
    public class BattleSkillController
    {
        //控制器拥有者
        BattleCharacterEntity owner;
        //角色技能对象缓存,key=>技能id,value=>技能对象
        Dictionary<int, BattleSkillBase> skillDict;
        //角色拥有的技能id集合
        HashSet<int> roleHasSkillHash;

        

        public BattleSkillController(BattleCharacterEntity owner)
        {
            this.owner = owner;
            skillDict = new Dictionary<int, BattleSkillBase>();
        }
    }
}
