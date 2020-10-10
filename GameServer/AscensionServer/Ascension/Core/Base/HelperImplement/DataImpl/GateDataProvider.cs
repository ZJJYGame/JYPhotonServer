using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Cosmos;
using Pipelines.Sockets.Unofficial.Arenas;

namespace AscensionServer
{
    [ImplementProvider]
    public class GateDataProvider : IDataProvider
    {
        string folderPath = Environment.CurrentDirectory + "/JsonData";
        Dictionary<string, string> jsonDict = new Dictionary<string, string>();
        Dictionary<Type, object> objectDict = new Dictionary<Type, object>();
        public object LoadData()
        {
            jsonDict.Clear();
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            char[] ch = ".txt".ToCharArray();
            foreach (var f in dir.GetFiles())
            {
                var str = Utility.IO.ReadTextFileContent(folderPath, f.Name);
                var pureStr = f.Name.TrimEnd(ch);
                jsonDict.Add(pureStr, str);
#if DEBUG
                Utility.Debug.LogInfo($"\n{pureStr}\n{str}\n");
#endif
            }
            return jsonDict;
        }
        public object ParseData()
        {
            objectDict.Clear();
            var datSet = Utility.Assembly.GetInstancesByAttribute<ConfigDataAttribute>(typeof(Data), true);
            for (int i = 0; i < datSet.Length; i++)
            {
                string json;
                var fullName = Utility.Text.Append(datSet[i].GetType().Name, ".txt");
                if (jsonDict.TryGetValue(fullName, out json))
                {
                    try
                    {
                        var obj = Utility.Json.ToObject(json, datSet[i].GetType());
                        if (obj != null)
                        {
                            objectDict.TryAdd(datSet[i].GetType(), obj);
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.Debug.LogError($"IDataProvider ToObject fail . Type :{datSet[i].GetType().Name} ;{e}");
                    }
                }
            }
            return objectDict;
        }
    }
}
