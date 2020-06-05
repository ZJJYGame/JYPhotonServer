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
           // AscensionServer._Log.Info("传过来的上线时间及功法id>>>>>>>>>" + subDataJson + ">>>>>>>>>>>>>");


            ///获取的时间秒
            var rsult = Singleton<NHManager>.Instance.CriteriaGet<OnOffLine>(nHCriteriarole);
            TimeSpan interval = (DateTime.Now).Subtract(Convert.ToDateTime(rsult.OffLineTime));
            //AscensionServer._Log.Info("传过来的上线时间及功法id>>>>>>>>>" + "测试" + interval .TotalSeconds.ToString()+ ">>>>>>>>>>>>>");
            //OnOffLine temp = Singleton<NHManager>.Instance.CriteriaGet<OnOffLine>(nHCriteriarole);
            bool exist = Singleton<NHManager>.Instance.Verify<OnOffLine>(nHCriteriarole);
            #region
            // AscensionServer._Log.Info("计算出的离线经验>>>>>>>>>" + exist + ">>>>>>>>>>>>");
            //if (exist)
            //{
            //    
            //    string AllExperience = (35 * interval.TotalSeconds).ToString();
            //    AscensionServer._Log.Info("计算出的离线经验>>>>>>>>>" + AllExperience + ">>>>>>>>>>>>");
            //    Owner.ResponseData.Add((byte)ObjectParameterCode.OnOffLine, AllExperience);
            //    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            //    Owner.OpResponse.Parameters = Owner.ResponseData;
            //}
            //else
            //{
            //    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            //}
            #endregion
            Utility.Assert.NotNull(rsult, ()=>
            {
                string AllExperience = (35 * interval.TotalSeconds).ToString();
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

