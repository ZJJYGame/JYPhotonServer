using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Cosmos;
namespace AscensionServer
{
    [Module]
    public class NetworkManager:Cosmos. Module ,INetworkManager
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


