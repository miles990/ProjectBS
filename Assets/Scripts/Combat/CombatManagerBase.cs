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

        public CombatUnit GetUnitByUDID(string udid)
        {
            return m_units.Find(x => x.UDID == udid);
        }

        protected List<CombatUnitAction> m_unitActions = new List<CombatUnitAction>();
        protected CombatUnitAction m_currentAction = null;

        public CombatUnit CurrentDyingUnit { get; protected set; }

        public AllCombatUnitAllEffectProcesser GetNewAllProcesser()
        {
            return new AllCombatUnitAllEffectProcesser(m_units);
        }

        public abstract void Shake();
        public abstract void AddActionIndex(string unitUDID, int addIndex);
        public abstract void AddExtraAction(string unitUDID, bool isImmediate);
        public abstract List<CombatUnit> GetSameCampUnits(int camp);
        public abstract void SetCurrentActionMinAttackRoll(int value);
        public abstract void SetCurrentActionMinDefenseRoll(int value);
        public abstract void EndComabat(bool isWin);
        public abstract void ForceEndCurrentAction();
        public abstract void ForceUnitDie(string unitUDID, System.Action onDiedCommandEnded);
        public abstract void ForceRemoveUnit(string unitUDID);
        public abstract void StartCombat(List<CombatUnit> playerUnits, List<CombatUnit> opponentUnits);
        
        public void AddInfo(string text, System.Action onShown)
        {
            GetPage<UI.CombatUIView>().AddCombatInfo(text, onShown);
        }
    }
}
