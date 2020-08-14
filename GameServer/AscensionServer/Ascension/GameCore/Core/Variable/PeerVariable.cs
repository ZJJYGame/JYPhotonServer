using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;
namespace AscensionServer
{
    /// <summary>
    /// peer的变量；可追加DTO；
    /// </summary>
    public class PeerVariable : Variable
    {
        public override Type Type { get { return this.GetType(); } }
        ConcurrentDictionary<byte, DataTransferObject> dtoDict;
        public override void OnInitialization()
        {
            dtoDict = new ConcurrentDictionary<byte, DataTransferObject>();
        }
        public override void OnTermination()
        {
            dtoDict.Clear();
        }
        public object GetValue(byte dataKey)
        {
            DataTransferObject dto;
            dtoDict.TryGetValue(dataKey, out dto);
            return dto;
        }
        /// <summary>
        /// 设置值；
        /// 若为空，则添加；
        /// 若非空，则更新；
        /// </summary>
        /// <param name="dataKey"></param>
        /// <param name="value"></param>
        public void SetValue(byte dataKey, object value)
        {
            if (dtoDict.ContainsKey(dataKey))
                dtoDict.TryAdd(dataKey, value as DataTransferObject);
            else
                dtoDict[dataKey] = value as DataTransferObject;
        }
    }
}
