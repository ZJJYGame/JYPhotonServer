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
            IRemotePeer peer = Utility.GetValue(operationResponse.Parameters, (byte)ParameterCode.ClientPeer) as IRemotePeer;
            var json = Convert.ToString(Utility.GetValue(operationResponse.Parameters, (byte)ParameterCode.Role));
            var roleObj = Utility.Json.ToObject<RoleDTO>(json);
            var role = RemoteRole.Create(roleObj.RoleID, roleObj);
            IRemoteRole remoteRole;
            var roleExist= GameManager.CustomeModule<RoleManager>().TryGetValue(role.RoleId, out remoteRole);
            bool result=false;
            if (roleExist)
                result = GameManager.CustomeModule<RoleManager>().TryUpdate(role.RoleId, role, remoteRole);
            if (result)
                operationResponse.ReturnCode = (byte)ReturnCode.Success;
            else
                operationResponse.ReturnCode = (byte)ReturnCode.ItemAlreadyExists;
            return operationResponse;
        }
    }
}
