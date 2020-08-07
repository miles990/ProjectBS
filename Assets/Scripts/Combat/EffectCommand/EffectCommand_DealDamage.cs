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
            processData.caster.targetToDmg.Clear();

            m_valueString = vars[1];
            m_forceRoll = vars[2];
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
            m_currentTargetIndex++;
            if (m_currentTargetIndex >= m_targets.Count)
            {
                ShowAttackAnimation();
                return;
            }

            processData.processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = null,
                target = m_targets[m_currentTargetIndex],
                timing = EffectProcesser.TriggerTiming.OnStartToAttack_Any,
                onEnded = OnStartToAttack_Any_Ended
            });
        }

        private void OnStartToAttack_Any_Ended()
        {
            processData.processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = processData.caster,
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

            if(!int.TryParse(m_valueString, out int _attack))
            {
                CombatUtility.Calculate(new CombatUtility.CalculateData
                {
                    caster = processData.caster,
                    target = m_targets[m_currentTargetIndex],
                    formula = m_valueString,
                    useRawValue = false
                });
            }

            float _flee = 0.5f - (float)(processData.caster.GetSpeed() / (float)(processData.caster.GetSpeed() + _attackTarget.GetSpeed()));
            float _rawDmg = (float)((_attack * _roll) - (_attackTarget.GetDefence() * UnityEngine.Random.Range(0, 101))) * (1f - UnityEngine.Random.Range(0f, _flee));
            int _dmg = Convert.ToInt32(_rawDmg);

            if (_dmg < 1)
                _dmg = 1;

            processData.caster.targetToDmg.Add(_attackTarget, _dmg);

            processData.processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = null,
                target = m_targets[m_currentTargetIndex],
                timing = EffectProcesser.TriggerTiming.OnDamageCalculated_Any,
                onEnded = OnDamageCalculated_Any_Ended
            });
        }

        private void OnDamageCalculated_Any_Ended()
        {
            processData.processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = processData.caster,
                target = m_targets[m_currentTargetIndex],
                timing = EffectProcesser.TriggerTiming.OnDamageCalculated_Self,
                onEnded = OnDamageCalculated_Self_Ended
            });
        }

        private void OnDamageCalculated_Self_Ended()
        {
            processData.processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = null,
                target = processData.caster,
                timing = EffectProcesser.TriggerTiming.OnStartToTakeDamage_Any,
                onEnded = OnStartToTakeDamage_Any_Ended
            });
        }

        private void OnStartToTakeDamage_Any_Ended()
        {
            processData.processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = m_targets[m_currentTargetIndex],
                target = processData.caster,
                timing = EffectProcesser.TriggerTiming.OnStartToTakeDamage_Self,
                onEnded = GoNextTarget
            });
        }

        private void ShowAttackAnimation()
        {
            UnityEngine.Debug.Log("ShowAttackAnimation: Skill ID=" + CombatManager.Instance.CurrentActionInfo.CastingSkill.ID);
            KahaGameCore.Static.TimerManager.Schedule(1f, OnAnimationShown);
        }

        private void OnAnimationShown()
        {
            m_currentTargetIndex = -1;
            ApplyDamageToNextTarget();
        }

        private void ApplyDamageToNextTarget()
        {
            m_currentTargetIndex++;
            if (m_currentTargetIndex >= m_targets.Count)
            {
                KahaGameCore.Static.TimerManager.Schedule(1f, m_onCompleted);
                return;
            }

            m_targets[m_currentTargetIndex].HP -= processData.caster.targetToDmg[m_targets[m_currentTargetIndex]];

            // TODO: display damage

            processData.processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = null,
                target = processData.caster,
                timing = EffectProcesser.TriggerTiming.OnDamageTaken_Any,
                onEnded = OnDamageTaken_Any_Ended
            });
        }

        private void OnDamageTaken_Any_Ended()
        {
            processData.processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = m_targets[m_currentTargetIndex],
                target = processData.caster,
                timing = EffectProcesser.TriggerTiming.OnDamageTaken_Self,
                onEnded = ApplyDamageToNextTarget
            });
        }
    }
}

