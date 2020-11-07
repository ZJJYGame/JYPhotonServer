using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol;
using System.Collections.Concurrent;

namespace AscensionServer 
{
    /// <summary>
    /// 技能实体对象；
    /// 技能的CD、持续时间等使用此作为容器进行运算；
    /// 此实体持有技能数据，
    /// </summary>
    public class SkillEntity : Entity, IReference,IRefreshable
    {
        public IDataContract DataContract { get; set; }
        ConcurrentDictionary<int, Variable<C2SSkillInput>> skillDict = 
            new ConcurrentDictionary<int, Variable<C2SSkillInput>>();
        protected SkillEntity(){}
        protected SkillEntity(int id) : base(id){}
        public virtual void OnCastSkill(IDataContract data)
        {

        }
        public virtual void OnInit(IDataContract data)
        {

        }
        public virtual void Clear(){}
        /// <summary>
        /// 空虚函数;
        /// </summary>
        public virtual void OnRefresh() { }
    }
}
