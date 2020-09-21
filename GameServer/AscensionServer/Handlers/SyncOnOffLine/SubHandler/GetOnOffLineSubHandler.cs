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
            string subDataJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.OnOffLine));
            var onofflinetemp = Utility.Json.ToObject<OnOffLine>(subDataJson);
            Bottleneck bottleneck = new Bottleneck() {RoleID= onofflinetemp.RoleID };
            NHCriteria nHCriteriabottleneck = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", bottleneck.RoleID);
            NHCriteria nHCriteriaRole = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", onofflinetemp.RoleID);
            var bottleneckObj= NHibernateQuerier.CriteriaSelect<Bottleneck>(nHCriteriabottleneck);
            ///获取的时间秒
            OffLineTimeDTO offLineTime = new OffLineTimeDTO() { RoleID = onofflinetemp.RoleID };
            var obj = NHibernateQuerier.CriteriaSelect<OffLineTime>(nHCriteriaRole);
            TimeSpan interval = (DateTime.Now).Subtract(Convert.ToDateTime(obj.OffTime));

            if (obj != null)
            {
                var Exptypeobj = NHibernateQuerier.CriteriaSelect<OnOffLine>(nHCriteriaRole);
                if (Exptypeobj.ExpType==1)

                {
                    List<int> date = new List<int>();
                    int AllExperience = (int)(onofflinetemp.GongFaExp * interval.TotalSeconds / 5);
                    date.Add(AllExperience);
                    date.Add(Exptypeobj.MsGfID);
                    date.Add(Exptypeobj.ExpType);
                    if (bottleneckObj!=null)
                    {
                        date.Add(Convert.ToByte(bottleneckObj.IsBottleneck));
                    }else
                        date.Add(0);
                    Utility.Debug.LogInfo(">>>>>>>>>>>>>>>>>>>>>>>>>得到的离线时间1" + Exptypeobj.MsGfID+"id"+ Exptypeobj.ExpType);
                    SetResponseData(() =>
                    {
                        SubDict.Add((byte)ParameterCode.OnOffLine, Utility.Json.ToJson(date));
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
                    if (bottleneckObj != null)
                    {
                        date.Add(Convert.ToByte(bottleneckObj.IsBottleneck));
                    }
                    else
                        date.Add(0);
                    Utility.Debug.LogInfo(">>>>>>>>>>>>>>>>>>>>>>>>>得到的离线时间2" + Exptypeobj.MsGfID + "id" + Exptypeobj.ExpType);
                    SetResponseData(() =>
                    {
                        SubDict.Add((byte)ParameterCode.OnOffLine, Utility.Json.ToJson(date));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
                else
                {
                    SetResponseData(() =>
                    {
                        SubDict.Add((byte)ParameterCode.OnOffLine, Utility.Json.ToJson(new List<string>()));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                    });
                }
                }else{
                SetResponseData(() =>
                {
                SubDict.Add((byte)ParameterCode.OnOffLine, Utility.Json.ToJson(new List<string>()));
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                });
                }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaRole);
        }
    }
    }

