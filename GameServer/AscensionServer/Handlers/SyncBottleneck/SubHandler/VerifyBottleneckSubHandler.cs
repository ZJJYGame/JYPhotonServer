using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;
using AscensionProtocol.DTO;
using RedisDotNet;

namespace AscensionServer
{
    class VerifyBottleneckSubHandler : SyncBottleneckSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Verify;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string bottleneckJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleBottleneck));
            var bottleneckObj = Utility.Json.ToObject<Bottleneck>(bottleneckJson);
            NHCriteria nHCriteriabottleneck = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", bottleneckObj.RoleID);
            var bottleneckTemp = NHibernateQuerier.CriteriaSelect<Bottleneck>(nHCriteriabottleneck);
            var roleTemp = NHibernateQuerier.CriteriaSelect<Role>(nHCriteriabottleneck);
            Utility.Debug.LogInfo("yzqData传过来的瓶颈数据" + bottleneckJson);
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, BottleneckData>>(out var bottleneckData);

            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, DemonData>>(out var demonData);

            //Utility.Debug.LogInfo("yzqData数据库获得的数据" + Utility.Json.ToJson(bottleneckTemp));
            if (bottleneckTemp!=null)
            {
                if (bottleneckTemp.BreakThroughVauleMax<= bottleneckObj.BreakThroughVauleNow)
                {
                    if (bottleneckData[roleTemp.RoleLevel].IsFinalLevel)
                    {
                        if (bottleneckTemp.IsDemon)
                        {
                            bottleneckTemp.BreakThroughVauleNow = 0;
                            bottleneckTemp.IsDemon = false;
                            NHibernateQuerier.Update<Bottleneck>(bottleneckTemp);
                            SetResponseParamters(() => {
                                subResponseParameters.Add((byte)ParameterCode.RoleBottleneck, Utility.Json.ToJson(bottleneckTemp));
                                operationResponse.ReturnCode = (short)ReturnCode.Success;
                            });
                        }
                        else
                        {
                            bottleneckTemp.BreakThroughVauleNow = 0;
                            bottleneckTemp.IsDemon = false;
                            bottleneckTemp.IsThunder = false;
                            bottleneckTemp.IsBottleneck = false;
                            NHibernateQuerier.Update<Bottleneck>(bottleneckTemp);
                            SetResponseParamters(() => {
                                subResponseParameters.Add((byte)ParameterCode.RoleBottleneck, Utility.Json.ToJson(bottleneckTemp));
                                operationResponse.ReturnCode = (short)ReturnCode.Success;
                            });
                        }
                    }
                    else
                    {
                        bottleneckTemp.BreakThroughVauleNow = 0;
                        NHibernateQuerier.Update<Bottleneck>(bottleneckTemp);
                        SetResponseParamters(() => {
                            subResponseParameters.Add((byte)ParameterCode.RoleBottleneck, Utility.Json.ToJson(bottleneckTemp));
                            operationResponse.ReturnCode = (short)ReturnCode.Success;
                        });
                    }
                }
                else
                {
                    SetResponseParamters(() => {
                        operationResponse.ReturnCode = (short)ReturnCode.Fail;
                    });
                }
            }else
                SetResponseParamters(() => {
                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            return operationResponse;
        }
    }
}
