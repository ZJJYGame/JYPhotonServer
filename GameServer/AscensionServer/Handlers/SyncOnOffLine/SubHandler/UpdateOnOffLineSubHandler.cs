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
using Cosmos;
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
            string subDataJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.OnOffLine));
            var onofflinetemp = Utility.Json.ToObject<OnOffLine>(subDataJson);
            NHCriteria nHCriteriaOnoff = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", onofflinetemp.RoleID);
            var obj = NHibernateQuerier.CriteriaSelect<OnOffLine>(nHCriteriaOnoff);
            if (obj != null)
            {
                obj.MsGfID = onofflinetemp.MsGfID;
                obj.ExpType = onofflinetemp.ExpType;
                NHibernateQuerier.Update(obj);
                SetResponseData(() =>
                {
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }
            else
            {
                NHibernateQuerier.Insert(onofflinetemp);
            }
            //peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaOnoff);
        }
    }
}
