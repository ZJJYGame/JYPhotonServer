using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;

namespace AscensionServer
{
   public class SyncUpdatSchoolRefreshHandler: Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.UpdateSchoolRefresh;
            base.OnInitialization();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResponseData.Clear();
            string schoolJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)
                ParameterCode.School));
            Utility.Debug.LogInfo("更新前的宗门信息");
            var schoolObj = Utility.Json.ToObject<School>(schoolJson);
            NHCriteria nHCriteriaschool = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", schoolObj.ID);
            var schooltemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<School>(nHCriteriaschool);
            if (schooltemp != null)
            {
                if (schoolObj.GetContributions >= 0)
                {
                    schooltemp.ContributionNow += schoolObj.GetContributions;
                    schooltemp.IsSignin = schoolObj.IsSignin;
                    ConcurrentSingleton<NHManager>.Instance.Update<School>(schooltemp);
                    OpResponse.ReturnCode = (byte)ReturnCode.Success;
                    ResponseData.Add((byte)ParameterCode.SchoolRefresh, Utility.Json.ToJson(schooltemp));
                    OpResponse.OperationCode = operationRequest.OperationCode;
                    OpResponse.Parameters = ResponseData;
                }
                else
                {
                   OpResponse.ReturnCode = (byte)ReturnCode.Fail;
                }
            }
            Utility.Debug.LogInfo("更新后的宗门信息" + Utility.Json.ToJson(schooltemp));
            peer.SendOperationResponse(OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaschool);
        }
    }
}
