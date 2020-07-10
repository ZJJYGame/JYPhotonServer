using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    public class ResourceSetDTO : DataTransferObject
    {
        /// <summary>
        /// 全局ID
        /// </summary>
        public int GlobalID { get; set; }
        /// <summary>
        /// 资源单位集合
        /// </summary>
        public HashSet<ResourceUnitDTO> ResUnitSet { get; set; }
        /// <summary>
        /// 资源单位的数量
        /// </summary>
        public int ResUnitCount
        {
            get
            {
                if (ResUnitSet == null)
                    return -1;
                return ResUnitSet.Count;
            }
        }
        public ResourceSetDTO()
        {
            GlobalID = -1;
            ResUnitSet = new HashSet<ResourceUnitDTO>();
        }
        public void SetData(int globalID, HashSet<ResourceUnitDTO> resUnitSet)
        {
            this.GlobalID = globalID;
            this.ResUnitSet = resUnitSet;
        }
        public override void Clear()
        {
            GlobalID = 0;
            ResUnitSet?.Clear();
        }
    }
}
