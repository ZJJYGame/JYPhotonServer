//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Photon.SocketServer;
//using AscensionProtocol;
//using AscensionProtocol.DTO;
//using AscensionServer.Model;
//using Cosmos;
//using RedisDotNet;
//namespace AscensionServer
//{
//    public class AddGongFaSubHandler : SyncGongFaSubHandler
//    {
//        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Add;
//        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
//        {
//            var dict = operationRequest.Parameters;
//            string gongfaJson = Convert.ToString(Utility.GetValue(dict,(byte)ParameterCode.GongFa));
//            string  roleJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Role));

//            var roleObj = Utility.Json.ToObject<RoleDTO>(roleJson);
//            var gongfaObj = Utility.Json.ToObject<CultivationMethodDTO>(gongfaJson);

//            NHCriteria nHCriteriaRoleID = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
//            var roleGongfatemp = NHibernateQuerier.CriteriaSelect<RoleGongFa>(nHCriteriaRoleID);

//            var roleTemp = NHibernateQuerier.CriteriaSelect<Role>(nHCriteriaRoleID);

//            Dictionary<string, string> DTOdict = new Dictionary<string, string>();

//            #region 背包验证逻辑
//            var ringObj = CosmosEntry.ReferencePoolManager.Spawn<RingDTO>();
//            ringObj.RingItems = new Dictionary<int, RingItemsDTO>();
//            ringObj.RingItems.Add(gongfaObj.CultivationMethodID, new RingItemsDTO());
//            var ringServer = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteriaRoleID);
//           var nHCriteriaRingID = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", ringServer.RingIdArray);
//            #endregion

//            if (InventoryManager.VerifyIsExist(gongfaObj.CultivationMethodID, nHCriteriaRingID))
//            {
//                var gongfaTemp = CosmosEntry.ReferencePoolManager.Spawn<CultivationMethodDTO>();

//             var result= GongFaStudyManager.AddGongFaJudge(gongfaObj.CultivationMethodID, roleObj,out gongfaTemp);

//                if (!result)
//                {
//                    Utility.Debug.LogInfo("1添加的新的功法为yzqData" + gongfaJson);
//                    SetResponseParamters(() =>
//                    {
//                        operationResponse.ReturnCode = (short)ReturnCode.Fail;
//                    });
//                }
//                else
//                {
//                    Utility.Debug.LogInfo("2添加的新的功法为yzqData" + gongfaJson);
//                    if (roleGongfatemp!=null)
//                    {
//                        Utility.Debug.LogInfo("3添加的新的功法为yzqData" + gongfaJson);
//                        var rolegongfaDict = Utility.Json.ToObject<Dictionary<int, int>>(roleGongfatemp.GongFaIDArray);
//                        if (rolegongfaDict.Count==0)
//                        {
//                            Utility.Debug.LogInfo("4添加的新的功法为yzqData" + gongfaJson);
//                            CultivationMethod cultivationMethod = new CultivationMethod() { CultivationMethodID = gongfaTemp.CultivationMethodID, CultivationMethodLevel = 1, CultivationMethodLevelSkillArray = Utility.Json.ToJson(gongfaTemp.CultivationMethodLevelSkillArray) };
//                            cultivationMethod = NHibernateQuerier.Insert(cultivationMethod);
//                            rolegongfaDict.Add(cultivationMethod.ID, cultivationMethod.CultivationMethodID);
//                            roleGongfatemp.GongFaIDArray = Utility.Json.ToJson(rolegongfaDict);
//                            NHibernateQuerier.Update(roleGongfatemp);

//                            RedisHelper.Hash.HashSet<CultivationMethodDTO>(RedisKeyDefine._GongfaPerfix+ roleObj.RoleID, roleObj.RoleID.ToString(), gongfaTemp);

//                            var roleGongfaObj= CosmosEntry.ReferencePoolManager.Spawn<RoleGongFaDTO>();
//                            roleGongfaObj.RoleID = roleGongfatemp.RoleID;
//                            roleGongfaObj.GongFaIDDict = rolegongfaDict;
//                            RedisHelper.Hash.HashSet<RoleGongFaDTO>(RedisKeyDefine._RoleGongfaPerfix + roleObj.RoleID, roleObj.RoleID.ToString(), roleGongfaObj);


//                            DTOdict.Add("Role",Utility.Json.ToJson(roleTemp));
//                            DTOdict.Add("CultivationMethod", Utility.Json.ToJson(gongfaTemp));
//                            #region 第一本功法指令发送
//                            roleTemp.RoleLevel = 1;
//                            NHibernateQuerier.Update(roleTemp);

//                            OperationData operationData = new OperationData();
//                            operationData.DataMessage = Utility.Json.ToJson(DTOdict);
//                            operationData.OperationCode = (byte)OperationCode.AddFirstGongfa;
//                            GameEntry.RoleManager.SendMessage(roleObj.RoleID, operationData);
//                            InventoryManager.RemoveCmdS2C(roleObj.RoleID, ringObj, nHCriteriaRingID);
//                            #endregion
//                        }
//                        else
//                        {
//                            Utility.Debug.LogInfo("2添加的新的功法为" + gongfaJson);
//                            if (!rolegongfaDict.ContainsKey(gongfaTemp.CultivationMethodID))
//                            {
//                                CultivationMethod cultivationMethod = new CultivationMethod() { CultivationMethodID = gongfaTemp.CultivationMethodID, CultivationMethodLevel = gongfaTemp.CultivationMethodLevel, CultivationMethodLevelSkillArray = Utility.Json.ToJson(gongfaTemp.CultivationMethodLevelSkillArray) };
//                                cultivationMethod = NHibernateQuerier.Insert(cultivationMethod);
//                                rolegongfaDict.Add(cultivationMethod.ID, cultivationMethod.CultivationMethodID);
//                                roleGongfatemp.GongFaIDArray = Utility.Json.ToJson(rolegongfaDict);
//                                NHibernateQuerier.Update(roleGongfatemp);

//                                RedisHelper.Hash.HashSet<CultivationMethodDTO>(RedisKeyDefine._GongfaPerfix + roleObj.RoleID, roleObj.RoleID.ToString(), gongfaTemp);
//                                var roleGongfaObj = CosmosEntry.ReferencePoolManager.Spawn<RoleGongFaDTO>();
//                                roleGongfaObj.RoleID = roleGongfatemp.RoleID;
//                                roleGongfaObj.GongFaIDArray = rolegongfaDict;
//                                RedisHelper.Hash.HashSet<RoleGongFaDTO>(RedisKeyDefine._RoleGongfaPerfix + roleObj.RoleID, roleObj.RoleID.ToString(), roleGongfaObj);


//                                DTOdict.Add("Role", Utility.Json.ToJson(roleTemp));
//                                DTOdict.Add("CultivationMethod", Utility.Json.ToJson(gongfaTemp));
//                                #region 第一本功法指令发送
//                                roleTemp.RoleLevel = gongfaTemp.CultivationMethodLevel;
//                                NHibernateQuerier.Update(roleTemp);

//                                OperationData operationData = new OperationData();
//                                operationData.DataMessage = Utility.Json.ToJson(DTOdict);
//                                operationData.OperationCode = (byte)OperationCode.SyncGongFa;
//                                GameEntry.RoleManager.SendMessage(roleObj.RoleID, operationData);
//                                InventoryManager.RemoveCmdS2C(roleObj.RoleID, ringObj, nHCriteriaRingID);
//                                #endregion
//                            }
//                            else
//                            {
//                                SetResponseParamters(() =>
//                                {
//                                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
//                                });
//                            }
//                        }
//                    }
//                }
//            }
//            else
//            {
//                SetResponseParamters(() =>
//                {
//                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
//                });
//            }



//            //Dictionary<int, int> gongfaDict;
//            //Dictionary<int, string> DOdict=new Dictionary<int, string>();

//            #region 待删
//            //if (roleGongFaObj != null)
//            //{
//            //    gongfaDict = Utility.Json.ToObject<Dictionary<int, int>>(roleGongFaObj.GongFaIDArray);
//            //    if (gongfaDict.Count > 0)
//            //    {
//            //        if (gongfaDict.Values.ToList().Contains(gongfaObj.CultivationMethodID))
//            //        {
//            //            operationResponse.ReturnCode = (short)ReturnCode.Fail;
//            //            subResponseParameters.Add((byte)ParameterCode.RoleGongFa, null);
//            //        }
//            //        else
//            //        {
//            //            CultivationMethod cultivationMethod = new CultivationMethod() { CultivationMethodID = gongfaObj.CultivationMethodID, CultivationMethodLevel = gongfaObj.CultivationMethodLevel };
//            //            cultivationMethod = NHibernateQuerier.Insert(cultivationMethod);
//            //            gongfaDict.Add(cultivationMethod.ID, cultivationMethod.CultivationMethodID);
//            //            roleGongFaObj.GongFaIDArray = Utility.Json.ToJson(gongfaDict);
//            //            NHibernateQuerier.Update(roleGongFaObj);
//            //            DOdict.Add(0, Utility.Json.ToJson(cultivationMethod));
//            //            DOdict.Add(1, Utility.Json.ToJson(roleGongFaObj));
//            //            #region Redis模块
//            //            RedisHelper.Hash.HashSet<CultivationMethod>(RedisKeyDefine._GongfaPerfix, cultivationMethod.CultivationMethodID.ToString(), cultivationMethod);
//            //            RedisHelper.Hash.HashSet<RoleGongFa>(RedisKeyDefine._RoleGongfaPerfix, roleObj.RoleID.ToString(), roleGongFaObj);
//            //            #endregion
//            //            SetResponseParamters(() =>
//            //            {
//            //                subResponseParameters.Add((byte)ParameterCode.RoleGongFa, Utility.Json.ToJson(DOdict));
//            //                operationResponse.ReturnCode = (short)ReturnCode.Success;
//            //            });
//            //        }
//            //    }
//            //    else
//            //    {
//            //        CultivationMethod cultivationMethod = new CultivationMethod() { CultivationMethodID = gongfaObj.CultivationMethodID, CultivationMethodLevel = gongfaObj.CultivationMethodLevel };
//            //        cultivationMethod = NHibernateQuerier.Insert(cultivationMethod);
//            //        gongfaDict.Add(cultivationMethod.ID, cultivationMethod.CultivationMethodID);
//            //        roleGongFaObj.GongFaIDArray = Utility.Json.ToJson(gongfaDict);
//            //        NHibernateQuerier.Update(roleGongFaObj);
//            //        roletemp.RoleLevel = 1;
//            //        DOdict.Add(0, Utility.Json.ToJson(cultivationMethod));
//            //        DOdict.Add(1, Utility.Json.ToJson(new RoleDTO() { RoleID = roletemp.RoleID, RoleFaction = roletemp.RoleFaction, RoleGender = roletemp.RoleGender, RoleLevel = roletemp.RoleLevel, RoleName = roletemp.RoleName, RoleRoot = roletemp.RoleRoot, RoleTalent = roletemp.RoleTalent }));
//            //        NHibernateQuerier.Update<Role>(roletemp);
//            //        OperationData operationData = new OperationData();
//            //        operationData.DataMessage = Utility.Json.ToJson(DOdict);
//            //        operationData.OperationCode = (byte)OperationCode.AddFirstGongfa;
//            //        GameEntry. RoleManager.SendMessage(roleObj.RoleID, operationData);
//            #region Redis模块
//            //        RedisHelper.Hash.HashSet<CultivationMethod>(RedisKeyDefine._RoleGongfaPerfix, cultivationMethod.CultivationMethodID.ToString(), cultivationMethod);
//            //        RedisHelper.Hash.HashSet<RoleGongFa>(RedisKeyDefine._GongfaPerfix, roleObj.RoleID.ToString(), roleGongFaObj);

//            //        CosmosEntry.ReferencePoolManager.Despawns(ringObj);
//            #endregion
//            //    }
//            //}
//            //CosmosEntry.ReferencePoolManager.Despawns(nHCriteriaRoleID);
//            #endregion
//            return operationResponse;
//        }
//    }
//}


