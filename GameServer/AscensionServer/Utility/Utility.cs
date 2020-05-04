using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace AscensionServer
{
    public class Utility
    {
        //扩展方法
        public static Value GetValue<Key, Value>(Dictionary<Key, Value> dicr, Key key)
        {
            Value value;

            bool isSuccess = dicr.TryGetValue(key, out value);
            if (isSuccess)
            {
                return value;
            }
            else
            {
                return default(Value);
            }
        }

        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T DeSerialize<T>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }
    }

    public class Singletion
    {
        private static Singletion Instance;

        public Singletion  Get_Instance
        {
            get
            {
                if (Instance == null)
                {
                    Instance = new Singletion();
                }
                return Instance;
            }
        }
    }
}
