using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionData;
using AscensionProtocol.DTO;
using UnityEngine;
using Protocol;
using AscensionProtocol;
using RedisDotNet;

namespace AscensionServer
{
    public class TacticalEntity :Entity, IReference,IRefreshable
    {
        public int RoleID { get; set; }

        public int LevelID { get; set; }

        public int ID { get; set; }
        public TacticalDTO TacticalDTO { get; set; }

        public void Onlnit(int id, int roleid,int levelid)
        {
            this.RoleID = roleid;
            this.ID = id;
            this.LevelID = levelid;
        }


        public void OnRefresh()
        {


        }

        public void RedisDeleteCaback(string key)
        {
           TacticalDeploymentManager.Instance.RedisDeleteCaback(key);
        }

        public static TacticalEntity Create(int id,int roleid, int levelid)
        {
            TacticalEntity  tacticalEntity= GameManager.ReferencePoolManager.Spawn<TacticalEntity>();
            tacticalEntity.Onlnit(id, roleid, levelid);
            return tacticalEntity;
        }

/// <summary>
/// 可能需要一个发送移除阵法的回调用于回收自己
/// </summary>

        public void Clear()
        {

        }
    }
}
