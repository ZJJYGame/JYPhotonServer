using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionData
{
    public class RegionJsonDataManager:IBehaviour
    {
        public void OnInitialization() { }
        public void OnTermination() { }
        /// <summary>
        /// 获取对应区域的对应瓦块的Json数据
        /// </summary>
        /// <param name="region">区域码</param>
        /// <param name="tileID">瓦块的码</param>
        /// <returns>json的内容</returns>
        public static string GetRegionJsonContent(Region region,int tileID)
        {
            string json=string.Empty;
            switch (region)
            {
                case Region.Adventure:
                    json = ConcurrentSingleton<AdventureJsonCache>.Instance.GetRegionJsonData(tileID);
                    break;
            }
            return json;
        }
        public static string GetRegionJsonContent(short regionID, int tileID)
        {
            string json = string.Empty;
            switch ((Region)regionID)
            {
                case Region.Adventure:
                    json = ConcurrentSingleton<AdventureJsonCache>.Instance.GetRegionJsonData(tileID);
                    break;
            }
            return json;
        }
    }
}
