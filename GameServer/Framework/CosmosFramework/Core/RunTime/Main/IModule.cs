using System.Collections;
using System;
namespace Cosmos
{
    public interface IModule: IControllableBehaviour,IOperable
    {
        string ModuleFullyQualifiedName { get; }
        ModuleEnum ModuleEnum { get; }
    }
}
