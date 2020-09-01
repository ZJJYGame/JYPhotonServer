using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;
using System.Security.Principal;

namespace AscensionServer
{
    /// <summary>
    /// peer的变量；可追加DTO；
    /// </summary>
    public class PeerEntity : IKeyValue<byte,DataTransferObject>
    {
        public IPeer ClientPeer { get; private set; }
        Dictionary<GenericValuePair<Type, byte>, Variable> variableDict;
        public PeerEntity()
        {
            variableDict = new Dictionary<GenericValuePair<Type, byte>, Variable>();
        }
        public object GetValue(byte dataKey)
        {
            DataTransferObject dto=null;
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

        }

        public bool TryGetValue(byte key, out DataTransferObject value)
        {
            throw new NotImplementedException();
        }
        public bool ContainsKey(byte key)
        {
            throw new NotImplementedException();
        }

        public bool TryRemove(byte Key)
        {
            throw new NotImplementedException();
        }

        public bool TryAdd(byte key, DataTransferObject Value)
        {
            throw new NotImplementedException();
        }

        public bool TryUpdate(byte key, DataTransferObject newValue, DataTransferObject comparsionValue)
        {
            throw new NotImplementedException();
        }
    }
}
