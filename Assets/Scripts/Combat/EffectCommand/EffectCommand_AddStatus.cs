﻿using System;
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
                case Keyword.Defense:
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

                        GetPage<UI.CombatUIView>().RefreshAllInfo();
                        GoNextTarget();
                        break;
                    }
                default:
                    {
                        float _value = CombatUtility.Calculate(
                            new CombatUtility.CalculateData
                            {
                                caster = processData.caster,
                                target = m_targets[m_currentTargetIndex],
                                referenceBuff = processData.referenceBuff,
                                formula = m_valueString,
                                useRawValue = true
                            });
                        int _add = Convert.ToInt32(_value);

                        switch (m_statusString)
                        {
                            case Keyword.Hatred:
                                {
                                    m_targets[m_currentTargetIndex].Hatred += _add;

                                    GetPage<UI.CombatUIView>().RefreshAllInfo();
                                    GoNextTarget();
                                    break;
                                }
                            case Keyword.HP:
                                {
                                    if (_add < 0)
                                    {
                                        m_targets[m_currentTargetIndex].HP += _add;
                                        GetPage<UI.CombatUIView>().RefreshAllInfo();
                                        GetPage<UI.CombatUIView>().DisplayDamage(new UI.CombatUIView.DisplayDamageData
                                        {
                                            taker = m_targets[m_currentTargetIndex],
                                            damageValue = -_add,
                                        }, GoNextTarget);
                                    }
                                    else
                                    {
                                        GetSelf().Hatred += _add;
                                        m_targets[m_currentTargetIndex].HP += _add;
                                        GetPage<UI.CombatUIView>().RefreshAllInfo();
                                        GetPage<UI.CombatUIView>().DisplayHeal(new UI.CombatUIView.DisplayHealData
                                        {
                                            taker = m_targets[m_currentTargetIndex],
                                            healValue = _add
                                        }, GoNextTarget);
                                    }
                                    break;
                                }
                            case Keyword.SP:
                                {
                                    m_targets[m_currentTargetIndex].SP += _add;
                                    GetPage<UI.CombatUIView>().RefreshAllInfo();
                                    GoNextTarget();
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
        }
    }
}
