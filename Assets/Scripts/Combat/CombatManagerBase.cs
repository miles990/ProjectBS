using KahaGameCore.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.Combat
{
    public class CombatManagerBase : Manager
    {
        public List<CombatUnit> AllUnit { get { return new List<CombatUnit>(m_units); } }
        private List<CombatUnit> m_units = new List<CombatUnit>();

        public int TurnCount { get; private set; } = 0;
        public CombatUnit CurrentDyingUnit { get; private set; }

        public AllCombatUnitAllEffectProcesser AllUnitAllEffectProcesser { get; private set; }
        private List<CombatUnitAction> m_unitActions = new List<CombatUnitAction>();
        private CombatUnitAction m_currentAction = null;

        private System.Action m_onDiedCommandEnded = null;

        public void AddActionIndex(CombatUnit unit, int addIndex)
        {
            int _currentIndex = m_unitActions.FindIndex(x => x.Actor == unit);
            if (_currentIndex != -1)
            {
                int _targetIndex = _currentIndex + addIndex;
                if (_targetIndex < 0)
                    _targetIndex = 0;
                if (_targetIndex >= m_unitActions.Count)
                    _targetIndex = m_unitActions.Count - 1;
                CombatUnitAction _refAction = m_unitActions[_currentIndex];
                m_unitActions.RemoveAt(_currentIndex);
                m_unitActions.Insert(_targetIndex, _refAction);
            }

            GetPage<UI.CombatUIView>().RefreshActionQueueInfo(m_unitActions);
        }

        public void AddExtraAction(CombatUnit unit, bool isImmediate)
        {
            CombatUnitAction _newAction = new CombatUnitAction(unit, AllUnitAllEffectProcesser);
            if (isImmediate)
            {
                m_unitActions.Insert(0, _newAction);
            }
            else
            {
                m_unitActions.Add(_newAction);
            }

            GetPage<UI.CombatUIView>().RefreshActionQueueInfo(m_unitActions);
        }

        public List<CombatUnit> GetSameCampUnits(int camp)
        {
            List<CombatUnit> _units = new List<CombatUnit>();
            for (int i = 0; i < m_units.Count; i++)
            {
                if (m_units[i].camp == camp)
                {
                    _units.Add(m_units[i]);
                }
            }

            return _units;
        }

        public void SetCurrentActionMinAttackRoll(int value)
        {
            if (value < 0)
                value = 0;

            m_currentAction.MinAttackRoll = value;
        }

        public void SetCurrentActionMinDefenseRoll(int value)
        {
            if (value < 0)
                value = 0;

            m_currentAction.MinDefenseRoll = value;
        }
    }
}
