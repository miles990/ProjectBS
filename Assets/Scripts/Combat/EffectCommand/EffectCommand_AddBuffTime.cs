using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_AddBuffTime : EffectCommandBase
    {
        private int m_effectID = 0;
        private int m_addTime = 0;
        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;
        private CombatUnit.Buff m_currentBuff = null;
        private Action m_onCompleted = null;
        private int m_currentBuffIndex = -1;

        public override void Process(string[] vars, Action onCompleted)
        {
            m_effectID = int.Parse(vars[1]);

            m_addTime = int.Parse(vars[2]);

            m_onCompleted = onCompleted;

            CombatTargetSelecter.Instance.StartSelect(
            new CombatTargetSelecter.SelectTargetData
            {
                id = GetSelectID(),
                attacker = processData.caster,
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
            GetPage<UI.CombatUIView>().RefreshAllInfo();

            m_currentTargetIndex++;
            if (m_currentTargetIndex >= m_targets.Count)
            {
                m_onCompleted?.Invoke();
                return;
            }

            if (m_effectID != -1)
            {
                m_currentBuff = m_targets[m_currentTargetIndex].buffs.Find(x => x.soruceID == m_effectID);
                if (m_currentBuff != null)
                {
                    m_targets[m_currentTargetIndex].AddBuffTime(
                        m_currentBuff,
                        m_addTime,
                        delegate { DisaplayRemoveBuff(GoNextTarget); },
                        GoNextTarget);
                }
                else
                {
                    GoNextTarget();
                }
            }
            else
            {
                m_currentBuffIndex = -1;
                GoNextBuff();
            }
        }

        private void DisaplayRemoveBuff(Action onShown)
        {
            GetPage<UI.CombatUIView>().DisplayRemoveBuff(new UI.CombatUIView.DisplayBuffData
            {
                buffName = ContextConverter.Instance.GetContext(m_currentBuff.GetBuffSourceData().NameContextID),
                taker = m_targets[m_currentTargetIndex]
            }, onShown);
        }

        private void GoNextBuff()
        {
            m_currentBuffIndex++;
            if (m_currentBuffIndex >= m_targets[m_currentTargetIndex].buffs.Count)
            {
                GoNextTarget();
                return;
            }
            m_currentBuff = m_targets[m_currentTargetIndex].buffs[m_currentBuffIndex];
            m_targets[m_currentTargetIndex].AddBuffTime(
                m_currentBuff,
                m_addTime,
                delegate { DisaplayRemoveBuff(GoNextBuff); },
                GoNextBuff);
        }
    }
}
