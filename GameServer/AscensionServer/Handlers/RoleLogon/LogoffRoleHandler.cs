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
            IRemotePeer peer = Utility.GetValue(operationResponse.Parameters, (byte)ParameterCode.ClientPeer) as IRemotePeer;
            var json = Convert.ToString(Utility.GetValue(operationResponse.Parameters, (byte)ParameterCode.Role));
            var roleObj = Utility.Json.ToObject<RoleDTO>(json);
            var result = GameManager.CustomeModule<RoleManager>().TryRemove(roleObj.RoleID);
            if (result)
            {
                GameManager.CustomeModule<RecordManager>().RecordTime(roleObj.RoleID, null);

                operationResponse.ReturnCode = (byte)ReturnCode.Success;
            }
            else
            {
                operationResponse.ReturnCode = (byte)ReturnCode.ItemNotFound;
            }
            return operationResponse;
        }
    }
}
