﻿using System;
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
        Dictionary<int, RoleTaskItemDTO> roleTaskDic = new Dictionary<int, RoleTaskItemDTO>();
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
            NHCriteria nHCriteriaRoleName = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleName", roleTmp.RoleName);
            var isExisted = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleName);
            if (isExisted)
                AscensionServer._Log.Info("----------------------------  Role >>Role name:+" + roleTmp.RoleName + " already exist !!!  ---------------------------------");
            Role role = Singleton<NHManager>.Instance.CriteriaSelect<Role>(nHCriteriaRoleName);//根据username查询数据
            string str_uuid = peer.PeerCache.UUID;
            NHCriteria nHCriteriaUUID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("UUID", str_uuid);
            var userRole = Singleton<NHManager>.Instance.CriteriaSelect<UserRole>(nHCriteriaUUID);
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
                role = Singleton<NHManager>.Instance.Insert<Role>(role);
                string roleId = role.RoleID.ToString();
                if (!string.IsNullOrEmpty(roleJson))
                    roleList.Add(roleId);
                else
                    roleList.Add(roleId);
                rolestatus.RoleID = int.Parse(roleId);
                Singleton<NHManager>.Instance.Insert(rolestatus);
                Singleton<NHManager>.Instance.Insert(new RoleAssets() { RoleID = rolestatus.RoleID });
                Singleton<NHManager>.Instance.Insert(new OnOffLine() { RoleID = rolestatus.RoleID });
                #region 任务
                roleTaskDic.Clear();
                roleTaskDic.Add(0001, new RoleTaskItemDTO() { RoleTaskType = "DialogSystem", RoleTaskAchieveState = "NoAchieveTask", RoleTaskAcceptState = "NoAcceptAbleTask", RoleTaskAbandonState = "NoAbandonTask" , RoleTaskKind = "MainTask" });
                Singleton<NHManager>.Instance.Insert(new RoleTaskProgress() { RoleID = rolestatus.RoleID, RoleTaskInfoDic = Utility.Json.ToJson(roleTaskDic)});
                #endregion
                Dictionary<string, string> DOdict = new Dictionary<string, string>();
                #region 测试待修改
                RoleGFDict.Clear();
                CultivationMethod gongFa = new CultivationMethod();
                gongFa = Singleton<NHManager>.Instance.Insert(gongFa);
                RoleGFDict.Add(gongFa.ID, gongFa.CultivationMethodID);
                Singleton<NHManager>.Instance.Insert(new RoleGongFa() { RoleID = rolestatus.RoleID, GongFaIDArray = Utility.Json.ToJson(RoleGFDict) });


                RoleMiShuDict.Clear();
                MiShu miShu = new MiShu();
                miShu = Singleton<NHManager>.Instance.Insert(miShu);
                RoleMiShuDict.Add(miShu.ID, miShu.MiShuID);
                Singleton<NHManager>.Instance.Insert(new RoleMiShu() { RoleID = rolestatus.RoleID, MiShuIDArray = Utility.Json.ToJson(RoleMiShuDict) });

                RolePetDict.Clear();
                Pet pet = new Pet();
                pet = Singleton<NHManager>.Instance.Insert(pet);
                PetStatus petStatus = new PetStatus() { PetID = pet.ID };
                petStatus = Singleton<NHManager>.Instance.Insert(petStatus);
                RolePetDict.Add(pet.ID, pet.PetID);
                Singleton<NHManager>.Instance.Insert(new RolePet() { RoleID = rolestatus.RoleID, PetIDDict = Utility.Json.ToJson(RolePetDict) });
                #endregion
                #region 背包
                Singleton<NHManager>.Instance.Insert(new RoleRing() { RoleID = rolestatus.RoleID });
                NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolestatus.RoleID);
                var ringArray = Singleton<NHManager>.Instance.CriteriaSelect<RoleRing>(nHCriteriaRoleID);
                if (string.IsNullOrEmpty(ringArray.RingIdArray))
               {
                   ring = Singleton<NHManager>.Instance.Insert<Ring>(new Ring() { RingId = 11110, RingItems = Utility.Json.ToJson(new Dictionary<int, RingItemsDTO>()) });
                   idRing.Add(ring.ID, ring.RingAdorn);
                   Singleton<NHManager>.Instance.Update<RoleRing>(new RoleRing() { RoleID = rolestatus.RoleID, RingIdArray = Utility.Json.ToJson(idRing) });
               }

                #endregion
                #region 副职业
                Singleton<NHManager>.Instance.Insert<Alchemy>(new Alchemy() { RoleID= rolestatus.RoleID});
                AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>添加副职业成功");
                #endregion
                #region 初始化门派
                initialSchool.Clear();
                Treasureattic treasureatti = new Treasureattic();
                treasureatti = Singleton<NHManager>.Instance.Insert(treasureatti);
                School school = new School();
                school.TreasureAtticID = treasureatti.ID;
                school = Singleton<NHManager>.Instance.Insert(school);
                initialSchool.Add(school.ID, school.SchoolID);
                Singleton<NHManager>.Instance.Insert(new RoleSchool() { RoleID = rolestatus.RoleID, RoleJoiningSchool = Utility.Json.ToJson(initialSchool), RoleJoinedSchool = Utility.Json.ToJson("") });
                #endregion

                var userRoleJson = Utility.Json.ToJson(roleList);
                Singleton<NHManager>.Instance.Update(new UserRole() { RoleIDArray = userRoleJson, UUID = str_uuid });
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                DOdict.Add("Role", Utility.Json.ToJson(role));
                DOdict.Add("RoleStatus", Utility.Json.ToJson(rolestatus));
                DOdict.Add("GongFa", Utility.Json.ToJson(gongFa));
                DOdict.Add("School", Utility.Json.ToJson(school));
                Owner.ResponseData.Add((byte)ParameterCode.Role, Utility.Json.ToJson(DOdict));
                Owner.OpResponse.Parameters = Owner.ResponseData;
            }
            else
            {
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            //把上面的回应给客户端
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaUUID, nHCriteriaRoleName);
        }


    }
}
