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

            UnityEngine.Debug.Log("_attackAddRollPersent=" + (_attackAddRollPersent * 100) + "%");
            UnityEngine.Debug.Log("_defenseAddRollPersent=" + (_defenseAddRollPersent * 100) + "%");

            if (!float.TryParse(m_valueString, out float _attack))
            {
                _attack = CombatUtility.Calculate(new CombatUtility.CalculateData
                {
                    caster = processData.caster,
                    target = m_targets[m_currentTargetIndex],
                    referenceBuff = processData.referenceBuff,
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

            if(processData.caster.targetToDmg.ContainsKey(_attackTarget))
            {
                processData.caster.targetToDmg[_attackTarget] = _dmg;
            }
            else
            {
                processData.caster.targetToDmg.Add(_attackTarget, _dmg);
            }
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
                _id = processData.referenceBuff.GetSkillEffectData().NameContextID;
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
                timing = EffectProcesser.TriggerTiming.OnAttackEnded_Any,
                onEnded = OnAttackedAnyEnded
            });
        }

        private void OnAttackedAnyEnded()
        {
            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = processData.caster,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnAttackEnded_Self,
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

            if (m_targets[m_currentTargetIndex].shields.Count > 0)
            {
                if (m_targets[m_currentTargetIndex].shields[0].value > processData.caster.targetToDmg[m_targets[m_currentTargetIndex]])
                {
                    m_targets[m_currentTargetIndex].shields[0].value -= processData.caster.targetToDmg[m_targets[m_currentTargetIndex]];
                    processData.caster.targetToDmg[m_targets[m_currentTargetIndex]] = 0;
                    OnDamageTaken_Self_Ended();
                    return;
                }
                else
                {
                    processData.caster.targetToDmg[m_targets[m_currentTargetIndex]] -= m_targets[m_currentTargetIndex].shields[0].value;
                    if (m_targets[m_currentTargetIndex].shields[0].triggerSKillID != 0)
                    {
                        EffectProcessManager.GetSkillProcesser(m_targets[m_currentTargetIndex].shields[0].triggerSKillID)
                            .Start(new EffectProcesser.ProcessData
                            {
                                allEffectProcesser = processData.allEffectProcesser,
                                caster = m_targets[m_currentTargetIndex],
                                target = null,
                                refenceSkill = null,
                                referenceBuff = m_targets[m_currentTargetIndex].shields[0].parentBuff,
                                skipIfCount = 0,
                                timing = EffectProcesser.TriggerTiming.OnActived,
                                onEnded = OnShieldSkillTriggered
                            });
                    }
                    else
                    {
                        OnShieldSkillTriggered();
                    }
                    return;
                }
            }

            m_targets[m_currentTargetIndex].lastTakenDamage = processData.caster.targetToDmg[m_targets[m_currentTargetIndex]];
            m_targets[m_currentTargetIndex].HP -= processData.caster.targetToDmg[m_targets[m_currentTargetIndex]];
            m_targets[m_currentTargetIndex].Hatred -= processData.caster.targetToDmg[m_targets[m_currentTargetIndex]];
            processData.caster.Hatred += processData.caster.targetToDmg[m_targets[m_currentTargetIndex]];

            GetPage<UI.CombatUIView>().DisplayDamage(new UI.CombatUIView.DisplayDamageData
            {
                taker = m_targets[m_currentTargetIndex],
                damageValue = processData.caster.targetToDmg[m_targets[m_currentTargetIndex]]
            }, OnDmgShown);
        }

        private void OnShieldSkillTriggered()
        {
            if(m_targets[m_currentTargetIndex].shields[0].parentBuff == null)
            {
                m_targets[m_currentTargetIndex].shields.RemoveAt(0);
                OnShieldBuffStackRemoved();
            }
            else
            {
                m_targets[m_currentTargetIndex].AddBuffStack(
                    m_targets[m_currentTargetIndex].shields[0].parentBuff,
                    -1,
                    OnShieldBuffStackRemoved,
                    OnShieldBuffStackRemoved);
            }
        }

        private void OnShieldBuffStackRemoved()
        {
            m_currentTargetIndex--;
            ApplyDamageToNextTarget();
        }

        private void OnDmgShown()
        {
            GetPage<UI.CombatUIView>().RefreshAllInfo();

            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = null,
                target = processData.caster,
                timing = EffectProcesser.TriggerTiming.OnDamageDealed_Any,
                onEnded = OnDamageDealed_Any_Ended
            });
        }

        private void OnDamageDealed_Any_Ended()
        {
            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = processData.caster,
                target = m_targets[m_currentTargetIndex],
                timing = EffectProcesser.TriggerTiming.OnDamageDealed_Self,
                onEnded = OnDamageDealed_Self_Ended
            });
        }

        private void OnDamageDealed_Self_Ended()
        {
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
                onEnded = OnDamageTaken_Self_Ended
            });
        }

        private void OnDamageTaken_Self_Ended()
        {
            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = null,
                target = processData.caster,
                timing = EffectProcesser.TriggerTiming.OnBeAttackedEnded_Any,
                onEnded = OnBeAttackedEnded_Any_Ended
            });
        }

        private void OnBeAttackedEnded_Any_Ended()
        {
            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = m_targets[m_currentTargetIndex],
                target = processData.caster,
                timing = EffectProcesser.TriggerTiming.OnBeAttackedEnded_Self,
                onEnded = ApplyDamageToNextTarget
            });
        }
    }
}

