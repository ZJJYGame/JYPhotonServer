using AscensionProtocol;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using Cosmos;
using AscensionProtocol.DTO;
using EventData = Photon.SocketServer.EventData;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;
namespace AscensionServer
{
  public  class SyncSchoolRefreshHandler: Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncSchoolRefresh; } }
        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {              
            responseParameters.Clear();
            var roleJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Role));
            var schoolJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.SchoolRefresh));
            var roleObj = Utility.Json.ToObject<RoleDTO>(roleJson);
            var schoolObj = Utility.Json.ToObject<SchoolDTO>(schoolJson);
            //if (!AscensionServer.Instance.RefreshPool.IsExists(roleObj.RoleID.ToString()) && schoolObj.SchoolID != 900)
            //{
            //    AscensionServer.Instance.RefreshPool.Add(roleObj.RoleID.ToString(), peer);
            //    ResponseData.Add((byte)ParameterCode.SchoolRefresh, Utility.Json.ToJson(true));
            //    //EventData eventData = new EventData((byte)EventCode.SchoolRefresh);
            //    //eventData.Parameters = data;
            //    //peer.SendEvent(eventData, new SendParameters());
            //    Utility.Debug.LogInfo("派发刷新商店的事件给服务器");
            //    OpResponse.Parameters = ResponseData;
            //    OpResponse.OperationCode = operationRequest.OperationCode;
            //    OpResponse.ReturnCode = (short)ReturnCode.Success;
            //    peer.SendOperationResponse(OpResponse, sendParameters);
            //}
            //else
            //{
            //    Utility.Debug.LogInfo("派发刷新商店的事件给服务器1》》》》》》》》》》》》》》》》" + schoolJson);
            //    Utility.Debug.LogInfo("派发刷新商店的事件给服务器2》》》》》》》》》》》》》》》》" + roleJson);
            //}
            return operationResponse;
        }
    }
}


