using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AscensionServer;
using AscensionServer.Manager;
namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

            IUserManager userManager = new UserManager();
            Console.WriteLine(userManager.VerifyUser("xll", "eee"));//true
            Console.WriteLine(userManager.VerifyUser("5l", "eee"));//false
            //*************************/
            Console.ReadKey();
        }
    }
}
