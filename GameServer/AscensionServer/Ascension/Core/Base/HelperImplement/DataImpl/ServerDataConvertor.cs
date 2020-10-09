﻿using System;
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
            List<GongFaBook> gongfaBookList;
            GameManager.CustomeModule<DataManager>().TryGetObjectValue(typeof(GongFaBook).Name, out gongfaBookList);
            var gongfaBookDict = gongfaBookList.ToDictionary(key => key.Book_ID, value => value);
            List<GongFa> gongfaList;
            GameManager.CustomeModule<DataManager>().TryGetObjectValue(typeof(GongFa).Name, out gongfaList);
            var gongfaDict = gongfaList.ToDictionary(key => key.Gongfa_ID, value => value);
            List<MonsterDatas> monsterDatas;
            GameManager.CustomeModule<DataManager>().TryGetObjectValue(typeof(MonsterDatas).Name, out monsterDatas);
            var monsterDatasDict = monsterDatas.ToDictionary(key => key.Monster_ID, value => value);
            GameManager.CustomeModule<DataManager>().TryAdd(gongfaDict);
            GameManager.CustomeModule<DataManager>().TryAdd(gongfaBookDict);
            GameManager.CustomeModule<DataManager>().TryAdd(monsterDatasDict);
            Utility.Debug.LogInfo("<DataManager>" + monsterDatasDict.Count);
        }
    }
}