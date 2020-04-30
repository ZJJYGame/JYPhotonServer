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
            UserManager manager = new UserManager();
            RoleManager manager1 = new RoleManager();
            IList<Role> roles = manager1.GetRoleList();
            Console.WriteLine("count " + roles.Count);
            //foreach (var item in roles)
            //{
            //    Console.WriteLine(item._User.Account);
            //}
            //RoleManager roleManager = new RoleManager();
            //Role role = roleManager.GetByRolename(rolename);//根据username查询数据
            //ICollection<Role> rolelist = roleManager.GetAllRoles();
            //AscensionServer.log.Info(rolelist.Count);
            //User user = manager.GetArticleContent(new User() { Account = "1" });

            // ICollection < User > rolelist =  manager.GetAllUsers();
            //IList<User> roles = manager.GetUserList();
            //foreach (var p1 in roles)
            //{
                
            //    if (p1.Account =="2")
            //    {
            //        Console.WriteLine(p1.RoleList.Count);
            //        foreach (var p in p1.RoleList)
            //        {
            //            Console.WriteLine(p.RoleUserAccount);
            //        }
            //    }
                    //Console.WriteLine(p1.RoleList.Count);
                    //foreach (Role p in p1.RoleList)
                    //{
                    //    Console.WriteLine(p.RoleName);
                
            
                //  }
                // Console.WriteLine(user.Role.RoleName);
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
            //Console.WriteLine(userManager.VerifyUser("01", "01"));//true
            //Console.WriteLine(userManager.GetByUsername("index2"));
            //Console.WriteLine(userManager.GetById(1).Username);

           //User user = userManager.GetByUsername("index2");
            //userManager.Remove(new User() { Username = "index3", Password = "qq3", Age = "13" });
            //Console.WriteLine( "表里的名字"+ user.Password);

            //Console.WriteLine(userManager.VerifyUser("123456", "123456"));//false
           // userManager.Add(new User() { Account = "102", Password = "102"});
            //userManager.Add(new User() { Password = "donPwd3", Username = "dontext3"});
            //*************************/
            //log.Info(userManager.VerifyUser("Sure", "157"));
            //log.Info(userManager.VerifyUser("da", "25"));

            RoleManager roleManager = new RoleManager();
            //roleManager.AddRole(new Role() { RoleName = "12", RoleRoot = "123", RoleUserAccount = "3" });

            //var LevelList = roleManager.GetRoleLevelList();
            //Console.WriteLine(LevelList[1].Role_Id.RoleName);
            //IList<RoleGongFa> gongFas = roleManager.GetGongFaList();
            //foreach (RoleGongFa p in gongFas)
            //{
            //    Console.WriteLine(p.GongFaId + " " + p.Role_Id.RoleJingXue + " " + p.Role_Id.RoleName);
            //}
            //roleManager.AddRole(new Role() { RoleName = "宫本7号",RoleRoot = "1,2" });
           // IList<Role> roles =  roleManager.GetRoleList();
            //ICollection<Role> rolelist = roleManager.GetAllRoles();
            //Console.WriteLine("角色名字" + rolelist.Count);
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
