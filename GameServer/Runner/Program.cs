using System;
using StackExchange.Redis;
using Cosmos;
using RedisDotNet;
using System.Collections.Generic;
using System.Reflection;
using Protocol;
namespace Runner
{
    class Program
    {
        static string path = Environment.CurrentDirectory + "/IO";
        static void Main(string[] args)
        {
            Utility.Json.SetHelper(new NewtonjsonHelper());
            Utility.MessagePack.SetHelper(new ImplMessagePackHelper());
            RoleStatusDatas rsd = new RoleStatusDatas()
            {
                MoveSpeed = 10,
                RoleHP = 666,
                RoleMP = 999,
                AttackPower = 78,
                AttackSpeed = 55,
                DefendPower = 88,
                RoleSoul = 76,
                BestBlood = 99
            };
            RoleStatusDTO rsdto = new RoleStatusDTO();
            Utility.Assembly.AssignSameFiledValue(rsd, rsdto);
            Console.WriteLine(Utility.Assembly.TraverseInstanceAllFiled(rsdto));
            Console.ReadKey();
        }
    }
}
