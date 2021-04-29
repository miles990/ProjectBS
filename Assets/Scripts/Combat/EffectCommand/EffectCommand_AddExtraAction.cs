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

            CombatUtility.ComabtManager.AddExtraAction(m_targets[m_currentTargetIndex].UDID, m_isImmediate);
            GetPage<UI.CombatUIView>().ShowAddExtraAction(m_targets[m_currentTargetIndex], delegate
            {
                if (m_isImmediate)
                {
                    GetPage<UI.CombatUIView>().AddCombatInfo
                        (
                            string.Format(ContextConverter.Instance.GetContext(500014), m_targets[m_currentTargetIndex].name), GoNextTarget
                        );
                }
                else
                {
                    GetPage<UI.CombatUIView>().AddCombatInfo
                        (
                            string.Format(ContextConverter.Instance.GetContext(500013), m_targets[m_currentTargetIndex].name), GoNextTarget
                        );
                }
            });
        }
    }
}
