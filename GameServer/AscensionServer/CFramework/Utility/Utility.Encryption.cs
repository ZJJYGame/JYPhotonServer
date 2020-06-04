using System.Collections;
using System.Collections.Generic;
using System;
namespace AscensionServer
{
    public sealed partial class Utility
    {
        /// <summary>
        /// 加密工具
        /// </summary>
        public sealed class Encryption
        {
            public  enum GUIDFormat
            {
                N,D,B,P,X
            }
            public static string GUID(GUIDFormat format)
            {
                return Guid.NewGuid().ToString(format.ToString());
            }
        }
    }
}