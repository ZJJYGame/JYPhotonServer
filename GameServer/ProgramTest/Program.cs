using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgramTest
{
    class Program
    {
        static void Main(string[] args)
        {
             //***匹配账户*********************
            IUserManager userManager = new UserManager();
            Console.WriteLine(userManager.VerifyUser("xll", "eee"));//true
            Console.WriteLine(userManager.VerifyUser("5l", "eee"));//false
            //*************************/
            Console.ReadKey();
        }
    }
}
