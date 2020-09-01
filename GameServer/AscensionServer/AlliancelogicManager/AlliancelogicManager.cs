using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventData = Photon.SocketServer.EventData;
using Cosmos;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;

namespace AscensionServer
{
    public  class AlliancelogicManager:ConcurrentSingleton<AlliancelogicManager>
    {
        /// <summary>
        /// 储存登录盟主的字典，用于派发申请消息
        /// </summary>
        Dictionary<int, AscensionPeer> alliancePoolDict = new Dictionary<int, AscensionPeer>();
        /// <summary>
        /// 储存申请仙盟的玩家的字典，用于派发消息
        /// </summary>
        Dictionary<int, AscensionPeer> applyPoolDict = new Dictionary<int, AscensionPeer>();

        #region 仙盟用于同意的逻辑
        /// <summary>
        /// 获得储存的仙盟盟主
        /// </summary>
        /// <param name="id"></param>
        /// <param name="peer"></param>
        /// <returns></returns>
        public bool TryGetValue(int id, out AscensionPeer peer)
        {
            peer = null;
            if (!alliancePoolDict.ContainsKey(id))
                return false;
            else
            {
                peer = alliancePoolDict[id];
                return true;
            }
        }
        /// <summary>
        /// 添加新增的仙盟盟主
        /// </summary>
        /// <param name="id"></param>
        /// <param name="peer"></param>
        /// <returns></returns>
        public bool TryAdd(int id , AscensionPeer peer)
        {

            if (alliancePoolDict.ContainsKey(id))
                return false;
            else
            {
                alliancePoolDict.Add(id,peer);
                return true;
            }
        }
        /// <summary>
        /// 移除已有的仙盟盟主
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool TryRemove(int id)
        {
            if (!alliancePoolDict.ContainsKey(id))
                return false;
            else
            {
                alliancePoolDict.Remove(id);
                return true;

            }
        }
        /// <summary>
        /// 仙盟申请消息派发
        /// </summary>
        /// <param name="peer"></param>
        public void SendApplyForMessage(AscensionPeer peer, Dictionary<byte, object> date)
        {
            //EventData eventData = new EventData();
            //eventData.Parameters = date;
            //peer.SendEvent(eventData,);
        }
        #endregion
        #region 仙盟用于请求的逻辑
        /// <summary>
        /// 获得储存的申请玩家
        /// </summary>
        /// <param name="id"></param>
        /// <param name="peer"></param>
        /// <returns></returns>
        public bool TryGetValueApply(int id, out AscensionPeer peer)
        {
            peer = null;
            if (!applyPoolDict.ContainsKey(id))
                return false;
            else
            {
                peer = applyPoolDict[id];
                return true;
            }
        }
        /// <summary>
        /// 添加新增的申请玩家
        /// </summary>
        /// <param name="id"></param>
        /// <param name="peer"></param>
        /// <returns></returns>
        public bool TryAddApply(int id, AscensionPeer peer)
        {

            if (applyPoolDict.ContainsKey(id))
                return false;
            else
            {
                applyPoolDict.Add(id, peer);
                return true;
            }
        }
        /// <summary>
        /// 移除已加入仙盟的玩家
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool TryRemoveApply(int id)
        {
            if (!applyPoolDict.ContainsKey(id))
                return false;
            else
            {
                applyPoolDict.Remove(id);
                return true;

            }
        }
        /// <summary>
        /// 仙盟申请同意的消息派发
        /// </summary>
        /// <param name="peer"></param>
        public void SendJoinSuccesMessage(AscensionPeer peer, Dictionary<byte, object> date)
        {
            //EventData eventData = new EventData();
            //eventData.Parameters = date;
            //peer.SendEvent(eventData, new SendParameter);
        }
        #endregion

        /// <summary>
        /// 用于查询数据库数据的整合的函数
        /// </summary>
        /// <typeparam name="T">所需查询的类型</typeparam>
        /// <param name="keyname">映射的变量名</param>
        /// <param name="key">对应的id</param>
        /// <returns></returns>
        public T GetNHCriteria<T>(  string  keyname,int key)
        {
            NHCriteria nHCriteria  = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue(keyname, key);
            var dataObjectTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelectAsync<T>(nHCriteria);

                GameManager.ReferencePoolManager.Despawns(nHCriteria);
                return dataObjectTemp.Result;

        }
        /// <summary>
        /// 仙盟的申请整合方法
        /// </summary>
        /// <param name="role"></param>
        /// <param name="schoolDTO"></param>
        /// <returns></returns>
        public ApplyForAllianceDTO JointDate( Role role  , RoleSchool schoolDTO)
        {
            ApplyForAllianceDTO applyForAllianceDTO = new ApplyForAllianceDTO() { RoleID = role.RoleID,School= schoolDTO .RoleJoinedSchool,MemberName=role.RoleName};
            return applyForAllianceDTO;

        }



        public void HandlerAlliance(int roleid)
        {

        }
    }
}
