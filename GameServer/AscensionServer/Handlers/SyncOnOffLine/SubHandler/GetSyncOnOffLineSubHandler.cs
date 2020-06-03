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
        
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {

            string subDataJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)OperationCode.SubOpCodeData));
            var subDataObj = Utility.ToObject<Dictionary<byte, object>>(subDataJson);
            string roleJsonTmp = Convert.ToString(Utility.GetValue(subDataObj, (byte)ParameterCode.OnOffLine));
            
            Owner.ResponseData.Clear();
            Owner.OpResponse.OperationCode = operationRequest.OperationCode;
            Owner.ResponseData.Add((byte)OperationCode.SubOperationCode, (byte)SubOpCode);

            var onofflinetemp = Utility.ToObject<OnOffLine>(roleJsonTmp);
            NHCriteria nHCriteriarole = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", onofflinetemp.RoleID);
            //AscensionServer._Log.Info("传过来的上线时间及功法id>>>>>>>>>" + roleJsonTmp + ">>>>>>>>>>>>>");


            ///获取的时间秒
            var rsult = Singleton<NHManager>.Instance.CriteriaGet<OnOffLine>(nHCriteriarole);
            TimeSpan interval = (DateTime.Now).Subtract(Convert.ToDateTime(rsult.OffLineTime));
            //AscensionServer._Log.Info("传过来的上线时间及功法id>>>>>>>>>" + "测试" + interval .TotalSeconds.ToString()+ ">>>>>>>>>>>>>");
            //OnOffLine temp = Singleton<NHManager>.Instance.CriteriaGet<OnOffLine>(nHCriteriarole);
            bool exist = Singleton<NHManager>.Instance.Verify<OnOffLine>(nHCriteriarole);
           // AscensionServer._Log.Info("计算出的离线经验>>>>>>>>>" + exist + ">>>>>>>>>>>>");
            if (exist)
            {
                //int Exp = Singleton<NHManager>.Instance.CriteriaGet<GongFa>(nHCriteriaID).GongFaExp;
                string AllExperience = (35 * interval.TotalSeconds).ToString();
                AscensionServer._Log.Info("计算出的离线经验>>>>>>>>>" + AllExperience + ">>>>>>>>>>>>");
                Owner.ResponseData.Add((byte)ParameterCode.OnOffLine, AllExperience);
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                Owner.OpResponse.Parameters = Owner.ResponseData;
            }
            else
            {
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriarole);
        }
    }
    }

