using System;
using System.Collections.Generic;
using System.Text;
using Cosmos;
namespace AscensionServer
{
    [CustomeModule]
    public class DataManager : Module<DataManager>
    {
        /// <summary>
        /// 间隔秒；
        /// </summary>
        int intervalSec = 1200;
        long latestRefreshTime;
        IDataProvider dataProvider;
        /// <summary>
        /// 对象字典；
        /// </summary>
        Dictionary<Type, object> dataDict;
        /// <summary>
        /// json数据字典；
        /// </summary>
        Dictionary<string, string> jsonDict;
        public override void OnInitialization()
        {
            InitProvider();
            latestRefreshTime = Utility.Time.SecondNow() + intervalSec;
        }
        public override void OnPreparatory()
        {
            var objs = Utility.Assembly.GetInstancesByAttribute<TargetHelperAttribute, IDataConvertor>();
            for (int i = 0; i < objs.Length; i++)
            {
                objs[i].ConvertData();
            }
        }
        /// <summary>
        /// 覆写轮询函数；
        /// 自动更新服务器数据类型；
        /// </summary>
        public override void OnRefresh()
        {
            if (IsPause)
                return;
            var now = Utility.Time.SecondNow();
            if (now >= latestRefreshTime)
            {
                jsonDict = dataProvider?.LoadData() as Dictionary<string, string>;
                dataDict = dataProvider?.ParseData() as Dictionary<Type, object>;
                latestRefreshTime = now + intervalSec;
            }
        }
        public bool ContainsKey(Type key)
        {
            return dataDict.ContainsKey(key);
        }
        public bool TryAdd(Type key, object value)
        {
            return dataDict.TryAdd(key, value);
        }
        public bool TryGetValue(Type key, out object value)
        {
            return dataDict.TryGetValue(key, out value);
        }
        public bool TryRemove(Type key)
        {
            return dataDict.Remove(key);
        }
        public bool TryRemove(Type key, out object value)
        {
            return dataDict.Remove(key, out value);
        }
        public bool ContainsKey<T>()
            where T : class
        {
            return ContainsKey(typeof(T));
        }
        public bool TryAdd<T>(T value)
            where T : class
        {
            return TryAdd(typeof(T), value);
        }
        public bool TryGetValue<T>(out T value)
            where T : class
        {
            value = default;
            object data;
            var result = TryGetValue(typeof(T), out data);
            if (result)
                value = data as T;
            return result;
        }
        public bool TryRemove<T>()
            where T : class
        {
            return TryRemove(typeof(T));
        }
        public bool TryRemove<T>(out T value)
            where T : class
        {
            value = default;
            object data;
            var result = TryRemove(typeof(T), out data);
            if (result)
                value = data as T;
            return result;
        }
        public bool TryAdd(string key, string value)
        {
            return jsonDict.TryAdd(key, value);
        }
        /// <summary>
        /// 通过类名获取json数据；
        /// typeof(Data).Name可作为key；
        /// </summary>
        /// <param name="key">类名</param>
        /// <param name="value">json数据</param>
        /// <returns>是否获取成功</returns>
        public bool TryGetValue(string key, out string value)
        {
            return jsonDict.TryGetValue(key, out value);
        }
        public bool TryGetObjectValue<T>(string key, out T value)
            where T : class
        {
            value = default;
            string json;
            var result = jsonDict.TryGetValue(key, out json);
            if (result)
            {
                value= Utility.Json.ToObject<T>(json);
            }
            return result;
        }
        public bool TryRemove(string key)
        {
            return jsonDict.Remove(key);
        }
        public bool TryRemove(string key, out string value)
        {
            return jsonDict.Remove(key, out value);
        }
        public bool ContainsKey(string key)
        {
            return jsonDict.ContainsKey(key);
        }
        void InitProvider()
        {
            var obj = Utility.Assembly.GetInstanceByAttribute<TargetHelperAttribute>(typeof(IDataProvider));
            dataProvider = obj as IDataProvider;
            jsonDict = dataProvider?.LoadData() as Dictionary<string, string>;
            dataDict = dataProvider?.ParseData() as Dictionary<Type, object>;
        }
    }
}
