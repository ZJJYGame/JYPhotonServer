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
    public class MultiplayResManager : Module, IMultiplayResManager
    {
        Dictionary<int, FixCollectable> collectableDict;
        public override void OnPreparatory()
        {
            collectableDict = new Dictionary<int, FixCollectable>();
            CommandEventCore.Instance.AddEventListener((byte)OperationCode.MultiplayRes, ProcessHandlerC2S);
            SpawnRes();
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
                    cr.IsCollected = false;

                    var vec = res.ResSpawnPositon.GetVector();
                    var xSign = Utility.Algorithm.Sign();
                    var xOffset = random.Next(0, res.ResSpawnRange);
                    vec.x += xSign==true? xOffset : -xOffset;

                    var zSign = Utility.Algorithm.Sign();
                    var zOffset = random.Next(0, res.ResSpawnRange);
                    vec.z += zSign == true ? zOffset: -zOffset;

                    cr.FixTransform = new FixTransform(vec, Vector3.zero, Vector3.one);

                    fc.CollectDict.Add(i, cr);
                }
                collectableDict.Add(fc.Id, fc);
            }
        }
        void ProcessHandlerC2S(int sessionId, OperationData opData)
        {
            var subCode = (MultiplayResOperationCode)opData.SubOperationCode;
            switch (subCode)
            {
                case MultiplayResOperationCode.SYN:
                    break;
                case MultiplayResOperationCode.Collect:
                    {
                        CollectS2C(sessionId, opData);
                    }
                    break;
                case MultiplayResOperationCode.Battle:
                    break;
                case MultiplayResOperationCode.FIN:
                    break;
            }
        }
        void CollectS2C(int sessionId, OperationData opData)
        {

        }
    }
}
