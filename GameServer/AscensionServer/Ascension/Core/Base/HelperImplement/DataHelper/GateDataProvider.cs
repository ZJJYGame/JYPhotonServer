using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Cosmos;
using Pipelines.Sockets.Unofficial.Arenas;

namespace AscensionServer
{
    [TargetHelper]
    public class GateDataProvider : IDataProvider
    {
        string folderPath = Environment.CurrentDirectory + "/JsonData";
        Dictionary<string, string> jsonDict = new Dictionary<string, string>();
        Dictionary<Type, object> objectDict = new Dictionary<Type, object>();
        public object LoadData()
        {
            jsonDict.Clear();
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            foreach (var f in dir.GetFiles())
            {
                var str = Utility.IO.ReadTextFileContent(folderPath, f.Name);
                jsonDict.Add(f.Name, str);
#if DEBUG
                Utility.Debug.LogInfo($"\n{f.Name}\n{str}\n");
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
