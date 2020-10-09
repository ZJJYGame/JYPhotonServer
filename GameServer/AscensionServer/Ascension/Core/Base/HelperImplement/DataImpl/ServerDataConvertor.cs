using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using System.IO;
using log4net.Config;
using AscensionServer.Threads;
using System.Reflection;
using ExitGames.Concurrency.Fibers;
using Cosmos;
using AscensionServer.Model;
using AscensionData;
using System.Threading;

namespace AscensionServer
{
    /// <summary>
    /// 服务器数据转换
    /// </summary>
    [TargetHelper]
    public class ServerDataConvertor : IDataConvertor
    {
        public void ConvertData()
        {
            //Utility.Debug.LogInfo("<DataManager> 测试 ConvertData");
            try
            {
               GameManager.CustomeModule<DataManager>().TryGetValue(typeof(GongFaBook).Name, out var gongfaBookSet);
                var gfbDict = TransObject<List<GongFaBook>>(gongfaBookSet).ToDictionary(key => key.Book_ID, value => value);
               

                GameManager.CustomeModule<DataManager>().TryGetValue(typeof(GongFa).Name, out var gongfaSet);
                var gfDict = TransObject<List<GongFa>>(gongfaSet).ToDictionary(key => key.Gongfa_ID, value => value);

                GameManager.CustomeModule<DataManager>().TryGetValue(typeof(MonsterDatas).Name, out var monsterDatasSet);
                var monsterDict = TransObject<List<MonsterDatas>>(monsterDatasSet).ToDictionary(key => key.Monster_ID, value => value);

                GameManager.CustomeModule<DataManager>().TryAdd(gfbDict);
                GameManager.CustomeModule<DataManager>().TryAdd(gfDict);
                GameManager.CustomeModule<DataManager>().TryAdd(monsterDict);
                GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, MonsterDatas>>(out var set);
                //Utility.Debug.LogInfo("<DataManager> 测试 TryGetValue " + set[22001].Monster_describe);
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
            Utility.Debug.LogInfo("<DataManager> 测试 ConvertData Step0211111111111");
        }

        /// <summary>
        /// 转换为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public T TransObject <T>(string data)
        {
            return Utility.Json.ToObject<T>(data);
        } 


    }
}