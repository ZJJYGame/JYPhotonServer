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
    public class LoginRoleHandler:Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.LoginRole; } }
        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            var json=Convert.ToString( Utility.GetValue(operationResponse.Parameters, (byte)ParameterCode.Role));
            var roleObj = Utility.Json.ToObject<RoleDTO>(json);
            RoleEntity.Create(Convert.ToUInt32( roleObj.RoleID));
            PeerEntity peer;
            var result= GameManager.CustomeModule<PeerManager>().TryGetValue(roleObj.RoleID, out peer);
            operationResponse.ReturnCode = (byte)ReturnCode.Success;
            return operationResponse;
        }
    }
}
