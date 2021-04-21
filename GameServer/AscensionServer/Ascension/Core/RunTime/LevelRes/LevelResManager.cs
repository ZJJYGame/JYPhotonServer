using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol;
using UnityEngine;

namespace AscensionServer
{
    [Module]
    public class LevelResManager : Module, ILevelResManager
    {

        LevelResEntity adventureLevelResEntity;
        Pool<OperationData> opDataPool;
        Pool<Dictionary<byte, object>> messageDataPool;
        public override void OnPreparatory()
        {
            adventureLevelResEntity = LevelResEntity.Create(LevelTypeEnum.Adventure, 701);
            opDataPool = new Pool<OperationData>
                (() => new OperationData(), d => { d.OperationCode = (byte)OperationCode.LevelRes; }, d => { d.Dispose(); });
            messageDataPool = new Pool<Dictionary<byte, object>>(() => new Dictionary<byte, object>(), md => { md.Clear(); });
            CommandEventCore.Instance.AddEventListener((byte)OperationCode.LevelRes, ProcessHandlerC2S);
            SpawnRes();
            GameEntry.LevelManager.OnRoleEnterLevel += SYNResS2C;
            GameEntry.LevelManager.OnRoleExitLevel += FINResS2C;
        }
        void SpawnRes()
        {
            Random random = new Random();
            GameEntry.DataManager.TryGetValue<MapResSpanwInfoData>(out var resSpawnInfoData);
            var dict = resSpawnInfoData.MapResSpawnInfoDict;
            foreach (var res in dict.Values)
            {
                var fc = new FixCollectable();
                fc.Id = res.ResId;
                fc.CollectDict = new Dictionary<int, FixCollectable.CollectableRes>();
                var length = res.ResAmount;
                for (int i = 0; i < length; i++)
                {
                    var cr = new FixCollectable.CollectableRes();
                    cr.Id = i;
                    cr.CanCollected = true;

                    var vec = res.ResSpawnPositon.GetVector();
                    var xSign = Utility.Algorithm.Sign();
                    var xOffset = random.Next(0, res.ResSpawnRange);
                    vec.x += xSign == true ? xOffset : -xOffset;

                    var zSign = Utility.Algorithm.Sign();
                    var zOffset = random.Next(0, res.ResSpawnRange);
                    vec.z += zSign == true ? zOffset : -zOffset;

                    cr.FixTransform = new FixTransform(vec, Vector3.zero, Vector3.one);

                    fc.CollectDict.Add(i, cr);
                }
                adventureLevelResEntity.CollectableDict.Add(fc.Id, fc);
            }
        }
        void ProcessHandlerC2S(int sessionId, OperationData opData)
        {
            var subCode = (LevelResOpCode)opData.SubOperationCode;
            switch (subCode)
            {
                case LevelResOpCode.Collect:
                    CollectS2C(sessionId, opData);
                    break;
                case LevelResOpCode.Battle:
                    BattleS2C(sessionId, opData);
                    break;
            }
        }
        void SYNResS2C(LevelTypeEnum levelType, int levelId, int roleId)
        {
            var opdata = opDataPool.Spawn();
            opdata.SubOperationCode = (byte)LevelResOpCode.SYN;
            var messageData = messageDataPool.Spawn();
            var collectableJson = Utility.Json.ToJson(adventureLevelResEntity.CollectableDict);
            messageData.Add((byte)LevelResParameterCode.Collectable, collectableJson);
            GameEntry.RoleManager.SendMessage(roleId, opdata);
            opDataPool.Despawn(opdata);
        }
        void FINResS2C(LevelTypeEnum levelType,int levelId, int roleId)
        {

        }
        void CollectS2C(int sessionId, OperationData opData)
        {
            var json = Convert.ToString(opData.DataMessage);
            var messageDict = Utility.Json.ToObject<Dictionary<byte, object>>(json);
            var gid = Convert.ToInt32(Utility.GetValue(messageDict, (byte)LevelResParameterCode.GId));
            var eleid = Convert.ToInt32(Utility.GetValue(messageDict, (byte)LevelResParameterCode.EleId));
            var opdata = opDataPool.Spawn();
            opdata.SubOperationCode = (byte)LevelResOpCode.Collect;
            if (adventureLevelResEntity.Collect(gid, eleid))
            {
                var messageData = messageDataPool.Spawn();
                messageData.Add((byte)LevelResParameterCode.GId, gid);
                messageData.Add((byte)LevelResParameterCode.EleId, eleid);
                opData.DataMessage = Utility.Json.ToJson(messageData);
                opData.ReturnCode = (byte)ReturnCode.Success;
                messageDataPool.Despawn(messageData);
                adventureLevelResEntity.BroadCast2AllS2C(opdata);
            }
            else
            {
                opData.ReturnCode = (byte)ReturnCode.Fail;
                GameEntry.PeerManager.SendMessage(sessionId, opdata);
            }
            opDataPool.Despawn(opdata);
        }
        void BattleS2C(int sessionId, OperationData opData)
        {

        }
    }
}
