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
   public class UpdateOnOffLineSubHandler : SyncOnOffLineSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string subDataJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.OnOffLine));
            var onofflinetemp = Utility.ToObject<OnOffLine>(subDataJson);
           
            NHCriteria nHCriteriaOnoff = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", onofflinetemp.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<OnOffLine>(nHCriteriaOnoff);
            var obj = Singleton<NHManager>.Instance.CriteriaGet<OnOffLine>(nHCriteriaOnoff);
            #region
            //if (subDataJson != null)
            //{
            //    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            //    if (exist)
            //    {
            //        Singleton<NHManager>.Instance.Update(new OnOffLine() { RoleID = onofflinetemp.RoleID, OffLineTime = DateTime.Now.ToString() });
            //    }
            //    else
            //    {

            //        //
            //        Singleton<NHManager>.Instance.Add(new OnOffLine() { RoleID = onofflinetemp.RoleID, OffLineTime = DateTime.Now.ToString() });

            //    }
            //}
            //else
            //    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            #endregion
            Utility.Assert.NotNull(obj.RoleID, () =>
            {
                Singleton<NHManager>.Instance.Update(new OnOffLine() { RoleID = onofflinetemp.RoleID, OffLineTime = DateTime.Now.ToString() });
                SetResponseData(() =>
                {
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }, () => Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail);


            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaOnoff);
        }
    }
}
