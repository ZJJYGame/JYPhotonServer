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
using Cosmos;
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
            var onofflinetemp = Utility.Json.ToObject<OnOffLine>(subDataJson);
            NHCriteria nHCriteriaRole = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", onofflinetemp.RoleID);
            ///获取的时间秒
            OffLineTimeDTO offLineTime = new OffLineTimeDTO() { RoleID = onofflinetemp.RoleID };
            var obj = Singleton<NHManager>.Instance.CriteriaSelect<OffLineTime>(nHCriteriaRole);
            TimeSpan interval = (DateTime.Now).Subtract(Convert.ToDateTime(obj.OffTime));
            if (obj != null)
            {
                var Exptypeobj = Singleton<NHManager>.Instance.CriteriaGet<OnOffLine>(nHCriteriaRole);
                if (Exptypeobj.ExpType==1)

                {
                    List<int> date = new List<int>();
                    int AllExperience = (int)(onofflinetemp.GongFaExp * interval.TotalSeconds / 5);
                    date.Add(AllExperience);
                    date.Add(Exptypeobj.MsGfID);
                    date.Add(Exptypeobj.ExpType);

                    SetResponseData(() =>
                    {
                        //AscensionServer._Log.Info("计算出功法的离线经验>>>>>>>>>" + onofflinetemp.MsGfID.ToString() + ">>>>>>>>>>>>");
                        SubDict.Add((byte)ObjectParameterCode.OnOffLine, Utility.Json.ToJson(date));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
                else if (Exptypeobj.ExpType==2)

                {
                    List<int> date = new List<int>();
                    int AllExperience =(int)(onofflinetemp.MiShuExp * interval.TotalSeconds / 5);
                    date.Add(AllExperience);
                    date.Add(Exptypeobj.MsGfID);
                    date.Add(Exptypeobj.ExpType);
                    SetResponseData(() =>
                    {
                        //AscensionServer._Log.Info("计算出秘書的离线经验>>>>>>>>>" + onofflinetemp.MsGfID.ToString() + ">>>>>>>>>>>>");
                        SubDict.Add((byte)ObjectParameterCode.OnOffLine, Utility.Json.ToJson(date));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
                else
                {
                    SetResponseData(() =>
                    {
                        // AscensionServer._Log.Info("计算出的离线经验>>>>>>>>> 爲空 >>>>>>>>>>>>");
                        SubDict.Add((byte)ObjectParameterCode.OnOffLine, Utility.Json.ToJson(new List<string>()));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                    });
                }
                }else{
                SetResponseData(() =>
                {
                        AscensionServer._Log.Info("计算出的离线经验>>>>>>>>> 失敗 >>>>>>>>>>>>");
                SubDict.Add((byte)ObjectParameterCode.OnOffLine, Utility.Json.ToJson(new List<string>()));
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                });
                }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRole);
        }
    }
    }

