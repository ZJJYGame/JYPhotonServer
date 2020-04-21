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
namespace ProgameTest
{
    class Program
    {
        //public static readonly ILogger log = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {


            //***匹配账户*********************
            IUserManager userManager = new UserManager();
            Console.WriteLine(userManager.VerifyUser("index2", "qqq1"));//true
            //Console.WriteLine(userManager.GetByUsername("index2"));
            //Console.WriteLine(userManager.GetById(1).Username);

            User user = userManager.GetByUsername("index2");
            //userManager.Remove(new User() { Username = "index3", Password = "qq3", Age = "13" });
            Console.WriteLine( "表里的名字"+ user.Password);

            //Console.WriteLine(userManager.VerifyUser("5l", "eee"));//false
           // userManager.Add(new User() { Username = "donTest", Password = "donPwd" });
            userManager.Add(new User() { Password = "donPwd", Username = "dontext" ,UUID = "2"});
            //*************************/
            //log.Info(userManager.VerifyUser("Sure", "157"));
            //log.Info(userManager.VerifyUser("da", "25"));
            Console.ReadKey();
        }
    }
}
