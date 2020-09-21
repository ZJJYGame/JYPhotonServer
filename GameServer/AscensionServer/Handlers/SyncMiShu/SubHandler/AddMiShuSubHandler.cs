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
    public class AddMiShuSubHandler : SyncMiShuSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string msJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.MiShu));
            string roleJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Role));

            var roleObj = Utility.Json.ToObject<Role>(roleJson);
            var mishuObj = Utility.Json.ToObject<MiShu>(msJson);
            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
            var roleMiShuObj = NHibernateQuerier.CriteriaSelect<RoleMiShu>(nHCriteriaRoleID);
            Dictionary<int, int> mishuDict;
            Dictionary<int, string> DOdict = new Dictionary<int, string>();
            if (roleMiShuObj!=null)
            {
                Utility.Debug.LogInfo("添加的学习的秘术为" + msJson);
                if (!string.IsNullOrEmpty(roleMiShuObj.MiShuIDArray))
                {
                    mishuDict = Utility.Json.ToObject<Dictionary<int, int>>(roleMiShuObj.MiShuIDArray);
                    if (mishuDict.Values.ToList().Contains(mishuObj.MiShuID))
                    {
                        Utility.Debug.LogInfo("人物已经学会的秘术" + roleMiShuObj.MiShuIDArray);
                        Utility.Debug.LogInfo("人物已经学会此秘术无法添加新的功法" + roleMiShuObj.MiShuIDArray);
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;

                        Owner.ResponseData.Add((byte)ParameterCode.RoleMiShu, null);
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
                        SetResponseData(() =>
                        {
                            Utility.Debug.LogInfo("添加的学习的秘术为" + Utility.Json.ToJson(DOdict));
                            SubDict.Add((byte)ParameterCode.RoleMiShu, Utility.Json.ToJson(DOdict));
                            Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                        });
                    }
                }
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaRoleID);

        }

    }
}
