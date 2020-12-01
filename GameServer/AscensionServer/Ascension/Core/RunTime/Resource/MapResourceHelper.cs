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

namespace AscensionServer
{
    [ImplementProvider]
    public class MapResourceHelper : IMapResourceHelper
    {
        public void LoadMapResource()
        {
            Task.Run(() => {
                var hasData= GameManager.CustomeModule<DataManager>().TryGetValue<MapResourceData>(out var mapResourceData);
                if (hasData)
                {
                    var length = mapResourceData.FixMapResourceList.Count;
                    var resList = mapResourceData.FixMapResourceList;
                    for (int i = 0; i < length; i++)
                    {
                        //resList[i].ResId
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
                if (GameManager.CustomeModule<MapResourceManager>().ResUnitSetDict.TryGetValue(occupiedUnitObj.GlobalID, out currentDictObj))
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
