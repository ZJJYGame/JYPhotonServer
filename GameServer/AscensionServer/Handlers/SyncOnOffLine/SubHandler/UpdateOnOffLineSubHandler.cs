/*
*Author : yingduan
*Since :	2020-05-26
*Description :  离线时间记录
*/
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
            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>收到離綫時間");
            var dict = ParseSubDict(operationRequest);
            string subDataJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.OnOffLine));
            /////
            //OnOffLine onOffLine = new OnOffLine() { RoleID=33};
            //string str = Convert.ToString(onOffLine);
            //////
            var onofflinetemp = Utility.ToObject<OnOffLine>(subDataJson);
           
            NHCriteria nHCriteriaOnoff = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", onofflinetemp.RoleID);
            var obj = Singleton<NHManager>.Instance.CriteriaGet<OnOffLine>(nHCriteriaOnoff);
            Utility.Assert.NotNull(obj, () =>
            {
                Singleton<NHManager>.Instance.Update(onofflinetemp);
                SetResponseData(() =>
                {
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }, () => { Singleton<NHManager>.Instance.Add(onofflinetemp); });

            
            //peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaOnoff);
        }
    }
}
