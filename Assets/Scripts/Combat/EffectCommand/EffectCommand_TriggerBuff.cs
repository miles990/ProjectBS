using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_TriggerBuff : EffectCommandBase
    {
        private int m_buffID = 0;
        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;
        private int m_currentBuffAmount = -1;
        private Action m_onCompleted = null;
        private EffectProcesser.TriggerTiming m_timing = EffectProcesser.TriggerTiming.OnActived;

        private CombatUnit.Buff m_currentBuff = null;

        public override void Process(string[] vars, Action onCompleted)
        {
            m_buffID = int.Parse(vars[1]);
            m_timing = (EffectProcesser.TriggerTiming)Enum.Parse(typeof(EffectProcesser.TriggerTiming), vars[2]);
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

            m_currentBuff = m_targets[m_currentTargetIndex].GetBuffByBuffID(m_buffID);

            GetPage<UI.CombatUIView>().AddCombatInfo
                (
                    string.Format
                    (
                        ContextConverter.Instance.GetContext(500030),
                        m_targets[m_currentTargetIndex].name,
                        ContextConverter.Instance.GetContext(m_currentBuff.GetBuffSourceData().NameContextID)
                    ), null
                );

            if(m_currentBuff == null)
            {
                GoNextTarget();
            }
            else
            {
                m_currentBuffAmount = -1;
                GoNextAmount();
            }
        }

        private void GoNextAmount()
        {
            m_currentBuffAmount++;
            if (m_currentBuffAmount >= m_currentBuff.amount)
            {
                m_onCompleted?.Invoke();
                return;
            }

            EffectProcessManager.GetBuffProcesser(m_buffID).Start(
                new EffectProcesser.ProcessData
                {
                    allEffectProcesser = processData.allEffectProcesser,
                    caster = CombatUtility.ComabtManager.GetUnitByUDID(m_currentBuff.ownerUnitUDID),
                    target = CombatUtility.ComabtManager.GetUnitByUDID(m_currentBuff.ownerUnitUDID),
                    refenceSkill = null,
                    referenceBuff = m_currentBuff,
                    skipIfCount = 0,
                    onEnded = GoNextAmount,
                    timing = m_timing
                });
        }
    }
}
