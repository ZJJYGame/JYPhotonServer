using AscensionProtocol;
using AscensionProtocol.DTO;
using Cosmos;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class LoginRoleHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.LoginRole; } }
        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            var json = Convert.ToString(Utility.GetValue(operationResponse.Parameters, (byte)ParameterCode.Role));
            var roleObj = Utility.Json.ToObject<RoleDTO>(json);
            var role = RoleEntity.Create(roleObj.RoleID, roleObj);
            var result = GameManager.CustomeModule<RoleManager>().TryAdd(role.RoleId, role);
            if (result)
                operationResponse.ReturnCode = (byte)ReturnCode.Success;
            else
                operationResponse.ReturnCode = (byte)ReturnCode.ItemAlreadyExists;
            return operationResponse;
        }
    }
}
