using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using RedisDotNet;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
    public class GetPetStatusSubHandler : SyncPetStatusSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string petstatus = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.PetStatus));
            var rolepetObj = Utility.Json.ToObject<RolePet>(petstatus);
            NHCriteria nHCriteriarolepet = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolepetObj.RoleID);
            var petArray = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RolePet>(nHCriteriarolepet);
            if (petArray != null)
            {
                string petdict = petArray.PetIDDict;
                Dictionary<int, int> petIDList;
                List<PetStatus> petList = new List<PetStatus>();
                List<PetaPtitudeDTO> petaptitudeList = new List<PetaPtitudeDTO>();
                List<NHCriteria> nHCriteriasList = new List<NHCriteria>();
                if (petdict != null)
                {
                    petIDList = new Dictionary<int, int>();
                    petIDList = Utility.Json.ToObject<Dictionary<int, int>>(petdict);
                    foreach (var petid in petIDList)
                    {
                        NHCriteria nHCriteriapet = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("PetID", petid.Key);
                        var petstatusObj = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<PetStatus>(nHCriteriapet);
                        var petaptitudeObj = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<PetaPtitude>(nHCriteriapet);
                        PetaPtitudeDTO petaPtitudeDTO = new PetaPtitudeDTO() { AttackphysicalAptitude = petaptitudeObj.AttackphysicalAptitude, AttacksoulAptitude = petaptitudeObj.AttacksoulAptitude, AttackspeedAptitude = petaptitudeObj.AttackspeedAptitude, AttackpowerAptitude = petaptitudeObj.AttackpowerAptitude, DefendphysicalAptitude = petaptitudeObj.DefendphysicalAptitude, DefendpowerAptitude = petaptitudeObj.DefendpowerAptitude, HPAptitude = petaptitudeObj.HPAptitude, DefendsoulAptitude = petaptitudeObj.DefendsoulAptitude, MPAptitude = petaptitudeObj.MPAptitude, Petaptitudecol = petaptitudeObj.Petaptitudecol, PetaptitudeDrug = Utility.Json.ToObject<Dictionary<int, int>>(petaptitudeObj.PetaptitudeDrug), PetID = petaptitudeObj.PetID, SoulAptitude = petaptitudeObj.SoulAptitude };
                        petaptitudeList.Add(petaPtitudeDTO);
                        petList.Add(petstatusObj);
                        nHCriteriasList.Add(nHCriteriapet);
                    }
                }
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.PetStatus, Utility.Json.ToJson(petList));
                    SubDict.Add((byte)ParameterCode.PetPtitude, Utility.Json.ToJson(petaptitudeList));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
                ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriasList);
            }
            else
            {
                SetResponseData(() =>
                {
                    AscensionServer._Log.Info(">>>>>>>>>>>>>》》》》》》》》》》》》>>获得宠物数据失败");
                    SubDict.Add((byte)ParameterCode.PetStatus, Utility.Json.ToJson(new List<string>()));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriarolepet);
        }
    }
}
