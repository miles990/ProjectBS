﻿using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_DealDamage : EffectCommandBase
    {
        private Action m_onCompleted = null;
        private string m_valueString = "";
        private string m_rollMin = "";
        private string m_ingnoreDefence = "";

        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;

        public override void Process(string[] vars, Action onCompleted)
        {
            processData.caster.targetToDmg.Clear();

            m_valueString = vars[1];
            m_rollMin = vars[2];
            m_ingnoreDefence = vars[3];
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

            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = null,
                target = m_targets[m_currentTargetIndex],
                timing = EffectProcesser.TriggerTiming.OnStartToAttack_Any,
                onEnded = OnStartToAttack_Any_Ended
            });
        }

        private void OnStartToAttack_Any_Ended()
        {
            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
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

            int _roll = int.Parse(m_rollMin);
            _roll = UnityEngine.Random.Range(_roll, 101);

            float _rollPersent = 0.01f * (float)_roll;

            if(!float.TryParse(m_valueString, out float _attack))
            {
                _attack = CombatUtility.Calculate(new CombatUtility.CalculateData
                {
                    caster = processData.caster,
                    target = m_targets[m_currentTargetIndex],
                    formula = m_valueString,
                    useRawValue = false
                });
            }

            //UnityEngine.Debug.Log("_attack=" + _attack);
            //UnityEngine.Debug.Log("_rollPersent=" + _rollPersent);

            float _finalAttackValue = (float)(_attack + _attack * _rollPersent);
            float _finalDefenceValue = (float)(_attackTarget.GetDefence() + _attackTarget.GetDefence() * UnityEngine.Random.Range(0f, 1f));
            float _ingnoreDefence = float.Parse(m_ingnoreDefence);
            float _flee = 0.5f - (float)(processData.caster.GetSpeed() / (float)(processData.caster.GetSpeed() + _attackTarget.GetSpeed()));
            float _fleeReduceDmgPersent = (1f - UnityEngine.Random.Range(0f, _flee));

            //UnityEngine.Debug.Log("_finalAttackValue=" + _finalAttackValue);
            //UnityEngine.Debug.Log("_finalDefenceValue=" + _finalDefenceValue);
            //UnityEngine.Debug.Log("_ingnoreDefence=" + _ingnoreDefence);
            //UnityEngine.Debug.Log("_flee=" + _flee);
            //UnityEngine.Debug.Log("_fleeReduceDmgPersent=" + _fleeReduceDmgPersent);

            float _rawDmg = (_finalAttackValue - (_finalDefenceValue * (1f - _ingnoreDefence))) * _fleeReduceDmgPersent;
            //UnityEngine.Debug.Log("_rawDmg=" + _rawDmg);
            int _dmg = Convert.ToInt32(_rawDmg);
            //UnityEngine.Debug.Log("_dmg=" + _dmg);

            if (_dmg < 1)
                _dmg = 1;

            processData.caster.targetToDmg.Add(_attackTarget, _dmg);

            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = null,
                target = m_targets[m_currentTargetIndex],
                timing = EffectProcesser.TriggerTiming.OnDamageCalculated_Any,
                onEnded = OnDamageCalculated_Any_Ended
            });
        }

        private void OnDamageCalculated_Any_Ended()
        {
            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = processData.caster,
                target = m_targets[m_currentTargetIndex],
                timing = EffectProcesser.TriggerTiming.OnDamageCalculated_Self,
                onEnded = OnDamageCalculated_Self_Ended
            });
        }

        private void OnDamageCalculated_Self_Ended()
        {
            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = null,
                target = processData.caster,
                timing = EffectProcesser.TriggerTiming.OnStartToTakeDamage_Any,
                onEnded = OnStartToTakeDamage_Any_Ended
            });
        }

        private void OnStartToTakeDamage_Any_Ended()
        {
            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = m_targets[m_currentTargetIndex],
                target = processData.caster,
                timing = EffectProcesser.TriggerTiming.OnStartToTakeDamage_Self,
                onEnded = GoNextTarget
            });
        }

        private void ShowAttackAnimation()
        {
            UnityEngine.Debug.Log("ShowAttackAnimation: Skill ID=" + processData.caster.lastSkillID);
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
            m_targets[m_currentTargetIndex].hatred -= processData.caster.targetToDmg[m_targets[m_currentTargetIndex]];
            processData.caster.hatred += processData.caster.targetToDmg[m_targets[m_currentTargetIndex]];

            CombatManager.Instance.ShowDamage(new UI.CombatUIView.DisplayDamageData
            {
                attackerName = processData.caster.name,
                defenderName = m_targets[m_currentTargetIndex].name,
                damageValue = processData.caster.targetToDmg[m_targets[m_currentTargetIndex]],
                skillName = ContextConverter.Instance.GetContext(processData.refenceSkill.NameContextID)
            });

            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = null,
                target = processData.caster,
                timing = EffectProcesser.TriggerTiming.OnDamageTaken_Any,
                onEnded = OnDamageTaken_Any_Ended
            });
        }

        private void OnDamageTaken_Any_Ended()
        {
            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = m_targets[m_currentTargetIndex],
                target = processData.caster,
                timing = EffectProcesser.TriggerTiming.OnDamageTaken_Self,
                onEnded = ApplyDamageToNextTarget
            });
        }
    }
}

