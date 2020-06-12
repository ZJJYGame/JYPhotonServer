/*
*Author : yingduan
*Since :	2020-06-1
*Description :  离线经验
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
   public  class GetSyncOnOffLineSubHandler : SyncOnOffLineSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string subDataJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.OnOffLine));
            var onofflinetemp = Utility.ToObject<OnOffLine>(subDataJson);
            NHCriteria nHCriteriarole = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", onofflinetemp.RoleID);

            ///获取的时间秒
            var rsult = Singleton<NHManager>.Instance.CriteriaGet<OnOffLine>(nHCriteriarole);
            TimeSpan interval = (DateTime.Now).Subtract(Convert.ToDateTime(rsult.OffLineTime));

            bool exist = Singleton<NHManager>.Instance.Verify<OnOffLine>(nHCriteriarole);
            Utility.Assert.NotNull(rsult, ()=>
            {
                string AllExperience = (onofflinetemp.UpgradeExp * interval.TotalSeconds/5).ToString();
                AscensionServer._Log.Info("计算出的离线经验>>>>>>>>>" + AllExperience + ">>>>>>>>>>>>");
                SetResponseData(()=>
                {
                    SubDict.Add((byte)ObjectParameterCode.OnOffLine, AllExperience);
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            },()=> Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail);

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriarole);
        }
    }
    }

