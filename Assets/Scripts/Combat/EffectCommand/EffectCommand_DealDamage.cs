using System;
using ProjectBS.Combat;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_DealDamage : EffectCommandBase
    {
        private Action m_onCompleted = null;
        private string m_valueString = "";
        private string m_forceRoll = "";

        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;

        public override void Process(string[] vars, Action onCompleted)
        {
            caster.targetToDmg.Clear();

            m_valueString = vars[1];
            m_forceRoll = vars[2];
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
                timing = EffectProcesser.TriggerTiming.OnStartToAttack_Any,
                onEnded = OnStartToAttack_Any_Ended
            });
        }

        private void OnStartToAttack_Any_Ended()
        {
            processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = caster,
                target = m_targets[m_currentTargetIndex],
                timing = EffectProcesser.TriggerTiming.OnStartToAttack_Self,
                onEnded = OnStartToAttack_Self_Ended
            });
        }

        private void OnStartToAttack_Self_Ended()
        {
            CombatUnit _attackTarget = m_targets[m_currentTargetIndex];

            int _roll = int.Parse(m_forceRoll);
            if (_roll == -1)
                _roll = UnityEngine.Random.Range(0, 101);

            float _flee = 0.5f - (float)(caster.GetSpeed() / (float)(caster.GetSpeed() + _attackTarget.GetSpeed()));
            float _rawDmg = ((caster.GetAttack() * _roll) - (_attackTarget.GetDefence() * UnityEngine.Random.Range(0, 101))) * (1f - UnityEngine.Random.Range(0f, _flee));
            int _dmg = Convert.ToInt32(_rawDmg);

            if (_dmg < 1)
                _dmg = 1;

            caster.targetToDmg.Add(_attackTarget, _dmg);

            processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = null,
                target = m_targets[m_currentTargetIndex],
                timing = EffectProcesser.TriggerTiming.OnDamageCalculated_Any,
                onEnded = OnDamageCalculated_Any_Ended
            });
        }

        private void OnDamageCalculated_Any_Ended()
        {
            processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = caster,
                target = m_targets[m_currentTargetIndex],
                timing = EffectProcesser.TriggerTiming.OnDamageCalculated_Self,
                onEnded = GoNextTarget
            });
        }
    }
}

