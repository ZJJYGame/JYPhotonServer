using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class FlyMagicToolData
    {
        public int FlyMagicToolID { get; set; }
        public int AddRoleHp { get; set; }
        public int AddPhysicAttack { get; set; }
        public int AddMagicAttack { get; set; }
        public int AddMoveSpeed { get; set; }
        public int FixedSpeed { get; set; }
        public int NumberOfPeoples { get; set; }
    }
}
