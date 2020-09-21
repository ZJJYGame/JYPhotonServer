using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using NHibernate.Linq.Clauses;
using AscensionProtocol.DTO;
using Renci.SshNet.Security;
using Cosmos;

namespace AscensionServer
{
    public class GetTemoraryRingSubHandler : SyncTemporarySubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            var TemRingRoleData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleTemInventory) as string;
            Utility.Debug.LogInfo(">>>>>Get 临时背包 " + TemRingRoleData + ">>>>>>>>>>>>>");
            var TemRingRoleObj = Utility.Json.ToObject<TemporaryRingDTO>(TemRingRoleData);
            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", TemRingRoleObj.RoleID);
            bool exist = NHibernateQuerier.Verify<RoleRing>(nHCriteriaRoleID);
            if (exist)
            {
                var ringServer = NHibernateQuerier.CriteriaSelect<TemporaryRing>(nHCriteriaRoleID);
                Owner.ResponseData.Add((byte)ParameterCode.RoleTemInventory, ringServer.RingItems);
                Owner.OpResponseData.Parameters = Owner.ResponseData;
                Owner.OpResponseData.ReturnCode = (short)ReturnCode.Success;
            }
            else Owner.OpResponseData.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponseData, sendParameters);
            GameManager.ReferencePoolManager.Despawn(nHCriteriaRoleID);
        }
    }
}
