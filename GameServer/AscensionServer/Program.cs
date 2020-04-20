using AscensionServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    class Program
    {
        static void Main(string[] args)
        {
            /*******插入********************
            User user = new User() { Username = "ww8w2w", Password = "e8e2ee"};
            IUserManager userManager = new UserManager();
            userManager.Add(user);
            ***************************/

            /****修改******************
            User user = new User() { ID = 4, Username = "2018", Password = "eedd" };
            IUserManager userManager = new UserManager();
            userManager.Update(user);
            *********************/

            /****删除******************
            User user = new User() { ID = 9};
            IUserManager userManager = new UserManager();
            userManager.Remove(user);
            *********************/

            /****通过ID查找*********************
            IUserManager userManager = new UserManager();
            User user = userManager.GetById(1);
            Console.WriteLine(user.Username);
            Console.WriteLine(user.Password); 
            *********************************/

            /****通过username查找***********************
            IUserManager userManager = new UserManager();
            User user = userManager.GetByUsername("xll");
            Console.WriteLine(user.Username);
            Console.WriteLine(user.Password);
            *****************************/


            /***查询表中所有内容***********************
            IUserManager userManager = new UserManager();
            ICollection<User> users = userManager.GetAllUsers();
            foreach(User u in users)
            {
                Console.WriteLine(u.Username+" "+u.Password);
            }
            **************************/

            //***匹配账户*********************
            IUserManager userManager = new UserManager();
        Console.WriteLine(userManager.VerifyUser("index1", "qqq111"));//true
            Console.WriteLine(userManager.VerifyUser("5l", "eee"));//false
            Console.WriteLine(userManager.GetById(1));

            //*************************/
            Console.ReadKey();
        }
}
}
