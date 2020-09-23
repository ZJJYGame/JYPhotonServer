using System;
namespace Runner
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Interface,AllowMultiple =false,Inherited =true)]
    public class InheritedAttribute : Attribute{}
}
