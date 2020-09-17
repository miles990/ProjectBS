using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_AddStatus : EffectCommandBase
    {
        private Action m_onCompleted = null;
        private string m_statusString = "";
        private string m_valueString = "";

        private List<CombatUnit> m_targets = null;
        private int m_currentTargetIndex = -1;

        public override void Process(string[] vars, Action onCompleted)
        {
            m_onCompleted = onCompleted;
            m_statusString = vars[1].Trim();
            m_valueString = vars[2].Trim();

            CombatTargetSelecter.Instance.StartSelect(new CombatTargetSelecter.SelectTargetData
            {
                attacker = GetSelf(),
                commandString = vars[0],
                onSelected = OnSelected
            });
        }

        private void OnSelected(List<CombatUnit> targets)
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

            switch (m_statusString)
            {
                case Keyword.Attack:
                case Keyword.Defence:
                case Keyword.MaxHP:
                case Keyword.Speed:
                    {
                        CombatUnit.Buff _buff = processData.referenceBuff;
                        if(_buff == null)
                        {
                            _buff = new CombatUnit.Buff
                            {
                                effectID = 0,
                                from = GetSelf(),
                                owner = m_targets[m_currentTargetIndex],
                                remainingTime = -1,
                                stackCount = 1
                            };
                        }
                        m_targets[m_currentTargetIndex].statusAdders.Add(new CombatUnit.StatusAdder
                        {
                            parentBuff = _buff,
                            statusType = m_statusString,
                            valueString = m_valueString
                        });
                        break;
                    }
                default:
                    {
                        UnityEngine.Debug.LogWarning("Add status formula=" + m_valueString);
                        float _value = CombatUtility.Calculate(
                            new CombatUtility.CalculateData
                            {
                                caster = processData.caster,
                                target = m_targets[m_currentTargetIndex],
                                formula = m_valueString,
                                useRawValue = true
                            });
                        int _add = Convert.ToInt32(_value);
                        UnityEngine.Debug.LogWarning("Add status _add=" + _add);
                        UnityEngine.Debug.LogWarning("Add status status=" + m_statusString);

                        switch (m_statusString)
                        {
                            case Keyword.Hatred:
                                {
                                    if (m_targets[m_currentTargetIndex].lockAddHatred && _add > 0)
                                    {
                                        break;
                                    }

                                    m_targets[m_currentTargetIndex].hatred += _add;
                                    break;
                                }
                            case Keyword.HP:
                                {
                                    if (_add < 0)
                                    {
                                        CombatManager.Instance.ShowDamage(new UI.CombatUIView.DisplayDamageData
                                        {
                                            takerName = m_targets[m_currentTargetIndex].name,
                                            damageValue = -_add,
                                        });
                                    }
                                    else
                                    {
                                        if(m_targets[m_currentTargetIndex].lockAddHP)
                                        {
                                            break;
                                        }

                                        CombatManager.Instance.ShowHeal(new UI.CombatUIView.DisplayHealData
                                        {
                                            takerName = m_targets[m_currentTargetIndex].name,
                                            healValue = _add
                                        });
                                    }
                                    GetSelf().hatred += _add;
                                    m_targets[m_currentTargetIndex].HP += _add;
                                    break;
                                }
                            case Keyword.SP:
                                {
                                    if (m_targets[m_currentTargetIndex].lockAddSP && _add > 0)
                                    {
                                        break;
                                    }

                                    m_targets[m_currentTargetIndex].SP += _add;
                                    break;
                                }
                            default:
                                {
                                    throw new Exception("[EffectCommand_AddStatus][Process] Invaild status=" + m_statusString);
                                }
                        }

                        break;
                    }
            }

            GoNextTarget();
        }
    }
}
