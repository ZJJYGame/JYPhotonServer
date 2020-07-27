using AscensionProtocol;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using Cosmos;
using AscensionProtocol.DTO;
using AscensionServer.Threads;
using EventData = Photon.SocketServer.EventData;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;
namespace AscensionServer
{
  public  class SyncSchoolRefreshHandler: Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncSchoolRefresh;
            base.OnInitialization();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {              
            ResponseData.Clear();
            var roleJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.SchoolRefresh));
            var roleObj = Utility.Json.ToObject<RoleDTO>(roleJson);
            if (!AscensionServer.Instance.RefreshPool.IsExists(roleObj.RoleID.ToString()))
            {
                AscensionServer.Instance.RefreshPool.Add(roleObj.RoleID.ToString(),peer);
            }
        }
    }
}
