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
    public class AddBottleneckSubHandler : SyncBottleneckSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Add;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string bottleneckJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleBottleneck));
            var bottleneckObj = Utility.Json.ToObject<Bottleneck>(bottleneckJson);
            NHCriteria nHCriteriabottleneck = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", bottleneckObj.RoleID);
            var bottleneckTemp = NHibernateQuerier.CriteriaSelect<Bottleneck>(nHCriteriabottleneck);
            Utility.Debug.LogInfo("yzqData传过来的瓶颈数据"+ bottleneckJson);
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, BottleneckData>>(out var bottleneckData);

            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, DemonData>>(out var demonData);

            bool isBreak = false;
            BottleneckData bottleneckDataTemp;
            //记录是否为大境界
            if (!bottleneckData.ContainsKey(bottleneckObj.RoleLevel + 1))
            {
                isBreak = true;
            }
            //判断是否有瓶颈
            float rootPercentNum = GetRootPercent(bottleneckData[bottleneckObj.RoleLevel], bottleneckObj.SpiritualRootVaule);
            Utility.Debug.LogInfo("yzqData得到的灵根概率" + rootPercentNum);
            if (GetPercent(rootPercentNum))
            {
                bottleneckTemp.IsBottleneck = true;
                if (isBreak)
                {
                    bottleneckTemp.IsThunder = true;
                    bottleneckTemp.ThunderRound = bottleneckData[bottleneckObj.RoleLevel].Thunder_Round;
                    int demonIndex = GetDemonPercent(demonData[bottleneckObj.RoleLevel], bottleneckObj.CraryVaule);
                    if (GetPercent(demonData[bottleneckObj.RoleLevel].Trigger_Chance[demonIndex] / 100))
                    {
                        bottleneckTemp.IsDemon = true;
                        bottleneckTemp.DemonID = demonData[bottleneckObj.RoleLevel].Demon_ID[demonIndex];
                    }
                }
                NHibernateQuerier.Update<Bottleneck>(bottleneckTemp);
                SetResponseParamters(() => {
                    subResponseParameters.Add((byte)ParameterCode.RoleBottleneck, Utility.Json.ToJson(bottleneckTemp));
                    operationResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
            {
                bottleneckTemp.RoleLevel = bottleneckObj.RoleLevel;
                NHibernateQuerier.Update<Bottleneck>(bottleneckTemp);
                SetResponseParamters(() => {
                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }

            return operationResponse;
        }

        //获取概率值
        private bool GetPercent(float num)
        {
            Random random = new Random();
            if (random.Next(1,10001)<= (num * 10000))
            {
                Utility.Debug.LogInfo("yzqData概率值为"+ (num * 10000) + "当前值为"+ random.Next(1, 10001));
                return true;
            }else
                return false;
        }

        /// <summary>
        /// 获取零灵根概率
        /// </summary>
        /// <param name="bottleneckData"></param>
        /// <param name="rootnum"></param>
        /// <returns></returns>
        private float GetRootPercent(BottleneckData bottleneckData,int rootnum)
        {
            float num;
            switch (rootnum)
            {
                case 1:
                    return num = bottleneckData.Spiritual_Root_1[0] / 100;
                    break;
                case 2:
                    return num = bottleneckData.Spiritual_Root_2[0] / 100;
                    break;
                case 3:
                    return num = bottleneckData.Spiritual_Root_3[0] / 100;
                    break;
                case 4:
                    return num = bottleneckData.Spiritual_Root_4[0] / 100;
                    break;
                case 5:
                    return num = bottleneckData.Spiritual_Root_5[0] / 100;
                    break;
                default:
                    return num = 1;
                    break;
            }
             
        }
        /// <summary>
        /// 获取心魔对应数组的下标
        /// </summary>
        /// <param name="demonData"></param>
        /// <param name="CraryVaule"></param>
        /// <returns></returns>
        private int GetDemonPercent(DemonData demonData, int CraryVaule)
        {
            int index=0;
            for (int i = 0; i < demonData.Crary_Value.Count; i++)
            {
                if (CraryVaule>=demonData.Crary_Value[i]&& CraryVaule<= demonData.Crary_Value[i+1])
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
    }
}
