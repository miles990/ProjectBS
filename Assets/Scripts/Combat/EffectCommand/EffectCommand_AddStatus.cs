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
                id = GetSelectID(),
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
                            m_targets[m_currentTargetIndex].statusAdders.Add(new CombatUnit.StatusAdder
                            {
                                parentBuffID = 0,
                                statusType = m_statusString,
                                valueString = m_valueString
                            });
                        }
                        else
                        {
                            CombatUnit.StatusAdder _adder = m_targets[m_currentTargetIndex].statusAdders
                                .Find(x => x.parentBuffID == processData.referenceBuff.soruceID
                                        && x.statusType == m_statusString);
                            if (_adder == null)
                            {
                                m_targets[m_currentTargetIndex].statusAdders.Add(new CombatUnit.StatusAdder
                                {
                                    parentBuffID = _buff.soruceID,
                                    statusType = m_statusString,
                                    valueString = m_valueString
                                });
                            }
                        }

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
                                    GoNextTarget();
                                    break;
                                }
                            case Keyword.HP:
                                {
                                    if (_add < 0)
                                    {
                                        GetPage<UI.CombatUIView>().DisplayDamage(new UI.CombatUIView.DisplayDamageData
                                        {
                                            taker = m_targets[m_currentTargetIndex],
                                            damageValue = -_add,
                                        }, delegate { OnHPValueInfoShown(_add); });
                                    }
                                    else
                                    {
                                        GetPage<UI.CombatUIView>().DisplayHeal(new UI.CombatUIView.DisplayHealData
                                        {
                                            taker = m_targets[m_currentTargetIndex],
                                            healValue = _add
                                        }, delegate { OnHPValueInfoShown(_add); });
                                    }
                                    break;
                                }
                            case Keyword.SP:
                                {
                                    m_targets[m_currentTargetIndex].SP += _add;
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

        private void OnHPValueInfoShown(int value)
        {
            m_targets[m_currentTargetIndex].HP += value;
            if(value > 0) GetSelf().Hatred += value;

            GoNextTarget();
        }
    }
}
