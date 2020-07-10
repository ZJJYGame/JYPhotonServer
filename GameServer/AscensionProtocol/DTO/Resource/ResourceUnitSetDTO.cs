using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    /// <summary>
    /// 资源单位集合
    /// </summary>
    public class ResourceUnitSetDTO : DataTransferObject
    {
        /// <summary>
        /// 全局ID
        /// </summary>
        public int GlobalID { get; set; }
        /// <summary>
        /// 表示资源单位的集合;
        /// 生成时分配的id作为key，生成的对象作为value
        /// </summary>
        public Dictionary<int, ResourceUnitDTO> ResUnitDict { get; set; }
        /// <summary>
        /// 资源单位的数量
        /// </summary>
        public int ResUnitCount
        {
            get
            {
                if (ResUnitDict == null)
                    return -1;
                return ResUnitDict.Count;
            }
        }
        public ResourceUnitSetDTO()
        {
            GlobalID = -1;
            ResUnitDict = new Dictionary<int, ResourceUnitDTO>();
        }
        public void SetData(int globalID)
        {
            this.GlobalID = globalID;
        }
        public void AddResUnit(ResourceUnitDTO resUnit)
        {
            if (!ResUnitDict.ContainsKey(resUnit.ID))
                ResUnitDict.Add(resUnit.ID, resUnit);
        }
        /// <summary>
        /// 占用资源
        /// </summary>
        /// <param name="resID">分配的资源ID</param>
        /// <returns>返回是否占用成功</returns>
        public bool OccupiedResUnit(int resID)
        {
            bool result = false;
            if (ResUnitDict.ContainsKey(resID))
            {
                if (ResUnitDict[resID].Occupied = true)
                    result = false;
                else
                {
                    ResUnitDict[resID].Occupied = true;
                    result = true;
                }
            }
            return result;
        }
        public override void Clear()
        {
            GlobalID = 0;
            ResUnitDict?.Clear();
        }
    }
}
