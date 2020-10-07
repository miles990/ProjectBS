using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_DealDamage : EffectCommandBase
    {
        private Action m_onCompleted = null;
        private string m_valueString = "";
        private string m_rollMin = "";
        private string m_ingnoreDefense = "";

        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;

        public override void Process(string[] vars, Action onCompleted)
        {
            GetSelf().targetToDmg.Clear();

            m_valueString = vars[1];
            m_rollMin = vars[2];
            m_ingnoreDefense = vars[3];
            m_onCompleted = onCompleted;

            CombatTargetSelecter.Instance.StartSelect(
                new CombatTargetSelecter.SelectTargetData
                {
                    attacker = GetSelf(),
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

            float _attackAddRollPersent = 0.01f * (float)_roll;
            float _defenseAddRollPersent = UnityEngine.Random.Range(0f, 1f);

            if (!float.TryParse(m_valueString, out float _attack))
            {
                _attack = CombatUtility.Calculate(new CombatUtility.CalculateData
                {
                    caster = processData.caster,
                    target = m_targets[m_currentTargetIndex],
                    formula = m_valueString,
                    useRawValue = false
                });
            }

            float _finalAttackValue = (float)(_attack + _attack * _attackAddRollPersent);
            float _finalDefenseValue = (float)(_attackTarget.GetDefense() + _attackTarget.GetDefense() * _defenseAddRollPersent);
            float _ingnoreDefense = float.Parse(m_ingnoreDefense);
            float _flee = 0.5f - (float)(processData.caster.GetSpeed() / (float)(processData.caster.GetSpeed() + _attackTarget.GetSpeed()));
            float _fleeReduceDmgPersent = (1f - UnityEngine.Random.Range(0f, _flee));

            float _rawDmg = (_finalAttackValue - (_finalDefenseValue * (1f - _ingnoreDefense))) * _fleeReduceDmgPersent;

            int _dmg = Convert.ToInt32(_rawDmg);

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
            int _id = 0;
            if(processData.refenceSkill != null)
            {
                _id = processData.refenceSkill.NameContextID;
            }
            else if(processData.referenceBuff != null)
            {
                _id = KahaGameCore.Static.GameDataManager.GetGameData<Data.SkillEffectData>(processData.referenceBuff.effectID).NameContextID;
            }

            GetPage<UI.CombatUIView>().ShowSkillAnimation(new UI.CombatUIView.SkillAnimationData
            {
                caster = processData.caster,
                nameContextID = _id,
                onEnded = OnAnimationShown
            });
        }

        private void OnAnimationShown()
        {
            m_currentTargetIndex = -1;
            ApplyDamageToNextTarget();
        }

        private void OnDamageApplied()
        {
            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = null,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnAttacked_Any,
                onEnded = OnAttackedAnyEnded
            });
        }

        private void OnAttackedAnyEnded()
        {
            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = processData.caster,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnAttacked_Self,
                onEnded = End
            });
        }

        private void End()
        {
            KahaGameCore.Static.TimerManager.Schedule(1f, m_onCompleted);
        }

        private void ApplyDamageToNextTarget()
        {
            m_currentTargetIndex++;
            if (m_currentTargetIndex >= m_targets.Count)
            {
                OnDamageApplied();
                return;
            }

            m_targets[m_currentTargetIndex].lastTakenDamage = processData.caster.targetToDmg[m_targets[m_currentTargetIndex]];
            m_targets[m_currentTargetIndex].HP -= processData.caster.targetToDmg[m_targets[m_currentTargetIndex]];
            m_targets[m_currentTargetIndex].Hatred -= processData.caster.targetToDmg[m_targets[m_currentTargetIndex]];

            processData.caster.Hatred += processData.caster.targetToDmg[m_targets[m_currentTargetIndex]];

            GetPage<UI.CombatUIView>().DisplayDamage(new UI.CombatUIView.DisplayDamageData
            {
                taker = m_targets[m_currentTargetIndex],
                damageValue = processData.caster.targetToDmg[m_targets[m_currentTargetIndex]]
            }, null);
            GetPage<UI.CombatUIView>().RefreshAllInfo();

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

