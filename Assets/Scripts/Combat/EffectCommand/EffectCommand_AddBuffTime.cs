using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_AddBuffTime : EffectCommandBase
    {
        private int m_buffID = 0;
        private int m_addTime = 0;
        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;
        private CombatUnit.Buff m_currentBuff = null;
        private Action m_onCompleted = null;
        private int m_currentBuffIndex = -1;

        public override void Process(string[] vars, Action onCompleted)
        {
            m_buffID = int.Parse(vars[1]);

            m_addTime = int.Parse(vars[2]);

            m_onCompleted = onCompleted;

            AddSkillOrEffectInfo();

            CombatTargetSelecter.Instance.StartSelect(
            new CombatTargetSelecter.SelectTargetData
            {
                selectID = CombatTargetSelecter.Instance.GetSelectID(processData),
                attacker = processData.caster,
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
                m_onCompleted?.Invoke();
                return;
            }

            if (m_buffID != -1)
            {
                m_currentBuff = m_targets[m_currentTargetIndex].GetBuffByBuffID(m_buffID);
                if (m_currentBuff != null)
                {
                    AddInfo();
                    GetPage<UI.CombatUIView>().ShowAddBuffTime
                        (m_targets[m_currentTargetIndex],
                        ContextConverter.Instance.GetContext(m_currentBuff.GetBuffSourceData().NameContextID),
                        m_addTime,
                        delegate
                        {
                            m_targets[m_currentTargetIndex].AddBuffTime(
                                m_currentBuff,
                                m_addTime,
                                delegate { DisaplayRemoveBuff(GoNextTarget); },
                                GoNextTarget);
                        });
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
            if (m_currentBuffIndex >= m_targets[m_currentTargetIndex].OwnBuffCount)
            {
                GoNextTarget();
                return;
            }
            m_currentBuff = m_targets[m_currentTargetIndex].GetBuffByIndex(m_currentBuffIndex);
            AddInfo();

            GetPage<UI.CombatUIView>().ShowAddBuffTime
                (m_targets[m_currentTargetIndex],
                ContextConverter.Instance.GetContext(m_currentBuff.GetBuffSourceData().NameContextID),
                m_addTime,
                delegate
                {
                    m_targets[m_currentTargetIndex].AddBuffTime(
                        m_currentBuff,
                        m_addTime,
                        delegate { DisaplayRemoveBuff(GoNextBuff); },
                        GoNextBuff);
                });
        }

        private void AddInfo()
        {
            if (m_addTime > 0)
            {
                GetPage<UI.CombatUIView>().AddCombatInfo
                    (
                      string.Format
                      (
                          ContextConverter.Instance.GetContext(500007),
                          m_targets[m_currentTargetIndex].name,
                          ContextConverter.Instance.GetContext(m_currentBuff.GetBuffSourceData().NameContextID),
                          m_addTime.ToString()
                      ), null
                    );
            }
            else
            {
                GetPage<UI.CombatUIView>().AddCombatInfo
                    (
                      string.Format
                      (
                          ContextConverter.Instance.GetContext(500008),
                          m_targets[m_currentTargetIndex].name,
                          ContextConverter.Instance.GetContext(m_currentBuff.GetBuffSourceData().NameContextID),
                          (m_addTime * -1).ToString()
                      ), null
                    );
            }
        }
    }
}
