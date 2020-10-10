using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Cosmos;
namespace AscensionServer
{
    [CustomeModule]
    public class NetworkManager:Module<NetworkManager>
    {
        INetworkMessageHelper messageHelper;
        public override void OnInitialization()
        {
            InitHelper();
        }
        public object EncodeMessage(object message)
        {
            return messageHelper.EncodeMessage(message);
        }
        void InitHelper()
        {
            var helper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, INetworkMessageHelper>(true);
            messageHelper = helper;
        }
    }
}
