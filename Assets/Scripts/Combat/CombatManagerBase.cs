using KahaGameCore.Interface;
using System.Collections.Generic;

namespace ProjectBS.Combat
{
    public abstract class CombatManagerBase : Manager
    {
        public struct CombatActionInfo
        {
            public CombatUnit actor;
            public int minAttackRoll;
            public int minDefenseRoll;
        }

        public CombatActionInfo CurrentActionInfo
        {
            get
            {
                return new CombatActionInfo
                {
                    actor = m_currentAction.Actor,
                    minAttackRoll = m_currentAction.MinAttackRoll,
                    minDefenseRoll = m_currentAction.MinDefenseRoll
                };
            }
        }

        public int TurnCount { get; protected set; } = 0;
        public EffectProcesser.TriggerTiming CurrentState { get; protected set; }

        public List<CombatUnit> AllUnit { get { return new List<CombatUnit>(m_units); } }
        protected List<CombatUnit> m_units = new List<CombatUnit>();

        public CombatUnit GetUnitByUDID(string udid)
        {
            return m_units.Find(x => x.UDID == udid);
        }

        protected List<CombatUnitAction> m_unitActions = new List<CombatUnitAction>();
        protected CombatUnitAction m_currentAction = null;

        public CombatUnit CurrentDyingUnit { get; protected set; }

        public AllCombatUnitAllEffectProcesser AllProcesser { get; protected set; }

        public abstract void Shake();
        public abstract void AddActionIndex(string unitUDID, int addIndex);
        public abstract void AddExtraAction(string unitUDID, bool isImmediate);
        public abstract void AddSkillQueue(string unitUDID, int skillID);
        public abstract List<CombatUnit> GetSameCampUnits(int camp);
        public abstract void SetCurrentActionMinAttackRoll(int value);
        public abstract void SetCurrentActionMinDefenseRoll(int value);
        public abstract void EndComabat(bool isWin);
        public abstract void ForceEndCurrentAction();
        public abstract void ForceUnitDie(string unitUDID, System.Action onDiedCommandEnded);
        public abstract void ForceRemoveUnit(string unitUDID);
        public abstract void StartCombat(List<CombatUnit> playerUnits, List<CombatUnit> opponentUnits);
    }
}
