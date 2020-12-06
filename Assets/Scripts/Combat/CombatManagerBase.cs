﻿using KahaGameCore.Interface;
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
        public List<CombatUnit> AllUnit { get { return new List<CombatUnit>(m_units); } }
        protected List<CombatUnit> m_units = new List<CombatUnit>();

        public AllCombatUnitAllEffectProcesser AllUnitAllEffectProcesser { get; protected set; }
        protected List<CombatUnitAction> m_unitActions = new List<CombatUnitAction>();
        protected CombatUnitAction m_currentAction = null;

        public CombatUnit CurrentDyingUnit { get; protected set; }

        public abstract void AddActionIndex(CombatUnit unit, int addIndex);
        public abstract void AddExtraAction(CombatUnit unit, bool isImmediate);
        public abstract List<CombatUnit> GetSameCampUnits(int camp);
        public abstract void SetCurrentActionMinAttackRoll(int value);
        public abstract void SetCurrentActionMinDefenseRoll(int value);
        public abstract void EndComabat(bool isWin);
        public abstract void ForceEndCurrentAction();
        public abstract void ForceUnitDie(CombatUnit unit, System.Action onDiedCommandEnded);
        public abstract void ForceRemoveUnit(CombatUnit unit);
        public abstract void StartCombat(List<CombatUnit> playerUnits, List<CombatUnit> opponentUnits);
    }
}