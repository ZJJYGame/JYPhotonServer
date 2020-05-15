using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    public class Monster
    {
        public virtual int ID { get; set; }
        public virtual byte MonsterLevel { get; set; }
        public virtual byte MonsterID { get; set; }
        public virtual int MonsterExp { get; set; }
        public virtual string MonsterName { get; set; }
        public virtual string MonsterSkillArray { get; set; }
    }
}
