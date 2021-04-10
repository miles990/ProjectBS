using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_AddBuffAmount : EffectCommandBase
    {
        private int m_buffID = 0;
        private int m_addAmountCount = 0;
        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;
        private CombatUnit.Buff m_currentBuff = null;
        private Action m_onCompleted = null;
        private int m_currentBuffIndex = -1;

        public override void Process(string[] vars, Action onCompleted)
        {
            if(!int.TryParse(vars[1], out m_buffID))
            {
                throw new Exception("[EffectCommand_AddBuffAmount][Process] invaild buff id=" + vars[1]);
            }

            if (!int.TryParse(vars[2], out m_addAmountCount))
            {
                throw new Exception("[EffectCommand_AddBuffAmount][Process] invaild add amount=" + vars[2]);
            }

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
            if(m_currentTargetIndex >= m_targets.Count)
            {
                m_onCompleted?.Invoke();
                return;
            }

            if(m_buffID != -1)
            {
                m_currentBuff = m_targets[m_currentTargetIndex].GetBuffByBuffID(m_buffID);
                if (m_currentBuff != null)
                {
                    m_targets[m_currentTargetIndex].AddBuffAmount(
                        m_currentBuff,
                        m_addAmountCount,
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

        private void AddInfo()
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
                          ContextConverter.Instance.GetContext(m_currentBuff.GetBuffSourceData().NameContextID)
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
                          ContextConverter.Instance.GetContext(m_currentBuff.GetBuffSourceData().NameContextID)
                      ), null
                    );
            }
        }

        private void DisaplayRemoveBuff(Action onShonw)
        {
            GetPage<UI.CombatUIView>().DisplayRemoveBuff(new UI.CombatUIView.DisplayBuffData
            {
                buffName = ContextConverter.Instance.GetContext(m_currentBuff.GetBuffSourceData().NameContextID),
                taker = m_targets[m_currentTargetIndex]
            }, onShonw);
        }

        private void GoNextBuff()
        {
            m_currentBuffIndex++;
            if(m_currentBuffIndex >= m_targets[m_currentTargetIndex].OwnBuffCount)
            {
                GoNextTarget();
                return;
            }
            m_currentBuff = m_targets[m_currentTargetIndex].GetBuffByIndex(m_currentBuffIndex);
            AddInfo();
            m_targets[m_currentTargetIndex].AddBuffAmount(
                            m_currentBuff,
                            m_addAmountCount,
                            delegate { DisaplayRemoveBuff(GoNextBuff); },
                            GoNextBuff);
        }
    }
}
