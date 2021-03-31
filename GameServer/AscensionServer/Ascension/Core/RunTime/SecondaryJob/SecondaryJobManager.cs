using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using Protocol;
using RedisDotNet;

namespace AscensionServer
{
    [Module]
    public partial class SecondaryJobManager : Cosmos. Module,ISecondaryJobManager
    {
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((byte)OperationCode.SyncSecondaryJob, ProcessHandlerC2S);
        }

        void ProcessHandlerC2S(int seeionid, OperationData packet)
        {
            switch ((SecondaryJobOpCode)packet.SubOperationCode)
            {
                case SecondaryJobOpCode.GetAlchemyStatus:
                    break;
                case SecondaryJobOpCode.UpdateAlchemy:
                    break;
                case SecondaryJobOpCode.CompoundAlchemy:
                    break;
                case SecondaryJobOpCode.GetPuppetStatus:
                    break;
                case SecondaryJobOpCode.UpdatePuppet:
                    break;
                case SecondaryJobOpCode.CompoundPuppet:
                    break;
                case SecondaryJobOpCode.GetWeaponStatus:
                    break;
                case SecondaryJobOpCode.UpdateWeapon:
                    break;
                case SecondaryJobOpCode.CompoundWeapon:
                    break;
                default:
                    break;
            }
        }
    }
}


