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
        public void LoadData()
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
                //Utility.Debug.LogInfo($"\n{pureStr}\n{str}\n");
#endif
            }
            GameManager.CustomeModule<DataManager>().SetDataDict(jsonDict);
            ParseData();
        }
        void  ParseData()
        {
            objectDict.Clear();
            var datas = Utility.Assembly.GetInstancesByAttribute<ConfigDataAttribute,Data>( true);
            for (int i = 0; i < datas.Length; i++)
            {
                string json;
                //var fullName = Utility.Text.Append(datas[i].GetType().Name, ".txt");
                var fullName = datas[i].GetType().Name;
                if (jsonDict.TryGetValue(fullName, out json))
                {
                    Utility.Debug.LogWarning($"find json{fullName}");
                    try
                    {
                        var obj = Utility.Json.ToObject(json, datas[i].GetType());
                        if (obj != null)
                        {
                            objectDict.TryAdd(datas[i].GetType(), obj);
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.Debug.LogError($"IDataProvider ToObject fail . Type :{datas[i].GetType().Name} ;{e}");
                    }
                }
            }
            GameManager.CustomeModule<DataManager>().SetDataDict(objectDict);
        }
    }
}
