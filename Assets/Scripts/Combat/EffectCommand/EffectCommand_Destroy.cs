using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_Destroy : EffectCommandBase
    {
        private Action m_onCompleted = null;
        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;

        public override void Process(string[] vars, Action onCompleted)
        {
            m_onCompleted = onCompleted;
            AddSkillOrEffectInfo();
            CombatTargetSelecter.Instance.StartSelect(new CombatTargetSelecter.SelectTargetData
            {
                selectID = CombatTargetSelecter.Instance.GetSelectID(processData),
                attacker = GetSelf(),
                currentEffectedTarget = processData.target,
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

            CombatUtility.ComabtManager.ForceRemoveUnit(m_targets[m_currentTargetIndex].UDID);
            GetPage<UI.CombatUIView>().AddCombatInfo
                (
                    string.Format
                    (
                        ContextConverter.Instance.GetContext(500023),
                        m_targets[m_currentTargetIndex].name
                    ), null
                );
            GetPage<UI.CombatUIView>().ShowUnitDestoryed(m_targets[m_currentTargetIndex], GoNextTarget);
        }
    }
}
