using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Cosmos;
using Protocol;
using RedisDotNet;
using UnityEngine;
namespace AscensionServer
{
    [ImplementProvider]
    public class MapResourceHelper : IMapResourceHelper
    {
        Dictionary<int, Dictionary<int, ResourceUnitDTO>> resDict
            =new Dictionary<int, Dictionary<int, ResourceUnitDTO>>();
        public void LoadMapResource()
        {
            Task.Run(() => {
                var hasData= GameManager.CustomeModule<DataManager>().TryGetValue<MapResourceData>(out var mapResourceData);
                if (hasData)
                {
                    var length = mapResourceData.FixMapResourceList.Count;
                    var resList = mapResourceData.FixMapResourceList;
                    Random random = new Random();
                    for (int i = 0; i < length; i++)
                    {
                        var resObj = resList[i];
                        var len = resObj.ResAmount;
                        var lftX = resObj.ResSpawnPositon.X + resObj.ResSpawnRange;
                        var rghX = resObj.ResSpawnPositon.X - resObj.ResSpawnRange;
                        var lftZ = resObj.ResSpawnPositon.Z + resObj.ResSpawnRange;
                        var rghZ = resObj.ResSpawnPositon.Z - resObj.ResSpawnRange;
                        for (int j = 0; j < len; j++)
                        {
                            int x;
                            int z;
                            if (lftX > rghX)
                            {
                                x = random.Next(rghX, lftX);
                            }else
                            {
                                x = random.Next(lftX,rghX);
                            }
                            if (lftZ > rghZ)
                            {
                                z = random.Next(rghZ, lftZ);
                            }
                            else
                            {
                                z = random.Next(lftZ,rghZ);
                            }

                        }
                    }
                }
                });
        }
        public void OnRefreshResource()
        {
        }
        public void OnRoleEnterMap(RoleEntity roleEntity)
        {
            OperationData operationData = new OperationData();
            //operationData.DataMessage = Utility.Json.ToJson(ResUnitSetDict);
            operationData.OperationCode = (byte)OperationCode.SyncResources;
            roleEntity.SendMessage(operationData);
            Utility.Debug.LogInfo("yzqData" + Utility.Json.ToJson(roleEntity));
            Utility.Debug.LogInfo("yzqData发送技能布局" + roleEntity.RoleId);
            if (RedisDotNet.RedisHelper.KeyExistsAsync(RedisKeyDefine._SkillLayoutPerfix + roleEntity.RoleId).Result)
            {
                var dict = RedisHelper.Hash.HashGet<AdventureSkillLayoutDTO>(RedisKeyDefine._SkillLayoutPerfix + roleEntity.RoleId, roleEntity.RoleId.ToString());
                OperationData opData = new OperationData();
                opData.DataMessage = Utility.Json.ToJson(dict);
                opData.OperationCode = (byte)OperationCode.RefreshSkillLayout;
                Utility.Debug.LogInfo("yzqData发送技能布局" + Utility.Json.ToJson(dict));
                GameManager.CustomeModule<RoleManager>().SendMessage(roleEntity.RoleId, operationData);
            }
        }

        public void TakeUpMapResource(Dictionary<byte, object> dataMessage)
        {
            var occupiedUnitJson = Convert.ToString(Utility.GetValue(dataMessage, (byte)ParameterCode.OccupiedUnit));
            Utility.Debug.LogInfo("请求资源数据  :  " + occupiedUnitJson);
            var occupiedUnitObj = Utility.Json.ToObject<OccupiedUnitDTO>(occupiedUnitJson);
            var result = false;
            //GameManager.CustomeModule<MapResourceManager>().OccupiedResUnit(occupiedUnitObj);
            if (result)
            {
               // operationResponse.ReturnCode = (short)ReturnCode.Success;

                ResourceUnitSetDTO currentDictObj = null;
                //if (GameManager.CustomeModule<MapResourceManager>().ResUnitSetDict.TryGetValue(occupiedUnitObj.GlobalID, out currentDictObj))
                {

                    ResourceUnitDTO resourceUnitDTO = null;
                    if (currentDictObj.ResUnitDict.TryGetValue(occupiedUnitObj.ResID, out resourceUnitDTO))
                        resourceUnitDTO.Occupied = result;
                }
                var levelmanager = GameManager.CustomeModule<LevelManager>();
                OperationData operationData = new OperationData();
                operationData.DataMessage = Utility.Json.ToJson(occupiedUnitObj);
                //GameManager.CustomeModule<MapResourceManager>().OccupiedUnitSetCache.Clear();
                levelmanager.SendMessageToLevelS2C(0, operationData);
            }
            //else
            //    operationResponse.ReturnCode = (short)ReturnCode.Fail;
        }
    }
}
