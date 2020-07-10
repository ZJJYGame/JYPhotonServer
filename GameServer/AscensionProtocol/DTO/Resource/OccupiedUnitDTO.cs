using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    /// <summary>
    /// 占用单位的数据
    /// 全局ID+分配的ID
    /// 这个DTO用于发起占用资源请求的传输数据类型，全局通用
    /// </summary>
    [Serializable]
    public class OccupiedUnitDTO:DataTransferObject
    {
        //TODO 迭代RegionID+TileID
        /// <summary>
        /// 区域码
        /// </summary>
        int RegionID { get; set; }
        /// <summary>
        /// 区域中的瓦片ID
        /// </summary>
        int TileID { get; set; }
        public int GlobalID { get; set; }
        public int ResID { get; set; }
        public void SetVale(int globalID,int resID)
        {
            this.ResID = resID;
            this.GlobalID = globalID;
        }
        public override void Clear()
        {
            GlobalID = 0;
            ResID = 0;
        }
    }
}
