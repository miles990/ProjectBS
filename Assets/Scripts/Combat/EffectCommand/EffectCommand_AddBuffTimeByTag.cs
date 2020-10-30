using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_AddBuffTimeByTag : EffectCommandBase
    {
        private int m_tag = 0;
        private int m_addTime = 0;
        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;
        private CombatUnit.Buff m_currentBuff = null;
        private Action m_onCompleted = null;
        private int m_currentBuffIndex = -1;

        public override void Process(string[] vars, Action onCompleted)
        {
            m_tag = int.Parse(vars[1]);

            m_addTime = int.Parse(vars[2]);

            m_onCompleted = onCompleted;

            CombatTargetSelecter.Instance.StartSelect(
            new CombatTargetSelecter.SelectTargetData
            {
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

            m_currentBuff = m_targets[m_currentTargetIndex].buffs.Find(x => x.GetSkillEffectData().Tag == m_tag);
            if (m_currentBuff != null)
            {
                m_currentBuff.remainingTime += m_addTime;
                if (m_currentBuff.remainingTime <= 0)
                {
                    if (!m_targets[m_currentTargetIndex].RemoveBuff(m_currentBuff, -1, delegate { DisaplayRemoveBuff(GoNextTarget); }))
                    {
                        GoNextTarget();
                    }
                }
                else
                {
                    GoNextTarget();
                }
            }
            else
            {
                GoNextTarget();
            }
        }

        private void DisaplayRemoveBuff(Action onShonw)
        {
            GetPage<UI.CombatUIView>().DisplayRemoveBuff(new UI.CombatUIView.DisplayBuffData
            {
                buffName = ContextConverter.Instance.GetContext(m_currentBuff.GetSkillEffectData().NameContextID),
                taker = m_targets[m_currentTargetIndex]
            }, onShonw);
        }
    }

}
