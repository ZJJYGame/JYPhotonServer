using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    public interface IDataManager:IModuleManager
    {
        bool ContainsKey(Type key);
        bool TryAdd(Type key, object value);
        bool TryGetValue(Type key, out object value);
        bool TryRemove(Type key);
        bool TryRemove(Type key, out object value);
        bool ContainsKey<T>() where T : class;
        bool TryAdd<T>(T value) where T : class;
        bool TryGetValue<T>(out T value) where T : class;
        bool TryRemove<T>() where T : class;
        bool TryRemove<T>(out T value) where T : class;
        bool TryAdd(string key, string value);
        /// <summary>
        /// 通过类名获取json数据；
        /// typeof(Data).Name可作为key；
        /// </summary>
        /// <param name="key">类名</param>
        /// <param name="value">json数据</param>
        /// <returns>是否获取成功</returns>
        bool TryGetValue(string key, out string value);
        bool TryGetObjectValue<T>(string key, out T value) where T : class;
        bool TryRemove(string key);
        bool TryRemove(string key, out string value);
        bool ContainsKey(string key);

        void ClearAll();
    }
}
;

