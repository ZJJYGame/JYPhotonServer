using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class PuppetUnitDTO
    {
        public virtual int RoleID { set; get; }
        public virtual Dictionary<int, PuppetUnitInfo> PuppetUnitInfoDict { set; get; }
        public virtual Dictionary<int, int> UnitIndesDict { set; get; }

      public  PuppetUnitDTO()
        {
            RoleID = -1;
            PuppetUnitInfoDict = new Dictionary<int, PuppetUnitInfo>();
            UnitIndesDict = new Dictionary<int, int>();
        }
    }

    public class PuppetUnitInfo
    {
        /// <summary>
        /// 气血，物攻，法攻，物防，法防，攻速，真元
        /// </summary>
        public virtual List<int> PuppetAttribute { set; get; }   
        /// <summary>
        /// 部件等级
        /// </summary>
        public virtual int UnitLevel { set; get; }
        /// <summary>
        ///部件部位
        /// </summary>
        public virtual int UnitPart { set; get; }
        public virtual int PuppetDurable { set; get; }
        /// <summary>
        /// 词缀类型
        /// </summary>
        public virtual int AffixType { set; get; }
        /// <summary>
        /// 词缀加成
        /// </summary>
        public virtual int AffixAddition{ set; get; }
        public virtual List<int> WeaponSkill { set; get; }

        public PuppetUnitInfo()
        {
            PuppetAttribute = new List<int>();
            UnitLevel = 0;
            PuppetDurable = 0;
            AffixType = 0;
            AffixAddition = 0;
            WeaponSkill = new List<int>();
        }
    }
}
