using Cosmos;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [CustomeModule]
    public class ChatManager:Module<ChatManager>
    {
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.PORT_CHAT,OnChatMessageC2S);
        }
        void OnChatMessageC2S(object msg)
        {
            try
            {
                var chatMsg = Utility.Json.ToObject<C2SChatMessage>(msg.ToString());
                GameManager.CustomeModule<PeerManager>().SendMessage(chatMsg.ReceiverSessionId, chatMsg.Message); ;
            }
            catch (Exception e)
            {

            }
        }
    }
}
