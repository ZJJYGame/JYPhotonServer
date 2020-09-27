using System;
using System.Collections.Generic;
using System.Text;
namespace AscensionServer
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Interface,AllowMultiple =false,Inherited =true)]
    public class ConfigDataAttribute:Attribute
    {
    }
}
