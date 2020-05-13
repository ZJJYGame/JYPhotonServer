using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    //TODO IOperable类完善
    public interface IOperable
    {
        void Add<T>(T data);
        T Get<T, K>(K dataKey);
        void Update<T>();
    }
}
