using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_TriggerBuff : EffectCommandBase
    {
        private int m_buffID = 0;
        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;
        private Action m_onCompleted = null;
        private EffectProcesser.TriggerTiming m_timing = EffectProcesser.TriggerTiming.OnActived;

        public override void Process(string[] vars, Action onCompleted)
        {
            m_buffID = int.Parse(vars[1]);
            m_timing = (EffectProcesser.TriggerTiming)Enum.Parse(typeof(EffectProcesser.TriggerTiming), vars[2]);
            m_onCompleted = onCompleted;

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

            CombatUnit.Buff _buff = m_targets[m_currentTargetIndex].GetBuffByBuffID(m_buffID);
            if(_buff != null)
            {
                EffectProcessManager.GetBuffProcesser(m_buffID).Start(
                    new EffectProcesser.ProcessData
                    {
                        allEffectProcesser = processData.allEffectProcesser,
                        caster = CombatUtility.ComabtManager.GetUnitByUDID(_buff.ownerUnitUDID),
                        target = CombatUtility.ComabtManager.GetUnitByUDID(_buff.ownerUnitUDID),
                        refenceSkill = null,
                        referenceBuff = _buff,
                        skipIfCount = 0,
                        onEnded = GoNextTarget,
                        timing = m_timing
                    });
            }
        }
    }
}
