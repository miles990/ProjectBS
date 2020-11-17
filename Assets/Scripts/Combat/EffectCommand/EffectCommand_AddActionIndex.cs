using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_AddActionIndex : EffectCommandBase
    {
        private Action m_onCompleted = null;
        private int m_addIndex = 0;

        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;

        public override void Process(string[] vars, Action onCompleted)
        {
            m_addIndex = int.Parse(vars[1]);
            m_onCompleted = onCompleted;

            CombatTargetSelecter.Instance.StartSelect(new CombatTargetSelecter.SelectTargetData
            {
                id = GetSelectID(),
                attacker = GetSelf(),
                commandString = vars[0],
                onSelected = OnSelected
            });
        }

        private void OnSelected(List<CombatUnit> targets)
        {
            m_targets = targets;
            m_currentTargetIndex = -1;

            GoNextTarget();
        }

        private void GoNextTarget()
        {
            m_currentTargetIndex++;
            if (m_currentTargetIndex >= m_targets.Count)
            {
                m_onCompleted?.Invoke();
                return;
            }

            CombatUtility.CurrentComabtManager.AddActionIndex(m_targets[m_currentTargetIndex], m_addIndex);
            GetPage<UI.CombatUIView>().ShowAddActionIndex(m_targets[m_currentTargetIndex], m_addIndex, GoNextTarget);
        }
    }
}
