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
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Add;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string roleJsonTmp = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Role));
            Role roleTmp = Utility.Json.ToObject<Role>(roleJsonTmp);
            NHCriteria nHCriteriaRoleName = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleName", roleTmp.RoleName);
            var isExisted = NHibernateQuerier.Verify<Role>(nHCriteriaRoleName);
            if (isExisted)
                Utility.Debug.LogInfo("----------------------------  Role >>Role name:+" + roleTmp.RoleName + " already exist !!!  ---------------------------------");
            Role role = NHibernateQuerier.CriteriaSelect<Role>(nHCriteriaRoleName);//根据username查询数据
           //TODO AddRoleSubHandler查询uuid未处理
            string userJsonTmp = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.User));
            User userTmp = Utility.Json.ToObject<User>(userJsonTmp);
            string str_uuid = userTmp.UUID;
            NHCriteria nHCriteriaUUID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("UUID", str_uuid);
            var userRole = NHibernateQuerier.CriteriaSelect<UserRole>(nHCriteriaUUID);
            string roleJson = userRole.RoleIDArray;
            string roleStatusJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleStatus));
            Utility.Debug.LogInfo("yzqData添加新角色" + userRole.UUID);
            //Dictionary<int, int> idRing = new Dictionary<int, int>();
            Dictionary<int, int> initialSchool = new Dictionary<int, int>();

            Ring ring = null;
            Utility.Debug.LogInfo("yzqData添加新角色2" );
            //如果没有查询到代表角色没被注册过可用
            if (role == null)
            {
                List<string> roleList = new List<string>();
                if (!string.IsNullOrEmpty(roleJson))
                    roleList = Utility.Json.ToObject<List<string>>(roleJson);
                //添加输入的用户进数据库
                role = roleTmp;
                var rolestatus = Utility.Json.ToObject<RoleStatus>(roleStatusJson);
                role = NHibernateQuerier.Insert<Role>(role);
                string roleId = role.RoleID.ToString();
                if (!string.IsNullOrEmpty(roleJson))
                    roleList.Add(roleId);
                else
                    roleList.Add(roleId);
                rolestatus.RoleID = int.Parse(roleId);
                NHibernateQuerier.Insert(rolestatus);
                NHibernateQuerier.Insert(new RoleAssets() { RoleID = rolestatus.RoleID });
                NHibernateQuerier.Insert(new OnOffLine() { RoleID = rolestatus.RoleID });
                NHibernateQuerier.Insert(new Bottleneck() { RoleID = rolestatus.RoleID });
                #region 任务
                roleTaskDic.Clear();
                roleTaskDic.Add("1001001", new RoleTaskItemDTO() { RoleTaskType = "DialogSystem", RoleTaskAchieveState = "NoAchieveTask", RoleTaskAcceptState = "NoAcceptAbleTask", RoleTaskAbandonState = "NoAbandonTask", RoleTaskKind = "MainTask" });
                NHibernateQuerier.Insert(new RoleTaskProgress() { RoleID = rolestatus.RoleID, RoleTaskInfoDic = Utility.Json.ToJson(roleTaskDic) });
                #endregion
                Dictionary<string, string> DOdict = new Dictionary<string, string>();
                #region 测试待修改
                RoleGFDict.Clear();
                //CultivationMethod gongFa = new CultivationMethod();
                //gongFa = ConcurrentSingleton<NHManager>.Instance.Insert(gongFa);
                //RoleGFDict.Add(gongFa.ID, gongFa.CultivationMethodID);
                NHibernateQuerier.Insert(new RoleGongFa() { RoleID = rolestatus.RoleID, GongFaIDArray = Utility.Json.ToJson(new Dictionary<string, string>()) });


                RoleMiShuDict.Clear();
                MiShu miShu = new MiShu();
                miShu = NHibernateQuerier.Insert(miShu);
                RoleMiShuDict.Add(miShu.ID, miShu.MiShuID);
                NHibernateQuerier.Insert(new RoleMiShu() { RoleID = rolestatus.RoleID, MiShuIDArray = Utility.Json.ToJson(RoleMiShuDict) });

                RolePetDict.Clear();
                //Pet pet = new Pet() {};
                //pet = ConcurrentSingleton<NHManager>.Instance.Insert(pet);
                //PetStatus petStatus = new PetStatus() { PetID = pet.ID };
                //ConcurrentSingleton<NHManager>.Instance.Insert(petStatus);
                //PetaPtitude petaPtitude=new PetaPtitude() { PetID = pet.ID,PetaptitudeDrug=Utility.Json.ToJson(new Dictionary<int, int>()) };
                //petaPtitude = ConcurrentSingleton<NHManager>.Instance.Insert(petaPtitude);
                //RolePetDict.Add(pet.ID, pet.PetID);
                NHibernateQuerier.Insert(new RolePet() { RoleID = rolestatus.RoleID, PetIDDict = "{}" });
                RolePurchaseRecord rolePurchaseRecord = new RolePurchaseRecord() { RoleID = rolestatus.RoleID ,GoodsPurchasedCount=Utility.Json.ToJson(new Dictionary<int, int>()) };
                NHibernateQuerier.Insert(rolePurchaseRecord);
                Weapon weapon = new Weapon() { RoleID= rolestatus.RoleID, Weaponindex = Utility.Json.ToJson(new Dictionary<int, int>()), WeaponStatusDict=Utility.Json.ToJson(new Dictionary<int, int>()) };
                NHibernateQuerier.Insert(weapon);
                #endregion
                #region 背包
                NHibernateQuerier.Insert(new RoleRing() { RoleID = rolestatus.RoleID });
                NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", rolestatus.RoleID);
                var ringArray = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteriaRoleID);
                if (ringArray.RingIdArray == 0)
                {
                    ringDict.Clear();
                    magicRingDict.Clear();
                    ringDict.Add(17701, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1, RingItemType =1 });
                    ringDict.Add(17711, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1, RingItemType = 2 });
                    ringDict.Add(17716, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1, RingItemType = 3 });
                    ringDict.Add(17721, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1, RingItemType = 4 });
                    ringDict.Add(17952, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1, RingItemType = 0 });
                    ringDict.Add(15001, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1, RingItemType = 0 });
                    ringDict.Add(15002, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1, RingItemType = 0 });
                    ringDict.Add(15003, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1, RingItemType = 0 });
                    ringDict.Add(15004, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1, RingItemType = 0 });
                    ringDict.Add(15005, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1, RingItemType = 0 });
                    ringDict.Add(15006, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1, RingItemType = 0 });
                    ringDict.Add(15007, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1, RingItemType = 0 });
                    ringDict.Add(15008, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1, RingItemType = 0 });
                    ringDict.Add(15009, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1, RingItemType = 0 });
                    ringDict.Add(15010, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1, RingItemType = 0 });
                    ringDict.Add(17001, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1, RingItemType = 0 });
                    ringDict.Add(17016, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1, RingItemType = 0 });
                    ringDict.Add(17006, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1, RingItemType = 0 });
                    //ringDict.Add(14301, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =20 });
                    //ringDict.Add(14302, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =20 });
                    //ringDict.Add(14303, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =20 });
                    //ringDict.Add(14304, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =20 });
                    //ringDict.Add(14305, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =20 });
                    //ringDict.Add(16001, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =50 });
                    //ringDict.Add(16002, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 50 });
                    //ringDict.Add(16003, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 50 });
                    //ringDict.Add(16004, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 50 });
                    //ringDict.Add(16005, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 50 });
                    //ringDict.Add(16006, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 50 });
                    //ringDict.Add(16007, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 50 });
                    //ringDict.Add(16008, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 50 });
                    //ringDict.Add(16009, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 50 });
                    //ringDict.Add(16010, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 50 });
                    //ringDict.Add(17955, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1 });
                    //ringDict.Add(17985, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1 });
                    //ringDict.Add(13021, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1 });
                    //ringDict.Add(13022, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1 });
                    //ringDict.Add(13023, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1 });
                    //ringDict.Add(13024, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1 });
                    //ringDict.Add(13025, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =1 });
                    //ringDict.Add(14101, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax =10 });
                    //ringDict.Add(14102, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 10 });
                    //ringDict.Add(14103, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 10 });
                    //ringDict.Add(14104, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 10 });
                    //ringDict.Add(14105, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 10 });
                    //ringDict.Add(14106, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 10 });
                    //ringDict.Add(14107, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 10 });
                    //ringDict.Add(14108, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 10 });
                    //ringDict.Add(14001, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 99 });
                    //ringDict.Add(14002, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 99 });
                    //ringDict.Add(14003, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 99 });
                    //ringDict.Add(14004, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 99 });
                    //ringDict.Add(14005, new RingItemsDTO() { RingItemAdorn = "0", RingItemCount = 1, RingItemTime = DateTime.Now.ToString("yyyyMMddHHmmss"), RingItemMax = 99 });
                    for (int i = 0; i < 6; i++)
                    { magicRingDict.Add(i, -1); }
                    ring = NHibernateQuerier.Insert<Ring>(new Ring() { RingId = 11110, RingItems = Utility.Json.ToJson(ringDict), RingMagicDictServer = Utility.Json.ToJson(magicRingDict), RingAdorn = Utility.Json.ToJson(new Dictionary<int, RingItemsDTO>()) });
                    //idRing.Add(ring.ID, 0);
                    NHibernateQuerier.Update<RoleRing>(new RoleRing() { RoleID = rolestatus.RoleID, RingIdArray = ring.ID });
                }

                #endregion
                #region 临时背包
                NHibernateQuerier.Insert(new TemporaryRing() { RoleID = rolestatus.RoleID ,  RingItems = Utility.Json.ToJson(new Dictionary<int,RingItemsDTO>())});
                #endregion
                #region 副职业
                NHibernateQuerier.Insert<Alchemy>(new Alchemy() { RoleID = rolestatus.RoleID, Recipe_Array = Utility.Json.ToJson(new List<int>()) });
                NHibernateQuerier.Insert<HerbsField>(new HerbsField() { RoleID = rolestatus.RoleID, jobLevel = 0, AllHerbs = Utility.Json.ToJson(new List<HerbFieldStatus>()) });
                NHibernateQuerier.Insert<Forge>(new Forge() { RoleID = rolestatus.RoleID, Recipe_Array = Utility.Json.ToJson(new List<int>()) });
                NHibernateQuerier.Insert<SpiritualRunes>(new SpiritualRunes() { RoleID = rolestatus.RoleID, Recipe_Array = Utility.Json.ToJson(new List<int>()) });
                NHibernateQuerier.Insert<Puppet>(new Puppet() { RoleID = rolestatus.RoleID, Recipe_Array = Utility.Json.ToJson(new List<int>()) });
                NHibernateQuerier.Insert<TacticFormation>(new TacticFormation() { RoleID = rolestatus.RoleID, Recipe_Array = Utility.Json.ToJson(new List<int>()) });

                Utility.Debug.LogInfo(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>添加副职业成功");
                #endregion
                #region 初始化门派
                Treasureattic treasureatti = new Treasureattic() { ItemAmountDict = Utility.Json.ToJson(new Dictionary<int, int>()), ItemRedeemedDict = Utility.Json.ToJson(new Dictionary<int, int>()) };
                treasureatti = NHibernateQuerier.Insert(treasureatti);
                SutrasAttic sutrasAttic = new SutrasAttic() { SutrasAmountDict = Utility.Json.ToJson(new Dictionary<int, int>()), SutrasRedeemedDictl = Utility.Json.ToJson(new Dictionary<int, int>()) };
                sutrasAttic = NHibernateQuerier.Insert(sutrasAttic);
                School school = new School();
                school.TreasureAtticID = treasureatti.ID;
                school.SutrasAtticID = sutrasAttic.ID;
                school = NHibernateQuerier.Insert(school);
                NHibernateQuerier.Insert(new RoleSchool() { RoleID = rolestatus.RoleID, RoleJoiningSchool = school.ID, RoleJoinedSchool = 0 });
                #endregion
                #region 仙盟
                RoleAlliance roleAlliance = new RoleAlliance() { RoleID = rolestatus.RoleID, RoleName = role.RoleName,ApplyForAlliance=Utility.Json.ToJson(new List<int>()) ,RoleSchool=900};
                NHibernateQuerier.Insert(roleAlliance);
                #endregion
                var userRoleJson = Utility.Json.ToJson(roleList);
                NHibernateQuerier.Update(new UserRole() { RoleIDArray = userRoleJson, UUID = str_uuid });
                operationResponse.ReturnCode = (short)ReturnCode.Success;
                RoleAllianceDTO roleAllianceDTO = new RoleAllianceDTO() { RoleID = rolestatus.RoleID, RoleName = role.RoleName, ApplyForAlliance = new List<int>() };
                DOdict.Add("Role", Utility.Json.ToJson(role));
                DOdict.Add("RoleStatus", Utility.Json.ToJson(rolestatus));
                //DOdict.Add("GongFa", Utility.Json.ToJson(gongFa));
                DOdict.Add("School", Utility.Json.ToJson(school));
                DOdict.Add("MiShu", Utility.Json.ToJson(miShu));
                DOdict.Add("RoleAlliance", Utility.Json.ToJson(roleAllianceDTO));
                SetResponseParamters(() => {
                    subResponseParameters.Add((byte)ParameterCode.Role, Utility.Json.ToJson(DOdict));
                    operationResponse.Parameters = subResponseParameters;
                });

            }
            else
            {
                SetResponseParamters(() => {
                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
                });

            }
            //把上面的回应给客户端
            GameManager.ReferencePoolManager.Despawns(nHCriteriaUUID, nHCriteriaRoleName);
            return operationResponse;
        }
    }
}
