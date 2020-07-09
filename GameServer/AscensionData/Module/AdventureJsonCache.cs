using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionData
{
   public class AdventureJsonCache:IBehaviour
    {
        string folderPath = "Json/Module/Adventure/Region";
        Dictionary<int, string> regionDataDict = new Dictionary<int, string>();
        public void OnInitialization() { }
        public void OnTermination() { }
        public string GetRegionJsonData(int index)
        {
            string data;
            regionDataDict.TryGetValue(index, out data);
            if (string.IsNullOrEmpty(data))
            {
                data = Utility.IO.ReadTextFileContent(folderPath, "Region_" + index+".txt");
                regionDataDict.Add(index, data);
            }
            return data;
        }
    }
}
