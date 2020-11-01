using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_RemoveBuffByTag : EffectCommandBase
    {
        private int m_tag = 0;
        private int m_removeStackCount = 0;
        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;
        private int m_currentBuffIndex = -1;
        private Action m_onEnded = null;

        private CombatUnit.Buff m_targetBuff;

        public override void Process(string[] vars, Action onCompleted)
        {
            m_tag = int.Parse(vars[1]);
            m_removeStackCount = int.Parse(vars[2]);

            CombatTargetSelecter.Instance.StartSelect(
                new CombatTargetSelecter.SelectTargetData
                {
                    attacker = GetSelf(),
                    commandString = vars[0],
                    onSelected = OnTargetSelected
                });
        }

        private void OnTargetSelected(List<CombatUnit> targets)
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
                m_onEnded?.Invoke();
                return;
            }

            m_currentBuffIndex = -1;
            GoNextBuff();
        }

        private void GoNextBuff()
        {
            m_currentBuffIndex++;
            if(m_currentBuffIndex >= m_targets[m_currentTargetIndex].buffs.Count)
            {
                GoNextTarget();
                return;
            }

            if(m_targets[m_currentTargetIndex].buffs[m_currentBuffIndex].GetSkillEffectData().Tag == m_tag)
            {
                m_targets[m_currentTargetIndex].AddBuffStack(
                    m_targets[m_currentTargetIndex].buffs[m_currentBuffIndex],
                    m_removeStackCount,
                    DisaplayRemoveBuff,
                    GoNextBuff);
            }
            else
            {
                GoNextBuff();
            }
        }

        private void DisaplayRemoveBuff()
        {
            GetPage<UI.CombatUIView>().DisplayRemoveBuff(new UI.CombatUIView.DisplayBuffData
            {
                buffName = ContextConverter.Instance.GetContext(m_targetBuff.GetSkillEffectData().NameContextID),
                taker = m_targets[m_currentTargetIndex]
            }, GoNextBuff);
        }
    }
}
