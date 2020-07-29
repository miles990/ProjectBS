using System;
using ProjectBS.Combat;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_DealDamage : EffectCommandBase
    {
        private Action m_onCompleted = null;
        private string m_valueString = "";

        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;
        private Dictionary<CombatUnit, int> m_targetToDmg = new Dictionary<CombatUnit, int>();

        public override void Process(string[] vars, Action onCompleted)
        {
            m_valueString = vars[1];
            m_onCompleted = onCompleted;

            CombatTargetSelecter.Instance.StartSelect(
                new CombatTargetSelecter.SelectTargetData
                {
                    attacker = caster,
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
                return;
            }

            processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = null,
                target = m_targets[m_currentTargetIndex],
                timing = EffectProcesser.TriggerTiming.OnStartToDealDamage_Any,
                onEnded = OnStartToDealDamageAnyEnded
            });
        }

        private void OnStartToDealDamageAnyEnded()
        {
            processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = caster,
                target = m_targets[m_currentTargetIndex],
                timing = EffectProcesser.TriggerTiming.OnStartToDealDamage_Self,
                onEnded = OnStartToDealDamageSelfEnded
            });
        }

        private void OnStartToDealDamageSelfEnded()
        {

        }
    }
}

