using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionServer
{
    [Serializable]
    public class RedisConfig: Data
    {
        /// <summary>
        ///192.168.0.117:6379,password=123456,DefaultDatabase=3
        /// </summary>
        public string Configuration { get; set; }

        public override void SetData(object data)
        {
            var dat = data as RedisConfig;
            Configuration = dat.Configuration;
        }

        public override string ToString()
        {
            return "Configuration :" + Configuration;
        }
    }
}
