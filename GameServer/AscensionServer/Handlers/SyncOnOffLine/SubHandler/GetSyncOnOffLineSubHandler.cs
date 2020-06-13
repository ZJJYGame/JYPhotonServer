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
using AscensionProtocol.DTO;
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
            OffLineTimeDTO offLineTime = new OffLineTimeDTO() { RoleID = onofflinetemp.RoleID };
            string onofflineJson = Utility.ToJson(offLineTime);
            //////
            var offtimetemp = Utility.ToObject<OffLineTime>(onofflineJson);

            NHCriteria nHCriteriaOnoff = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", onofflinetemp.RoleID);
            var obj = Singleton<NHManager>.Instance.CriteriaGet<OffLineTime>(nHCriteriaOnoff);
            TimeSpan interval = (DateTime.Now).Subtract(Convert.ToDateTime(obj.OffTime));

            bool exist = Singleton<NHManager>.Instance.Verify<OnOffLine>(nHCriteriarole);
            Utility.Assert.NotNull(obj, ()=>
            {
                var Exptypeobj = Singleton<NHManager>.Instance.CriteriaGet<OnOffLine>(nHCriteriarole);
                if (Exptypeobj.ExpType==0)
                {
                    List<string> date = new List<string>();
                    string AllExperience = (onofflinetemp.GongFaExp * interval.TotalSeconds / 5).ToString();
                    date.Add(AllExperience);
                    date.Add(Exptypeobj.ExpType.ToString());

                    SetResponseData(() =>
                    {
                        //AscensionServer._Log.Info("计算出功法的离线经验>>>>>>>>>" + AllExperience + ">>>>>>>>>>>>");
                        SubDict.Add((byte)ObjectParameterCode.OnOffLine, Utility.ToJson(date));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
                else if (Exptypeobj.ExpType== 1)
                {
                    List<string> date = new List<string>();
                    string AllExperience = (onofflinetemp.MiShuExp * interval.TotalSeconds / 5).ToString();
                    date.Add(AllExperience);
                    date.Add(Exptypeobj.ExpType.ToString());
                    SetResponseData(() =>
                    {
                        //AscensionServer._Log.Info("计算出秘書的离线经验>>>>>>>>>" + AllExperience + ">>>>>>>>>>>>");
                        SubDict.Add((byte)ObjectParameterCode.OnOffLine, Utility.ToJson(date));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
                else {
                SetResponseData(() =>
                {
                   // AscensionServer._Log.Info("计算出的离线经验>>>>>>>>> 爲空 >>>>>>>>>>>>");
                    SubDict.Add((byte)ObjectParameterCode.OnOffLine, Utility.ToJson(new List<string>()));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                });
        }
            },() => SetResponseData(() =>
            {
               // AscensionServer._Log.Info("计算出的离线经验>>>>>>>>> 失敗 >>>>>>>>>>>>");
                SubDict.Add((byte)ObjectParameterCode.OnOffLine, Utility.ToJson(new List<string>()));
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }));

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriarole);
        }
    }
    }

