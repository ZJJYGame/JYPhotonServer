using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer;
using AscensionProtocol.DTO;
using AscensionProtocol;
using System.Timers;
using Photon.SocketServer;
using EventData = Photon.SocketServer.EventData;
using System.Threading;
using Cosmos;

namespace AscensionServer.Threads
{
   public  class SyncAdventureSkillEvent:SyncEvent
    {
        public override void Handler(object state)
        {
            SkillbuffEnd();
        }

        public override void OnInitialization()
        {
            base.OnInitialization();
            EventData.Code = (byte)EventCode.RoleAdventureEndtSkill;

        }


        void SkillbuffEnd()
        {
            HashSet<RoleAdventureSkillDTO> roleAdventureSkills = new HashSet<RoleAdventureSkillDTO>();
            //var loggedList = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
            //var loggedCount = loggedList.Count;
            //if (loggedCount <= 0)
            //    return;
            //for (int i = 0; i < loggedCount; i++)
            //{
            //    if (!loggedList[i].IsUseSkill)
            //    {
            //        roleAdventureSkills.Add(loggedList[i].PeerCache.RoleAdventureSkill);
            //        loggedList[i].IsUseSkill = true;
            //    }

            //}

            var data = new Dictionary<byte, object>();
            data.Add((byte)ParameterCode.RoleAdventureEndSkill, Utility.Json.ToJson(roleAdventureSkills));
            EventData.Parameters = data;

            //foreach (var p in loggedList)
            //{
            //    p.SendEvent(EventData, SendParameter);
            //    //AscensionServer._Log.Info("技能时间已发出》》》》》》》》》》》》》》》》》》》》");
            //}
        }






}

}
