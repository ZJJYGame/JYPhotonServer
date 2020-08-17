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

namespace AscensionServer
{
    /// <summary>
    /// 添加新角色子操作码
    /// </summary>
    public class AddRoleSubHandler : SyncRoleSubHandler
    {
        Dictionary<int, int> RoleGFDict = new Dictionary<int, int>();
        Dictionary<int, int> RoleMiShuDict = new Dictionary<int, int>();
        Dictionary<int, int> RolePetDict = new Dictionary<int, int>();
        Dictionary<string, RoleTaskItemDTO> roleTaskDic = new Dictionary<string, RoleTaskItemDTO>();
        Dictionary<int, RingItemsDTO> ringDict = new Dictionary<int, RingItemsDTO>();
        Dictionary<int, int> magicRingDict = new Dictionary<int, int>();
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string roleJsonTmp = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Role));
            Role roleTmp = Utility.Json.ToObject<Role>(roleJsonTmp);
            NHCriteria nHCriteriaRoleName = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleName", roleTmp.RoleName);
            var isExisted = ConcurrentSingleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleName);
            if (isExisted)
                AscensionServer._Log.Info("----------------------------  Role >>Role name:+" + roleTmp.RoleName + " already exist !!!  ---------------------------------");
            Role role = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Role>(nHCriteriaRoleName);//根据username查询数据
            string str_uuid = peer.PeerCache.UUID;
            NHCriteria nHCriteriaUUID = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("UUID", str_uuid);
            var userRole = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<UserRole>(nHCriteriaUUID);
            string roleJson = userRole.RoleIDArray;
            string roleStatusJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleStatus));
            Dictionary<int, int> idRing = new Dictionary<int, int>();
            Dictionary<int, int> initialSchool = new Dictionary<int, int>();
            Ring ring = null;
            //如果没有查询到代表角色没被注册过可用

            if (role == null)
            {
                List<string> roleList = new List<string>();
                if (!string.IsNullOrEmpty(roleJson))
                    roleList = Utility.Json.ToObject<List<string>>(roleJson);
                //添加输入的用户进数据库
                role = roleTmp;
                var rolestatus = Utility.Json.ToObject<RoleStatus>(roleStatusJson);
                role = ConcurrentSingleton<NHManager>.Instance.Insert<Role>(role);
                string roleId = role.RoleID.ToString();
                if (!string.IsNullOrEmpty(roleJson))
                    roleList.Add(roleId);
                else
                    roleList.Add(roleId);
                rolestatus.RoleID = int.Parse(roleId);
                ConcurrentSingleton<NHManager>.Instance.Insert(rolestatus);
                ConcurrentSingleton<NHManager>.Instance.Insert(new RoleAssets() { RoleID = rolestatus.RoleID });
                ConcurrentSingleton<NHManager>.Instance.Insert(new OnOffLine() { RoleID = rolestatus.RoleID });
                #region 任务
                roleTaskDic.Clear();
                roleTaskDic.Add("1001001", new RoleTaskItemDTO() { RoleTaskType = "DialogSystem", RoleTaskAchieveState = "NoAchieveTask", RoleTaskAcceptState = "NoAcceptAbleTask", RoleTaskAbandonState = "NoAbandonTask", RoleTaskKind = "MainTask" });
                ConcurrentSingleton<NHManager>.Instance.Insert(new RoleTaskProgress() { RoleID = rolestatus.RoleID, RoleTaskInfoDic = Utility.Json.ToJson(roleTaskDic) });
                #endregion
                Dictionary<string, string> DOdict = new Dictionary<string, string>();
                #region 测试待修改
                RoleGFDict.Clear();
                CultivationMethod gongFa = new CultivationMethod();
                gongFa = ConcurrentSingleton<NHManager>.Instance.Insert(gongFa);
                RoleGFDict.Add(gongFa.ID, gongFa.CultivationMethodID);
                ConcurrentSingleton<NHManager>.Instance.Insert(new RoleGongFa() { RoleID = rolestatus.RoleID, GongFaIDArray = Utility.Json.ToJson(RoleGFDict) });


                RoleMiShuDict.Clear();
                MiShu miShu = new MiShu();
                miShu = ConcurrentSingleton<NHManager>.Instance.Insert(miShu);
                RoleMiShuDict.Add(miShu.ID, miShu.MiShuID);
                ConcurrentSingleton<NHManager>.Instance.Insert(new RoleMiShu() { RoleID = rolestatus.RoleID, MiShuIDArray = Utility.Json.ToJson(RoleMiShuDict) });

                RolePetDict.Clear();
                Pet pet = new Pet() {};
                pet = ConcurrentSingleton<NHManager>.Instance.Insert(pet);
                PetStatus petStatus = new PetStatus() { PetID = pet.ID };
                ConcurrentSingleton<NHManager>.Instance.Insert(petStatus);
                PetaPtitude petaPtitude=new PetaPtitude() { PetID = pet.ID,PetaptitudeDrug=Utility.Json.ToJson(new Dictionary<int, int>()) };
                petaPtitude = ConcurrentSingleton<NHManager>.Instance.Insert(petaPtitude);
                RolePetDict.Add(pet.ID, pet.PetID);
                ConcurrentSingleton<NHManager>.Instance.Insert(new RolePet() { RoleID = rolestatus.RoleID, PetIDDict = Utility.Json.ToJson(RolePetDict) });
                RolePurchaseRecord rolePurchaseRecord = new RolePurchaseRecord() { RoleID = rolestatus.RoleID ,GoodsPurchasedCount=Utility.Json.ToJson(new Dictionary<int, int>()) };
                ConcurrentSingleton<NHManager>.Instance.Insert(rolePurchaseRecord);
                Weapon weapon = new Weapon() { RoleID= rolestatus.RoleID, Weaponindex = Utility.Json.ToJson(new Dictionary<int, int>()), WeaponStatusDict=Utility.Json.ToJson(new Dictionary<int, int>()) };
                ConcurrentSingleton<NHManager>.Instance.Insert(weapon);
                #endregion
                #region 背包
                ConcurrentSingleton<NHManager>.Instance.Insert(new RoleRing() { RoleID = rolestatus.RoleID });
                NHCriteria nHCriteriaRoleID = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolestatus.RoleID);
                var ringArray = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RoleRing>(nHCriteriaRoleID);
                if (string.IsNullOrEmpty(ringArray.RingIdArray))
                {
                    ringDict.Clear();
                    magicRingDict.Clear();
                    ringDict.Add(17701, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(17711, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(17716, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(17721, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(17952, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(15001, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(15002, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(15003, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(15004, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(15005, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(15006, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(15007, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(15008, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(15009, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(15010, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(17001, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(17016, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(17006, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(14301, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(14302, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(14303, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(14304, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(14305, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(16001, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(16002, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(16003, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(16004, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(16005, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(16006, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(16007, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(16008, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(16009, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(16010, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(17955, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(17985, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(13021, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(13022, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(13023, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(13024, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(13025, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(14101, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(14102, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(14103, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(14104, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(14105, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(14106, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(14107, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(14108, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(14001, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(14002, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(14003, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(14004, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    ringDict.Add(14005, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss") });
                    for (int i = 0; i < 6; i++)
                    { magicRingDict.Add(i, -1); }
                    ring = ConcurrentSingleton<NHManager>.Instance.Insert<Ring>(new Ring() { RingId = 11110, RingItems = Utility.Json.ToJson(ringDict), RingMagicDictServer = Utility.Json.ToJson(magicRingDict) });
                    idRing.Add(ring.ID, ring.RingAdorn);

                    ConcurrentSingleton<NHManager>.Instance.Update<RoleRing>(new RoleRing() { RoleID = rolestatus.RoleID, RingIdArray = Utility.Json.ToJson(idRing) });
                }

                #endregion
                #region 临时背包
                ConcurrentSingleton<NHManager>.Instance.Insert(new TemporaryRing() { RoleID = rolestatus.RoleID ,  RingItems = Utility.Json.ToJson(new Dictionary<int,RingItemsDTO>())});
                #endregion
                #region 副职业
                ConcurrentSingleton<NHManager>.Instance.Insert<Alchemy>(new Alchemy() { RoleID = rolestatus.RoleID, Recipe_Array = Utility.Json.ToJson(new List<int>()) });
                ConcurrentSingleton<NHManager>.Instance.Insert<HerbsField>(new HerbsField() { RoleID = rolestatus.RoleID, jobLevel = 0, AllHerbs = Utility.Json.ToJson(new List<HerbFieldStatus>()) });
                ConcurrentSingleton<NHManager>.Instance.Insert<Forge>(new Forge() { RoleID = rolestatus.RoleID, Recipe_Array = Utility.Json.ToJson(new List<int>()) });
                ConcurrentSingleton<NHManager>.Instance.Insert<SpiritualRunes>(new SpiritualRunes() { RoleID = rolestatus.RoleID, Recipe_Array = Utility.Json.ToJson(new List<int>()) });
                ConcurrentSingleton<NHManager>.Instance.Insert<Puppet>(new Puppet() { RoleID = rolestatus.RoleID, Recipe_Array = Utility.Json.ToJson(new List<int>()) });
                ConcurrentSingleton<NHManager>.Instance.Insert<TacticFormation>(new TacticFormation() { RoleID = rolestatus.RoleID, Recipe_Array = Utility.Json.ToJson(new List<int>()) });

                AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>添加副职业成功");
                #endregion
                #region 初始化门派
                Treasureattic treasureatti = new Treasureattic() { ItemAmountDict = Utility.Json.ToJson(new Dictionary<int, int>()), ItemRedeemedDict = Utility.Json.ToJson(new Dictionary<int, int>()) };
                treasureatti = ConcurrentSingleton<NHManager>.Instance.Insert(treasureatti);
                SutrasAttic sutrasAttic = new SutrasAttic() { SutrasAmountDict = Utility.Json.ToJson(new Dictionary<int, int>()), SutrasRedeemedDictl = Utility.Json.ToJson(new Dictionary<int, int>()) };
                sutrasAttic = ConcurrentSingleton<NHManager>.Instance.Insert(sutrasAttic);
                School school = new School();
                school.TreasureAtticID = treasureatti.ID;
                school.SutrasAtticID = sutrasAttic.ID;
                school = ConcurrentSingleton<NHManager>.Instance.Insert(school);
                ConcurrentSingleton<NHManager>.Instance.Insert(new RoleSchool() { RoleID = rolestatus.RoleID, RoleJoiningSchool = school.ID, RoleJoinedSchool = 0 });
                #endregion

                var userRoleJson = Utility.Json.ToJson(roleList);
                ConcurrentSingleton<NHManager>.Instance.Update(new UserRole() { RoleIDArray = userRoleJson, UUID = str_uuid });
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                DOdict.Add("Role", Utility.Json.ToJson(role));
                DOdict.Add("RoleStatus", Utility.Json.ToJson(rolestatus));
                DOdict.Add("GongFa", Utility.Json.ToJson(gongFa));
                DOdict.Add("School", Utility.Json.ToJson(school));
                DOdict.Add("MiShu", Utility.Json.ToJson(miShu));
                Owner.ResponseData.Add((byte)ParameterCode.Role, Utility.Json.ToJson(DOdict));
                Owner.OpResponse.Parameters = Owner.ResponseData;
            }
            else
            {
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            //把上面的回应给客户端
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaUUID, nHCriteriaRoleName);
        }


    }
}
