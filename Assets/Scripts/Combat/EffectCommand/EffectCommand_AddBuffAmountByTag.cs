using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_AddBuffAmountByTag : EffectCommandBase
    {
        private int m_tag = 0;
        private int m_addAmountCount = 0;
        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;
        private int m_currentBuffIndex = -1;
        private Action m_onEnded = null;

        private CombatUnit.Buff m_targetBuff;

        public override void Process(string[] vars, Action onCompleted)
        {
            m_tag = int.Parse(vars[1]);
            m_addAmountCount = int.Parse(vars[2]);
            m_onEnded = onCompleted;

            CombatTargetSelecter.Instance.StartSelect(
                new CombatTargetSelecter.SelectTargetData
                {
                    id = CombatTargetSelecter.Instance.GetSelectID(processData),
                    attacker = GetSelf(),
                    currentEffectedTarget = processData.target,
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
            if(m_currentBuffIndex >= m_targets[m_currentTargetIndex].OwnBuffCount)
            {
                GoNextTarget();
                return;
            }

            m_targetBuff = m_targets[m_currentTargetIndex].GetBuffByIndex(m_currentBuffIndex);

            if (m_targetBuff.GetBuffSourceData().Tag == m_tag)
            {
                if (m_addAmountCount > 0)
                {
                    GetPage<UI.CombatUIView>().AddCombatInfo
                        (
                          string.Format
                          (
                              ContextConverter.Instance.GetContext(500005),
                              m_targets[m_currentTargetIndex].name,
                              m_addAmountCount.ToString(),
                              ContextConverter.Instance.GetContext(m_targetBuff.GetBuffSourceData().NameContextID)
                          ), null
                        );
                }
                else
                {
                    GetPage<UI.CombatUIView>().AddCombatInfo
                        (
                          string.Format
                          (
                              ContextConverter.Instance.GetContext(500006),
                              m_targets[m_currentTargetIndex].name,
                              (m_addAmountCount * -1).ToString(),
                              ContextConverter.Instance.GetContext(m_targetBuff.GetBuffSourceData().NameContextID)
                          ), null
                        );
                }

                m_targets[m_currentTargetIndex].AddBuffAmount(
                    m_targets[m_currentTargetIndex].GetBuffByIndex(m_currentBuffIndex),
                    m_addAmountCount,
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
                buffName = ContextConverter.Instance.GetContext(m_targetBuff.GetBuffSourceData().NameContextID),
                taker = m_targets[m_currentTargetIndex]
            }, GoNextBuff);
        }
    }
}
