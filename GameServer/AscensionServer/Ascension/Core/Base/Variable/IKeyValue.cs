﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public interface IKeyValue<TKey,KValue>
    {
        bool TryGetValue(TKey key,out KValue value);
        bool ContainsKey(TKey key);
        bool TryRemove(TKey key);
        bool TryAdd(TKey key,KValue Value);
        bool TryUpdate(TKey key,KValue newValue,KValue comparsionValue);
    }
}