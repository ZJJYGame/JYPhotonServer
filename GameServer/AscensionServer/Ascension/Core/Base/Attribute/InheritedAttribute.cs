using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =true)]
    public class InheritedAttribute : Attribute
    {
    }
}
