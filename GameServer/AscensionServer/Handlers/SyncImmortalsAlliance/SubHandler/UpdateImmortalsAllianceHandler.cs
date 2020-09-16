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
using RedisDotNet;
using StackExchange.Redis;

namespace AscensionServer
{
    public class UpdateImmortalsAllianceHandler : SyncImmortalsAllianceSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string immortalsAllianceJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.ImmortalsAlliance));
            var immortalsAllianceObj = Utility.Json.ToObject<AllianceStatusDTO>
                (immortalsAllianceJson);
            Utility.Debug.LogError("发送仙盟宗旨修改成功"+ immortalsAllianceJson);
            var allianceMemberTemp = AlliancelogicManager.Instance.GetNHCriteria<AllianceStatus>("ID", immortalsAllianceObj.ID);
            Utility.Debug.LogError("发送仙盟宗旨修改成功"+ allianceMemberTemp.ID);
            if (allianceMemberTemp!=null)
            {
                if (allianceMemberTemp.AllianceName == immortalsAllianceObj.AllianceName)
                {
                    allianceMemberTemp.Manifesto = immortalsAllianceObj.Manifesto;
                    SetResponseData(() =>
                    {
                        Utility.Debug.LogError("发送仙盟宗旨修改成功");
                        SubDict.Add((byte)ParameterCode.ImmortalsAlliance, Utility.Json.ToJson(allianceMemberTemp));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
               
            }else
                SetResponseData(() =>
                {
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                });


            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
}
