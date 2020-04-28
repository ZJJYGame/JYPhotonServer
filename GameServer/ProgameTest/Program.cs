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
            //ISessionFactory sessionFactory = new Configuration().Configure().BuildSessionFactory();
            //using (ISession session = sessionFactory.OpenSession())
            //{
            //    ArticleModel art = session.Get<ArticleModel>(1);
            //    Console.WriteLine(art.Id);
            //    Console.WriteLine(art.Title);
            //    Console.WriteLine(art.Content.Content);
            //}

            //IArticleManager articleManager = new ArticleManager();
            //ArticleModel article =  articleManager.GetArticleContent(new ArticleModel() { Id = 2});
            //Console.WriteLine(article.Id);
            //Console.WriteLine(article.Title);
            //Console.WriteLine(article.Content.Id);
            //string str = article.Content.Content;
            //Console.WriteLine(str);
            //Console.WriteLine();
            //***匹配账户*********************
            IUserManager userManager = new UserManager();
            Console.WriteLine(userManager.VerifyUser("01", "01"));//true
            //Console.WriteLine(userManager.GetByUsername("index2"));
            //Console.WriteLine(userManager.GetById(1).Username);

            //User user = userManager.GetByUsername("index2");
            //userManager.Remove(new User() { Username = "index3", Password = "qq3", Age = "13" });
            //Console.WriteLine( "表里的名字"+ user.Password);

            //Console.WriteLine(userManager.VerifyUser("123456", "123456"));//false
            //userManager.Add(new User() { Account = "02", Password = "02"});
            //userManager.Add(new User() { Password = "donPwd3", Username = "dontext3"});
            //*************************/
            //log.Info(userManager.VerifyUser("Sure", "157"));
            //log.Info(userManager.VerifyUser("da", "25"));

            RoleManager roleManager = new RoleManager();
            //var LevelList = roleManager.GetRoleLevelList();
            //Console.WriteLine(LevelList[1].Role_Id.RoleName);
            //IList<RoleGongFa> gongFas = roleManager.GetGongFaList();
            //foreach (RoleGongFa p in gongFas)
            //{
            //    Console.WriteLine(p.GongFaId + " " + p.Role_Id.RoleJingXue + " " + p.Role_Id.RoleName);
            //}

          IList<Role> roles =  roleManager.GetRoleList();
           int sount =  roles[0].RoleLevelList.Count;
           //Console.WriteLine("sount +" + sount);
            //foreach (Role p1 in roles)
            //{
            //    Console.WriteLine("--" + p1.RoleId);
            //    foreach (RoleGongFa p in p1.RoleGongFaList)
            //    {
            //        Console.WriteLine(p.Role_Id.RoleName);
            //    }
            //}
            Console.ReadKey();
        }
    }
}
