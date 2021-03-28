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

            AddSkillOrEffectInfo();

            CombatTargetSelecter.Instance.StartSelect(new CombatTargetSelecter.SelectTargetData
            {
                id = CombatTargetSelecter.Instance.GetSelectID(processData),
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

            CombatUtility.ComabtManager.AddActionIndex(m_targets[m_currentTargetIndex].UDID, m_addIndex);
            
            if(m_addIndex < 0)
            {
                GetPage<UI.CombatUIView>().AddCombatInfo
                    (
                      string.Format
                      (
                          ContextConverter.Instance.GetContext(500003),
                          m_targets[m_currentTargetIndex].name,
                          (m_addIndex * -1).ToString()
                      ), null
                    );
            }
            else
            {
                GetPage<UI.CombatUIView>().AddCombatInfo
                    (
                      string.Format
                      (
                          ContextConverter.Instance.GetContext(500004),
                          m_targets[m_currentTargetIndex].name,
                          m_addIndex.ToString()
                      ), null
                    );
            }
            
            GetPage<UI.CombatUIView>().ShowAddActionIndex(m_targets[m_currentTargetIndex], m_addIndex, GoNextTarget);
        }
    }
}
