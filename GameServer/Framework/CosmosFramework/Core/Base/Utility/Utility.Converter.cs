using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos
{
    public  sealed partial class Utility
    {
        public static class Converter
        {
            //TODO Converter 转换工具需要完善
            public static string GetString(byte[] value)
            {
                if (value == null)
                {
                    Debug.LogError(new ArgumentNullException("Value is invalid"));
                }
                return Encoding.UTF8.GetString(value);
            }
            public static byte[] GetBytes(string value)
            {
                return Encoding.UTF8.GetBytes(value);
            }
            //public static byte[] GetBytes(bool value)
            //{

            //}
            public static void GetBytes(bool value,byte[] buffer,int startIndex)
            {
                if (buffer == null)
                {
                  Debug.LogError ( new ArgumentNullException("Buffer is invalid."));
                }
                if(startIndex < 0 || startIndex + 1 > buffer.Length)
                {
                    Debug.LogError( new ArgumentNullException("Start index is invalid."));
                }
                buffer[startIndex] = value ? (byte)1 : (byte)0;
            }
            public static string GetString(byte[] value,int startIndex,int length)
            {
                if (value == null)
                {
                    Debug.LogError(new ArgumentNullException("Value is invalid."));
                }
                return Encoding.UTF8.GetString(value, startIndex, length);
            }
            /// <summary>
            ///T是一个类的对象，由object转换成class对象 
            /// </summary>
            public static T ConvertToObject<T>(object arg)
                where T : class
            {
                return arg as T;
            }
            /// <summary>
            /// object类型转换
            /// </summary>
            public static int Int(object arg)
            {
                return Convert.ToInt32(arg);
            }
            /// <summary>
            /// object类型转换
            /// </summary>
            public static float Float(object arg)
            {
                return (float)System.Math.Round(Convert.ToSingle(arg));
            }
            /// <summary>
            /// object类型转换
            /// </summary>
            public static long Long(object arg)
            {
                return Convert.ToInt64(arg);
            }
            /// <summary>
            /// 约束数值长度，少增多减；
            /// 例如128约束5位等于12800，1024约束3位等于102；
            /// </summary>
            /// <param name="srcValue">原始数值</param>
            /// <param name="length">需要保留的长度</param>
            /// <returns>修改后的int数值</returns>
            public static long RetainInt64(long srcValue, ushort length)
            {
                if (length == 0)
                    return 0;
                var len = srcValue.ToString().Length;
                if (len > length)
                {
                    string sub = srcValue.ToString().Substring(0, length);
                    return long.Parse(sub);
                }
                else
                {
                    var result = srcValue * (long)Math.Pow(10, length - len);
                    return result;
                }
            }
            public static int RetainInt32(int srcValue, ushort length)
            {
                if (length == 0)
                    return 0;
                var len = srcValue.ToString().Length;
                if (len > length)
                {
                    string sub = srcValue.ToString().Substring(0, length);
                    return int.Parse(sub);
                }
                else
                {
                    var result = srcValue * (int)Math.Pow(10, length - len);
                    return result;
                }
            }
        }
    }
}
