using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Utility
{
    public class Singletion
    {
        private static Singletion Instance;

        public Singletion Get_Instance
        {
            get
            {
                if (Instance == null)
                {
                    Instance = new Singletion();
                }
                return Instance;
            }
        }
    }
}
