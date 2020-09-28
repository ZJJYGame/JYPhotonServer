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
using AscensionRegion;
using System.Threading;

namespace AscensionServer
{
    /// <summary>
    /// 加载Json
    /// </summary>
    [CustomeModule]
    public class ServerDataManager : Module<ServerDataManager>
    {
        public T InjectJson<T>(object data) { return Utility.Json.ToObject<T>(data.ToString()); }

        /// <summary>
        /// 加载json 路径
        /// </summary>
        /// <param name="loadToJsonPath"></param>
        /// <returns></returns>
        public string LoadPath(string loadToJsonPath) { return Utility.IO.ReadTextFileContent(LoadToJsonPathManger.JsonData, loadToJsonPath); }



        public override void OnInitialization()
        {
            var gongFaBooks = InjectJson<List<GongFaBook>>(LoadPath(LoadToJsonPathManger.GongFaBook));
            ResourcesListManger.Instance.GongFaBooksDict = gongFaBooks.ToDictionary(key => key.Book_ID, value => value);
            var gongFa = InjectJson<List<GongFa>>(LoadPath(LoadToJsonPathManger.GongFa));
            ResourcesListManger.Instance.GongFaDict = gongFa.ToDictionary(key => key.Gongfa_ID, value => value);

            Utility.Debug.LogError("<Json列表>" + ResourcesListManger.Instance.GongFaBooksDict[13001].Book_Describe);
            Utility.Debug.LogError("<Json列表>" + ResourcesListManger.Instance.GongFaDict[18001].Gongfa_Describe);
        }

    }


    [CustomeModule]
    /// <summary>
    /// 战斗用到的 Json 列表管理
    /// </summary>
    public class ResourcesListManger : Module<ResourcesListManger>
    {
        /// 功法书Dict
        /// </summary>
        public  Dictionary<int, GongFaBook> GongFaBooksDict;
        /// <summary>
        /// 功法Dict
        /// </summary>
        public Dictionary<int, GongFa> GongFaDict;



    }


    /// <summary>
    /// Json表格 路径管理
    /// </summary>
    public class LoadToJsonPathManger
    {
        public const string JsonData = "JsonData";
        public const string GongFaBook = "GongFaBook.txt";
        public const string GongFa = "GongFa.txt";




    }



}
