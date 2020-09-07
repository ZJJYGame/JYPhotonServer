using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public interface  ISimpleKeyValue<TKey,TValue>
    {
        bool TryGetValue(TKey key, out TValue value);
        bool ContainsKey(TKey key);
        bool TryRemove(TKey key);
        bool TryAdd(TKey key, TValue Value);
    }
}
