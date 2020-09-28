using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public interface IRoleEntity: IKeyValue<Type, object>, IReference
    {
        int RoleId { get; }
        object[] Find(Predicate<object> handler);
    }
}
