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
using RedisDotNet;
namespace AscensionServer
{
    public class AddMiShuSubHandler : SyncMiShuSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Add;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string msJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.MiShu));
            string roleJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Role));

            var roleObj = Utility.Json.ToObject<Role>(roleJson);
            var mishuObj = Utility.Json.ToObject<MiShu>(msJson);
            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
            var roleMiShuObj = NHibernateQuerier.CriteriaSelect<RoleMiShu>(nHCriteriaRoleID);
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, MiShuData>>(out var mishuDataDict);

            Dictionary<int, int> mishuDict;
            Dictionary<int, string> DOdict = new Dictionary<int, string>();
            if (roleMiShuObj!=null)
            {
                if (!string.IsNullOrEmpty(roleMiShuObj.MiShuIDArray))
                {
                    mishuDict = Utility.Json.ToObject<Dictionary<int, int>>(roleMiShuObj.MiShuIDArray);
                    if (mishuDict.Values.ToList().Contains(mishuObj.MiShuID))
                    {
 
                        operationResponse.ReturnCode = (short)ReturnCode.Fail;
                        subResponseParameters.Add((byte)ParameterCode.RoleMiShu, null);
                    }
                    else
                    {
                        MiShu miShu = new MiShu() { MiShuID= mishuObj .MiShuID};
                        miShu= NHibernateQuerier.Insert(miShu);
                        mishuDict.Add(miShu.ID, miShu.MiShuID);
                        roleMiShuObj.MiShuIDArray = Utility.Json.ToJson(mishuDict);
                        NHibernateQuerier.Update(roleMiShuObj);
                        DOdict.Add(0, Utility.Json.ToJson(miShu));
                        DOdict.Add(1, Utility.Json.ToJson(roleMiShuObj));
                        #region Redis模块
                        RedisHelper.Hash.HashSet<MiShu>(RedisKeyDefine._MiShuPerfix, miShu.ID.ToString(), miShu);
                        RedisHelper.Hash.HashSet<RoleMiShu>(RedisKeyDefine._RoleMiShuPerfix, roleObj.RoleID.ToString(), roleMiShuObj);
                        #endregion
                        SetResponseParamters(() =>
                        {
                            subResponseParameters.Add((byte)ParameterCode.RoleMiShu, Utility.Json.ToJson(DOdict));
                            operationResponse.ReturnCode = (short)ReturnCode.Success;
                        });
                    }
                }
            }
            GameManager.ReferencePoolManager.Despawns(nHCriteriaRoleID);
            return operationResponse;
        }

    }
}
