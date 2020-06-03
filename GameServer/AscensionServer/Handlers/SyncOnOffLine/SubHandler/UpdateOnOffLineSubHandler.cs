using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
namespace AscensionServer
{
    class UpdateOnOffLineSubHandler : SyncOnOffLineSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string subDataJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)OperationCode.SubOpCodeData));
            var subDataObj = Utility.ToObject<Dictionary<byte, object>>(subDataJson);
            string roleJsonTmp = Convert.ToString(Utility.GetValue(subDataObj, (byte)ParameterCode.OnOffLine));
            var onofflinetemp = Utility.ToObject<OnOffLine>(roleJsonTmp);
            Owner.OpResponse.OperationCode = operationRequest.OperationCode;
            Owner.ResponseData.Clear();
            NHCriteria nHCriteriaOnoff = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", onofflinetemp.RoleID);

            bool exist = Singleton<NHManager>.Instance.Verify<OnOffLine>(nHCriteriaOnoff);



            if (subDataJson!=null)
            {
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                if (exist)
                {
                    Singleton<NHManager>.Instance.Update(new OnOffLine() { RoleID = onofflinetemp.RoleID, OffLineTime = DateTime.Now.ToString() });

                }
                else
                {

                    //
                    Singleton<NHManager>.Instance.Add(new OnOffLine() { RoleID = onofflinetemp.RoleID, OffLineTime = DateTime.Now.ToString() });

                }
            }else
                 Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaOnoff);
        }
    }
}
