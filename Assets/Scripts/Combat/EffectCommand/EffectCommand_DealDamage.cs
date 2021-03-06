using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_DealDamage : EffectCommandBase
    {
        private Action m_onCompleted = null;
        private string m_valueString = "";
        private string m_ingnoreDefense = "";

        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;

        public override void Process(string[] vars, Action onCompleted)
        {
            //UnityEngine.Debug.Log("DealDamage Start(" + GetHashCode() + ")");

            GetSelf().targetToDmg.Clear();

            m_valueString = vars[1];
            m_ingnoreDefense = vars[2];
            m_onCompleted = onCompleted;

            AddSkillOrEffectInfo();
            GetPage<UI.CombatUIView>().AddCombatInfo
                (
                    string.Format
                    (
                        ContextConverter.Instance.GetContext(500020),
                        GetSelf().name
                    ), null
                );

            CombatTargetSelecter.Instance.StartSelect(
                new CombatTargetSelecter.SelectTargetData
                {
                    selectID = CombatTargetSelecter.Instance.GetSelectID(processData),
                    attacker = GetSelf(),
                    currentEffectedTarget = processData.target,
                    commandString = vars[0],
                    onSelected = OnTargetSelected
                });
        }

        private void OnTargetSelected(List<CombatUnit> targets)
        {
            //UnityEngine.Debug.Log("DealDamage OnTargetSelected(" + GetHashCode() + ")");

            m_targets = targets;

            if (m_targets.Count <= 0)
            {
                End();
                return;
            }

            m_currentTargetIndex = -1;
            GoNextTarget();
        }

        private void GoNextTarget()
        {
            m_currentTargetIndex++;
            //UnityEngine.Debug.Log("DealDamage GoNextTarget(" + GetHashCode() + ") m_currentTargetIndex=" + m_currentTargetIndex + ", m_targets.Count=" + m_targets.Count);
            if (m_currentTargetIndex >= m_targets.Count)
            {
                ShowAttackAnimation();
                return;
            }

            //UnityEngine.Debug.Log("DealDamage OnStartToAttack_Other(" + GetHashCode() + ") m_currentTargetIndex=" + m_currentTargetIndex + ", m_targets.Count=" + m_targets.Count);
            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = processData.caster,
                target = m_targets[m_currentTargetIndex],
                timing = EffectProcesser.TriggerTiming.OnStartToAttack_Other,
                onEnded = OnStartToAttack_Other_Ended
            });
        }

        private void OnStartToAttack_Other_Ended()
        {
            //UnityEngine.Debug.Log("DealDamage OnStartToAttack_Self(" + GetHashCode() + ") m_currentTargetIndex=" + m_currentTargetIndex + ", m_targets.Count=" + m_targets.Count);

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
            //UnityEngine.Debug.Log("DealDamage Calculate Dmg(" + GetHashCode() + ")");

            CombatUnit _attackTarget = m_targets[m_currentTargetIndex];

            CombatManagerBase.CombatActionInfo _info = CombatUtility.ComabtManager.CurrentActionInfo;
            float _minAttackRollPersent = (float)_info.minAttackRoll / 100f;
            float _minDefenseRollPersent = (float)_info.minDefenseRoll / 100f;

            float _attackAddRollPersent = UnityEngine.Random.Range(_minAttackRollPersent, 1f);
            float _defenseAddRollPersent = UnityEngine.Random.Range(_minDefenseRollPersent, 1f);

            CombatUtility.LastAttackRoll = Convert.ToInt32(_attackAddRollPersent * 100f);
            CombatUtility.LastDefenseRoll = Convert.ToInt32(_defenseAddRollPersent * 100f);

            if (!float.TryParse(m_valueString, out float _attack))
            {
                _attack = CombatUtility.Calculate(new CombatUtility.CalculateData
                {
                    caster = processData.caster,
                    target = m_targets[m_currentTargetIndex],
                    processData = processData,
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
            _rawDmg *= CombatUtility.GetHatredPersent(_attackTarget);

            int _dmg = Convert.ToInt32(_rawDmg);

            if (_dmg < 1)
                _dmg = 1;

            if (processData.caster.targetToDmg.ContainsKey(_attackTarget.UDID))
            {
                processData.caster.targetToDmg[_attackTarget.UDID] = _dmg;
            }
            else
            {
                processData.caster.targetToDmg.Add(_attackTarget.UDID, _dmg);
            }

            GetPage<UI.CombatUIView>().AddCombatInfo
                (
                    string.Format
                    (
                        ContextConverter.Instance.GetContext(500021),
                        Convert.ToInt32(_finalAttackValue),
                        (CombatUtility.LastAttackRoll + 100).ToString() + "%",
                        Convert.ToInt32(_finalDefenseValue),
                        (CombatUtility.LastDefenseRoll + 100).ToString() + "%"
                    ), OnAttackInfoShown
                );
        }

        private void OnAttackInfoShown()
        {
            //UnityEngine.Debug.Log("DealDamage OnDamageCalculated_Other(" + GetHashCode() + ") m_currentTargetIndex=" + m_currentTargetIndex + ", m_targets.Count=" + m_targets.Count);

            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = processData.caster,
                target = m_targets[m_currentTargetIndex],
                timing = EffectProcesser.TriggerTiming.OnDamageCalculated_Other,
                onEnded = OnDamageCalculated_Other_Ended
            });
        }

        private void OnDamageCalculated_Other_Ended()
        {
            //UnityEngine.Debug.Log("DealDamage OnDamageCalculated_Self(" + GetHashCode() + ") m_currentTargetIndex=" + m_currentTargetIndex + ", m_targets.Count=" + m_targets.Count);

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
            //UnityEngine.Debug.Log("DealDamage OnStartToTakeDamage_Other(" + GetHashCode() + ") m_currentTargetIndex=" + m_currentTargetIndex + ", m_targets.Count=" + m_targets.Count);

            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = m_targets[m_currentTargetIndex],
                target = processData.caster,
                timing = EffectProcesser.TriggerTiming.OnStartToTakeDamage_Other,
                onEnded = OnStartToTakeDamage_Other_Ended
            });
        }

        private void OnStartToTakeDamage_Other_Ended()
        {
            //UnityEngine.Debug.Log("DealDamage OnStartToTakeDamage_Self(" + GetHashCode() + ") m_currentTargetIndex=" + m_currentTargetIndex + ", m_targets.Count=" + m_targets.Count);

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
            //UnityEngine.Debug.Log("DealDamage ShowAttackAnimation(" + GetHashCode() + ")");

            int _id = 0;
            if(processData.refenceSkill != null)
            {
                _id = processData.refenceSkill.skill.ID;
            }

            Dictionary<CombatUnit, int> _targetToDmg = new Dictionary<CombatUnit, int>();
            foreach(KeyValuePair<string, int> udidToDmg in processData.caster.targetToDmg)
            {
                _targetToDmg.Add(CombatUtility.ComabtManager.GetUnitByUDID(udidToDmg.Key), udidToDmg.Value);
            }

            GetPage<UI.CombatUIView>().ShowSkillAnimation(new UI.CombatUIView.SkillAnimationData
            {
                caster = processData.caster,
                targets = m_targets,
                targetToDmg = _targetToDmg,
                skillID = _id,
                onEnded = OnAnimationShown
            });
        }

        private void OnAnimationShown()
        {
            m_currentTargetIndex = -1;
            ApplyDamageToNextTargetShield();
        }

        private void OnShieldDamageApplied()
        {
            for(int i = 0; i < m_targets.Count; i++)
            {
                GetPage<UI.CombatUIView>().AddCombatInfo
                    (
                        string.Format
                        (
                            ContextConverter.Instance.GetContext(500022),
                            processData.caster.targetToDmg[m_targets[i].UDID]
                        ), null
                    );

                if (i == m_targets.Count - 1)
                {
                    GetPage<UI.CombatUIView>().DisplayDamage(new UI.CombatUIView.DisplayDamageData
                    {
                        damageValue = processData.caster.targetToDmg[m_targets[i].UDID],
                        taker = m_targets[i]
                    }, OnDamageShown);
                }
                else
                {
                    GetPage<UI.CombatUIView>().DisplayDamage(new UI.CombatUIView.DisplayDamageData
                    {
                        damageValue = processData.caster.targetToDmg[m_targets[i].UDID],
                        taker = m_targets[i]
                    }, null);
                }
            }
        }

        private void OnDamageShown()
        {
            //UnityEngine.Debug.Log("DealDamage OnDamageShown(" + GetHashCode() + ")");

            for (int i = 0; i < m_targets.Count; i++)
            {
                m_targets[i].lastTakenDamage = processData.caster.targetToDmg[m_targets[i].UDID];
                m_targets[i].HP -= processData.caster.targetToDmg[m_targets[i].UDID];
                m_targets[i].Hatred -= processData.caster.targetToDmg[m_targets[i].UDID];
                processData.caster.Hatred += processData.caster.targetToDmg[m_targets[i].UDID];
            }

            m_currentTargetIndex = -1;
            StartDoAfterApplyDamageEvent();
        }

        private void OnDamageApplied()
        {
            //UnityEngine.Debug.Log("DealDamage OnAttackEnded_Other(" + GetHashCode() + ")");

            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = processData.caster,
                target = processData.caster,
                timing = EffectProcesser.TriggerTiming.OnAttackEnded_Other,
                onEnded = OnAttacked_Other_Ended
            });
        }

        private void OnAttacked_Other_Ended()
        {
            //UnityEngine.Debug.Log("DealDamage OnAttacked_Other_Ended(" + GetHashCode() + ")");

            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = processData.caster,
                target = processData.caster,
                timing = EffectProcesser.TriggerTiming.OnAttackEnded_Self,
                onEnded = End
            });
        }

        private void End()
        {
            //UnityEngine.Debug.Log("DealDamage End(" + GetHashCode() + ")");

            m_onCompleted?.Invoke();
        }

        private void ApplyDamageToNextTargetShield()
        {
            m_currentTargetIndex++;
            //UnityEngine.Debug.Log("DealDamage ApplyDamageToNextTargetShield(" + GetHashCode() + ") m_currentTargetIndex=" + m_currentTargetIndex + ", m_targets.Count=" + m_targets.Count);

            if (m_currentTargetIndex >= m_targets.Count)
            {
                OnShieldDamageApplied();
                return;
            }

            if (m_targets[m_currentTargetIndex].shields.Count > 0)
            {
                if (m_targets[m_currentTargetIndex].shields[0].value > processData.caster.targetToDmg[m_targets[m_currentTargetIndex].UDID])
                {
                    m_targets[m_currentTargetIndex].shields[0].value -= processData.caster.targetToDmg[m_targets[m_currentTargetIndex].UDID];
                    processData.caster.targetToDmg[m_targets[m_currentTargetIndex].UDID] = 0;
                    ApplyDamageToNextTargetShield();
                }
                else
                {
                    processData.caster.targetToDmg[m_targets[m_currentTargetIndex].UDID] -= m_targets[m_currentTargetIndex].shields[0].value;
                    if (m_targets[m_currentTargetIndex].shields[0].triggerSKillID > 0)
                    {
                        EffectProcessManager.GetSkillProcesser(m_targets[m_currentTargetIndex].shields[0].triggerSKillID)
                            .Start(new EffectProcesser.ProcessData
                            {
                                allEffectProcesser = processData.allEffectProcesser,
                                caster = m_targets[m_currentTargetIndex],
                                target = processData.caster,
                                refenceSkill = null,
                                referenceBuff = m_targets[m_currentTargetIndex].GetBuffByBuffID(m_targets[m_currentTargetIndex].shields[0].parentBuffID),
                                skipIfCount = 0,
                                timing = EffectProcesser.TriggerTiming.OnActived,
                                onEnded = OnShieldSkillTriggered
                            });
                    }
                    else
                    {
                        OnShieldSkillTriggered();
                    }
                }
            }
            else
            {
                OnShieldDamageApplied();
            }
        }

        private void OnShieldSkillTriggered()
        {
            //UnityEngine.Debug.Log("DealDamage OnShieldSkillTriggered(" + GetHashCode() + ") m_currentTargetIndex=" + m_currentTargetIndex + ", m_targets.Count=" + m_targets.Count);

            if (m_targets[m_currentTargetIndex].shields[0].parentBuffID == 0)
            {
                m_targets[m_currentTargetIndex].shields.RemoveAt(0);
                OnShieldBuffAmountRemoved();
            }
            else
            {
                m_targets[m_currentTargetIndex].AddBuffAmount(
                    m_targets[m_currentTargetIndex].GetBuffByBuffID(m_targets[m_currentTargetIndex].shields[0].parentBuffID),
                    -1,
                    OnShieldBuffAmountRemoved,
                    OnShieldBuffAmountRemoved);
            }
        }

        private void OnShieldBuffAmountRemoved()
        {
            //UnityEngine.Debug.Log("DealDamage OnShieldBuffAmountRemoved(" + GetHashCode() + ") m_currentTargetIndex=" + m_currentTargetIndex + ", m_targets.Count=" + m_targets.Count);

            m_currentTargetIndex--;
            ApplyDamageToNextTargetShield();
        }

        private void StartDoAfterApplyDamageEvent()
        {
            m_currentTargetIndex++;
            //UnityEngine.Debug.Log("DealDamage OnDamageDealed_Other(" + GetHashCode() + ") m_currentTargetIndex=" + m_currentTargetIndex + ", m_targets.Count=" + m_targets.Count);

            if (m_currentTargetIndex >= m_targets.Count)
            {
                OnDamageApplied();
                return;
            }

            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = processData.caster,
                target = m_targets[m_currentTargetIndex],
                timing = EffectProcesser.TriggerTiming.OnDamageDealed_Other,
                onEnded = OnDamageDealed_Other_Ended
            });
        }

        private void OnDamageDealed_Other_Ended()
        {
            //UnityEngine.Debug.Log("DealDamage OnDamageDealed_Self(" + GetHashCode() + ") m_currentTargetIndex=" + m_currentTargetIndex + ", m_targets.Count=" + m_targets.Count);

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
            //UnityEngine.Debug.Log("DealDamage OnDamageTaken_Other(" + GetHashCode() + ") m_currentTargetIndex=" + m_currentTargetIndex + ", m_targets.Count=" + m_targets.Count);

            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = m_targets[m_currentTargetIndex],
                target = processData.caster,
                timing = EffectProcesser.TriggerTiming.OnDamageTaken_Other,
                onEnded = OnDamageTaken_Other_Ended
            });
        }

        private void OnDamageTaken_Other_Ended()
        {
            //UnityEngine.Debug.Log("DealDamage OnDamageTaken_Self(" + GetHashCode() + ") m_currentTargetIndex=" + m_currentTargetIndex + ", m_targets.Count=" + m_targets.Count);

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
            //UnityEngine.Debug.Log("DealDamage OnBeAttackedEnded_Other(" + GetHashCode() + ") m_currentTargetIndex=" + m_currentTargetIndex + ", m_targets.Count=" + m_targets.Count);

            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = m_targets[m_currentTargetIndex],
                target = processData.caster,
                timing = EffectProcesser.TriggerTiming.OnBeAttackedEnded_Other,
                onEnded = OnBeAttackedEnded_Other_Ended
            });
        }

        private void OnBeAttackedEnded_Other_Ended()
        {
            //UnityEngine.Debug.Log("DealDamage OnBeAttackedEnded_Self(" + GetHashCode() + ") m_currentTargetIndex=" + m_currentTargetIndex + ", m_targets.Count=" + m_targets.Count);

            processData.allEffectProcesser.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = m_targets[m_currentTargetIndex],
                target = processData.caster,
                timing = EffectProcesser.TriggerTiming.OnBeAttackedEnded_Self,
                onEnded = StartDoAfterApplyDamageEvent
            });
        }
    }
}

