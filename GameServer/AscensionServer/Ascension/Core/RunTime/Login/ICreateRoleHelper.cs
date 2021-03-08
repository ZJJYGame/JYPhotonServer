using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer 
{
    public interface ICreateRoleHelper:Cosmos.IReference
    {
        OperationData CreateRole(Dictionary<byte, object> dataMessage);
    }
}
