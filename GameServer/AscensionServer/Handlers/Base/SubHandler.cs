using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;

namespace AscensionServer
{
    /// <summary>
    /// 子操作处理者
    /// </summary>
    /// <typeparam name="T">拥有者</typeparam>
    public abstract class SubHandler: ISubHandler
    {
        public Handler Owner { get; set; }
        public SubOperationCode SubOpCode { get; protected set; }
        public abstract void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer);
        public virtual void OnInitialization() { }
        public virtual void OnTermination() { }
    }
}
