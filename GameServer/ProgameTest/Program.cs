using AscensionServer;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using LogManager = ExitGames.Logging.LogManager;

namespace ProgameTest
{
    class Program
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {


            //***匹配账户*********************
            IUserManager userManager = new UserManager();
            Console.WriteLine(userManager.VerifyUser("index2", "qqq1"));//true
            Console.WriteLine(userManager.VerifyUser("5l", "eee"));//false
            //*************************/
            //log.Info(userManager.VerifyUser("Sure", "157"));
            //log.Info(userManager.VerifyUser("da", "25"));
            Console.ReadKey();
        }
    }
}
