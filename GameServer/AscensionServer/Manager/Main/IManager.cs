using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
   public interface IManager
    {
        void Add<T>(T data) where T : class, new();
        void Update<T>(T data) where T:new();
        void Remove<T>(T data) where T:new();
        T Get<T, K>(K key) where T : new() where K:IComparable<K>;
    }
}
