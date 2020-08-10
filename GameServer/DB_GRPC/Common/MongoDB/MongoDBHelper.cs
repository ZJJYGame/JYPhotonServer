using MagicOnion;
using MagicOnion.Server;
using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;
using System.Threading.Tasks;
namespace CF_DB_Manager
{
    public class MongoDBHelper:ServiceBase<IMongoDBHelper>,IMongoDBHelper
    {
    }
}
