using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
  public  class Utility
    {
        #region 2020.4.24 Don
        //public static T GetParameter<T>(Dictionary<byte, object> parameters, ParameterCode parameterCode, bool isObject = true)
        //{
        //    object o = null;
        //    parameters.TryGetValue((byte)parameterCode, out o);
        //    if (isObject == false)
        //    {
        //        return (T)o;
        //    }
        //    return JsonMapper.ToObject<T>(o.ToString());
        //}

        //public static void AddParameter<T>(Dictionary<byte, object> parameters, ParameterCode key, T value,
        //    bool isObject = true)
        //{
        //    if (isObject)
        //    {
        //        string json = JsonMapper.ToJson(value);
        //        parameters.Add((byte)key, json);
        //    }
        //    else
        //    {
        //        parameters.Add((byte)key, value);
        //    }
        //}
        //public static SubCode GetSubcode(Dictionary<byte, object> parameters)
        //{
        //    return GetParameter<SubCode>(parameters, ParameterCode.SubCode, false);
        //}

        //public static void AddSubcode(Dictionary<byte, object> parameters, SubCode subcode)
        //{
        //    AddParameter<SubCode>(parameters, ParameterCode.SubCode, subcode, false);
        //}

        //public static void AddOperationcodeSubcodeRoleID(Dictionary<byte, object> parameters, OperationCode opCode, int roleID)
        //{
        //    if (parameters.ContainsKey((byte)ParameterCode.OperationCode))
        //    {
        //        parameters.Remove((byte)ParameterCode.OperationCode);
        //    }
        //    if (parameters.ContainsKey((byte)ParameterCode.RoleID))
        //    {
        //        parameters.Remove((byte)ParameterCode.RoleID);
        //    }
        //    parameters.Add((byte)ParameterCode.OperationCode, opCode);
        //    parameters.Add((byte)ParameterCode.RoleID, roleID);
        //}
        //public static T2 GetDictValue<T1, T2>(Dictionary<T1, T2> dict, T1 key)
        //{
        //    T2 value;
        //    dict.TryGetValue(key, out value);
        //    return value == null ? default(T2) : value;
        //}
        #endregion

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
}
