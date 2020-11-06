using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_AddExtraAction : EffectCommandBase
    {
        private Action m_onCompleted = null;
        private bool m_isImmediate = false;

        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;

        public override void Process(string[] vars, Action onCompleted)
        {
            m_isImmediate = vars[1] == "1";
            m_onCompleted = onCompleted;

            CombatTargetSelecter.Instance.StartSelect(new CombatTargetSelecter.SelectTargetData
            {
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

            CombatUtility.CurrentComabtManager.AddExtraAction(m_targets[m_currentTargetIndex], m_isImmediate);
            GetPage<UI.CombatUIView>().ShowAddExtraAction(m_targets[m_currentTargetIndex], GoNextTarget);
        }
    }
}
