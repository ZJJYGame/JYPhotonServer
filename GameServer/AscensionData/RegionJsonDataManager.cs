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
        /// 获取区域的json数据
        /// </summary>
        /// <param name="region">模块</param>
        /// <param name="regionIndex">区域的序号码</param>
        /// <returns>json的内容</returns>
        public static string GetRegionJsonContent(Region region,int regionIndex)
        {
            string json=string.Empty;
            switch (region)
            {
                case Region.Adventure:
                    json = Singleton<AdventureJsonCache>.Instance.GetRegionJsonData(regionIndex);
                    break;
            }
            return json;
        }
    }
}
