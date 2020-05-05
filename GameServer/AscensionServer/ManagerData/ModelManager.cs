/*
*Author   Don
*Since 	2020-04-18
*Description  描述这个类或脚本的具体用途
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
namespace AscensionServer
{
    /// <summary>
    /// 映射模型管理者
    /// </summary>
    public class ModelManager:NHManager<ModelManager>
    {
        void info()
        {
            Add(new Role());
        }
    }
}
