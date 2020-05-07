using AscensionServer;
//using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using ExitGames.Logging;
//using ExitGames.Logging.Log4Net;
//using LogManager = ExitGames.Logging.LogManager;
using AscensionServer.Model;
using AscensionServer.Manager;
//using NHibernate;
//using NHibernate.Cfg;


namespace ProgameTest
{
    class Program
    {
        //public static readonly ILogger log = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            Singleton<UserRoleManager>.Instance.Add(new UserRole() { UUID = "newuuid", Role_Id_Array = "78,76" });
            Console.ReadKey();
        }
    }
}
