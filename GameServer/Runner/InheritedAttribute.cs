using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Interface,AllowMultiple =false,Inherited =true)]
    public class InheritedAttribute : Attribute
    {
    }
}
