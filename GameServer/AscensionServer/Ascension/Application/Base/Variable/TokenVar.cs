using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class TokenVar : NetVariable
    {
        public override Type Type { get { return this.GetType(); } }
        /// <summary>
        /// string类型的token
        /// </summary>
        object token;
        public override object GetValue()
        {
            return token;
        }
        public override void SetValue(object value)
        {
            token = value;
        }
    }
}
