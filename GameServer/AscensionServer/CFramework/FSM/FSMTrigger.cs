using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public abstract class FSMTrigger<T> : IBehaviour,IReference
        where T : class
    {
        public abstract void Clear();
        public abstract void OnInitialization();
        public abstract void OnTermination();
    }
}
