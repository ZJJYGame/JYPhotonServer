using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public interface IOperable<T>
        where T:class
    {
        void Add(T data);
        T Get<K>(K dataKey) where K:IComparable;
        void Update(T data);
        void Remove(T data);
    }
}
