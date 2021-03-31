using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionServer.Model;
using AscensionProtocol.DTO;

namespace AscensionServer
{
    public class BattleCharacterEntity : IReference,IComparable<BattleCharacterEntity>
    {
        /// <summary>
        /// 房间ID
        /// </summary>
        public int RoomID { get; protected set; }
        /// <summary>
        /// 唯一ID
        /// </summary>
        public int UniqueID { get; protected set; }
        /// <summary>
        /// 公共ID
        /// </summary>
        public int GlobalID { get; protected set; }
        public string Name { get; protected set; }
        /// <summary>
        /// 角色是否死亡
        /// </summary>
        public bool HasDie { get { return CharacterBattleData.Hp <= 0 ? true : false; } }
        /// <summary>
        /// 属性数据
        /// </summary>
        public CharacterBattleData CharacterBattleData { get; protected set; }
        public BattleFactionType BattleFactionType { get; protected set; }

        public List<BattleCharacterEntity> FriendCharacterEntities { get; protected set; }
        public List<BattleCharacterEntity> EnemyCharacterEntities { get; protected set; }

        public BattleCmd BattleCmd { get; protected set; }
        public int ActionID { get; protected set; }
        public List<int> TargetIDList { get; protected set; }

       public BattleSkillController BattleSkillController { get; protected set; }

        /// <summary>
        /// 设置该角色的敌方和友方
        /// </summary>
        public void SetFriendAndEnemy(List<BattleCharacterEntity> factionOneCharacterEntites, List<BattleCharacterEntity> factionTwoCharacterEntites)
        {
            switch (BattleFactionType)
            {
                case BattleFactionType.FactionOne:
                    FriendCharacterEntities = factionOneCharacterEntites;
                    EnemyCharacterEntities = factionTwoCharacterEntites;
                    break;
                case BattleFactionType.FactionTwo:
                    FriendCharacterEntities = factionTwoCharacterEntites;
                    EnemyCharacterEntities = factionOneCharacterEntites;
                    break;
            }
        }

        /// <summary>
        /// 转换成发送用的人物初始化数据
        /// </summary>
        /// <returns></returns>
        public virtual T ToBattleDataBase<T>()
            where T: BattleDataBase
        {
            return default(T);
        }
        /// <summary>
        /// 设置角色该回合战斗行为
        /// </summary>
        public virtual void SetBattleAction(BattleCmd battleCmd,BattleTransferDTO battleTransferDTO)
        {
        }
        /// <summary>
        /// 出手前判断并分配战斗行为
        /// </summary>
        public virtual void AllocationBattleAction()
        {
            //todo buff判断，强制控制角色行动
        }
        /// <summary>
        /// 角色进行行动的方法
        /// </summary>
        public List<BattleTransferDTO> Action()
        {
            if (HasDie)
                return new List<BattleTransferDTO>();
            switch (BattleCmd)
            {
                case BattleCmd.PropsInstruction:
                    break;
                case BattleCmd.SkillInstruction:
                    return BattleSkillController.UseSkill();
                case BattleCmd.MagicWeapon:
                    break;
                case BattleCmd.CatchPet:
                    break;
                case BattleCmd.SummonPet:
                    break;
                case BattleCmd.RunAwayInstruction:
                    break;
                case BattleCmd.Tactical:
                    break;
                case BattleCmd.Defend:
                    break;
            }
            return new List<BattleTransferDTO>();
        }
        /// <summary>
        /// 受到技能等效果的结算
        /// </summary>
        public void OnActionEffect(BattleDamageData battleDamageData)
        {
            switch (battleDamageData.battleSkillActionType)
            {
                case BattleSkillActionType.Damage:
                    //todo受击时的触发判断
                    CharacterBattleData.ChangeProperty(battleDamageData.baseDamageTargetProperty, battleDamageData.damageNum);
                    CharacterBattleData.ChangeProperty(battleDamageData.extraDamageTargetProperty, battleDamageData.extraDamageNum);
                    break;
                case BattleSkillActionType.Heal:
                    break;
                case BattleSkillActionType.Resurrection:
                    break;
                case BattleSkillActionType.Summon:
                    break;
            }
        }

        protected void Init()
        {
            FriendCharacterEntities = new List<BattleCharacterEntity>();
            EnemyCharacterEntities = new List<BattleCharacterEntity>();
            BattleSkillController = new BattleSkillController(this);
            TargetIDList = new List<int>();

        }


        public virtual void Clear()
        {

        }

        public int CompareTo(BattleCharacterEntity other)
        {
            if (CharacterBattleData.AttackSpeed > other.CharacterBattleData.AttackSpeed)
                return 1;
            else if (CharacterBattleData.AttackSpeed > other.CharacterBattleData.AttackSpeed)
                return 0;
            else
                return -1;
        }
    }
}
