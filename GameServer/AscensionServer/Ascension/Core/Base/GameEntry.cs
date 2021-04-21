using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    public class GameEntry
    {
        public static IDataManager DataManager { get { return GameManager.GetModule<IDataManager>(); } }
        public static INetworkManager NetworkManager { get { return GameManager.GetModule<INetworkManager>();} }
        public static IGongFaManager  GongFaManager{ get { return GameManager.GetModule<IGongFaManager>();} }
        public static IPetStatusManager  PetStatusManager{ get { return GameManager.GetModule<IPetStatusManager>();} }
        public static IPeerManager PeerManager { get { return GameManager.GetModule<IPeerManager>();} }
        public static IRoleManager  RoleManager{ get { return GameManager.GetModule<IRoleManager>();} }
        public static IRoomManager  RoomManager{ get { return GameManager.GetModule<IRoomManager>();} }
        public static IAuctionManager AuctionManager { get { return GameManager.GetModule<IAuctionManager>();} }
        public static IServerBattleManager ServerBattleManager { get { return GameManager.GetModule<IServerBattleManager>();} }
        public static IServerTeamManager ServerTeamManager { get { return GameManager.GetModule<IServerTeamManager>();} }
        public static IRecordManager RecordManager { get { return GameManager.GetModule<IRecordManager>();} }
        public static ILevelManager LevelManager { get { return GameManager.GetModule<ILevelManager>();} }
        public static IDemonicSoulManager DemonicSoulManager { get { return GameManager.GetModule<IDemonicSoulManager>();} }
        public static ITacticalManager TacticalDeploymentManager { get { return GameManager.GetModule<ITacticalManager>();} }
        public static IBattleCharacterManager BattleCharacterManager { get { return GameManager.GetModule<IBattleCharacterManager>(); } }
        public static IBattleRoomManager BattleRoomManager { get { return GameManager.GetModule<IBattleRoomManager>(); } }
        public static IPracticeManager practiceManager { get { return GameManager.GetModule<IPracticeManager>(); } }
        public static ILevelResManager  LevelResManager{ get { return GameManager.GetModule<ILevelResManager>(); } }
    }
}


