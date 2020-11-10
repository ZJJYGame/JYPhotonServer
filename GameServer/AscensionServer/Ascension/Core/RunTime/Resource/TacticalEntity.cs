using Cosmos;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol;
using NHibernate.Linq.Clauses;
using System.ServiceModel.Configuration;
using AscensionProtocol.DTO;
using RedisDotNet;

namespace AscensionServer
{
    public class TacticalEntity :Entity, IReference,IRefreshable
    {
        public int RoleID { get; set; }

        public int LevelID { get; set; }

        public int ID { get; set; }
        public TacticalDTO TacticalDTO { get; set; }
        public TacticalEntity()
        {
            RoleID = -1 ;
            LevelID = -1;
            ID = -1;
            TacticalDTO = new TacticalDTO();
        }

        public void Onlnit(int id, int roleid,int levelid)
        {
            this.RoleID = roleid;
            this.ID = id;
            this.LevelID = levelid;
            RedisManager.Instance.AddKeyExpireListener(RedisKeyDefine._DeldteTacticalPerfix + id, this.RedisDeleteCaback);
        }
        public void OnRefresh()
        {

        }

        public void RedisDeleteCaback(string key)
        {
            GameManager.CustomeModule<TacticalDeploymentManager>().RedisDeleteCaback(key);
        }

        public static TacticalEntity Create(int id,int roleid, int levelid)
        {
            TacticalEntity  te= GameManager.ReferencePoolManager.Spawn<TacticalEntity>();
            te.Onlnit(id, roleid, levelid);
            return te;
        }



        public void Clear()
        {

        }
    }
}
