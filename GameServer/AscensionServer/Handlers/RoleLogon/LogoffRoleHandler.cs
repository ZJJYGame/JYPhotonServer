using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Cosmos;
using Photon.SocketServer;

namespace AscensionServer
{
    public class LogoffRoleHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.LogoffRole; } }
        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            var json = Convert.ToString(Utility.GetValue(operationResponse.Parameters, (byte)ParameterCode.Role));
            var roleObj = Utility.Json.ToObject<RoleDTO>(json);
            RoleEntity.Create(Convert.ToUInt32(roleObj.RoleID));
            operationResponse.ReturnCode = (byte)ReturnCode.Success;
            return operationResponse;
        }
    }
}
