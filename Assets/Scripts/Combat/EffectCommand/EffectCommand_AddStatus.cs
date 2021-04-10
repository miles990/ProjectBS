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

            AddSkillOrEffectInfo();

            CombatTargetSelecter.Instance.StartSelect(new CombatTargetSelecter.SelectTargetData
            {
                selectID = CombatTargetSelecter.Instance.GetSelectID(processData),
                attacker = GetSelf(),
                currentEffectedTarget = processData.target,
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
                for(int i = 0; i < m_targets.Count; i++)
                {
                    GetPage<UI.CombatUIView>().ForceDisableDamageAnimation(m_targets[i]);
                }
                m_onCompleted?.Invoke();
                return;
            }

            string _statusInfoName = "";
            bool _showInfo = false;
            switch (m_statusString)
            {
                case Keyword.Attack:
                    {
                        _statusInfoName = ContextConverter.Instance.GetContext(1000020);
                        break;
                    }
                case Keyword.Defense:
                    {
                        _statusInfoName = ContextConverter.Instance.GetContext(1000021);
                        break;
                    }
                case Keyword.MaxHP:
                    {
                        _statusInfoName = ContextConverter.Instance.GetContext(1000018);
                        break;
                    }
                case Keyword.Speed:
                    {
                        _statusInfoName = ContextConverter.Instance.GetContext(1000022);
                        break;
                    }
                case Keyword.Hatred:
                    {
                        _statusInfoName = ContextConverter.Instance.GetContext(1000023);
                        break;
                    }
                case Keyword.HP:
                    {
                        _statusInfoName = ContextConverter.Instance.GetContext(1000019);
                        break;
                    }
                case Keyword.SP:
                    {
                        _statusInfoName = ContextConverter.Instance.GetContext(1000024);
                        break;
                    }
            }

            float _result = CombatUtility.Calculate(new CombatUtility.CalculateData
            {
                caster = processData.caster,
                target = m_targets[m_currentTargetIndex],
                processData = new EffectProcesser.ProcessData(),
                formula = m_valueString,
                useRawValue = true
            });

            switch (m_statusString)
            {
                case Keyword.Attack:
                case Keyword.Defense:
                case Keyword.MaxHP:
                case Keyword.Speed:
                    {
                        int _previousMaxHP = m_targets[m_currentTargetIndex].GetMaxHP();

                        CombatUnit.Buff _buff = processData.referenceBuff;
                        if(_buff == null)
                        {
                            m_targets[m_currentTargetIndex].statusAdders.Add(new CombatUnit.StatusAdder
                            {
                                parentBuffID = 0,
                                statusType = m_statusString,
                                valueString = m_valueString
                            });
                            _showInfo = true;
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
                                _showInfo = true;
                            }
                        }

                        int _currentMaxHP = m_targets[m_currentTargetIndex].GetMaxHP();
                        if (_currentMaxHP != _previousMaxHP)
                        {
                            m_targets[m_currentTargetIndex].HP += (_currentMaxHP - _previousMaxHP);
                        }

                        break;
                    }
                default:
                    {
                        float _value = CombatUtility.Calculate(
                            new CombatUtility.CalculateData
                            {
                                caster = processData.caster,
                                target = m_targets[m_currentTargetIndex],
                                processData = processData,
                                formula = m_valueString,
                                useRawValue = true
                            });
                        int _add = Convert.ToInt32(_value);

                        switch (m_statusString)
                        {
                            case Keyword.Hatred:
                                {
                                    m_targets[m_currentTargetIndex].Hatred += _add;
                                    break;
                                }
                            case Keyword.HP:
                                {
                                    if (_add < 0)
                                    {
                                        GetPage<UI.CombatUIView>().SimpleDisplayDamage(new UI.CombatUIView.DisplayDamageData
                                        {
                                            taker = m_targets[m_currentTargetIndex],
                                            damageValue = -_add,
                                        }, null);
                                        OnHPValueInfoShown(_add);
                                    }
                                    else
                                    {
                                        GetPage<UI.CombatUIView>().DisplayHeal(new UI.CombatUIView.DisplayHealData
                                        {
                                            taker = m_targets[m_currentTargetIndex],
                                            healValue = _add
                                        }, null);
                                        OnHPValueInfoShown(_add);
                                    }
                                    break;
                                }
                            case Keyword.SP:
                                {
                                    m_targets[m_currentTargetIndex].SP += _add;
                                    break;
                                }
                            default:
                                {
                                    throw new Exception("[EffectCommand_AddStatus][Process] Invaild status=" + m_statusString);
                                }
                        }
                        _showInfo = true;
                        break;
                    }
            }

            if (_showInfo)
                AddInfo(_result, _statusInfoName);
            else
                GoNextTarget();
        }

        private void AddInfo(float _result, string _statusInfoName)
        {
            int _resultIntValue = Convert.ToInt32(_result);

            if (_resultIntValue >= 0)
            {
                GetPage<UI.CombatUIView>().AddCombatInfo
                    (
                        string.Format
                        (
                            ContextConverter.Instance.GetContext(500016),
                            m_targets[m_currentTargetIndex].name,
                            _resultIntValue,
                            _statusInfoName
                        ), GoNextTarget
                    );
            }
            else
            {
                GetPage<UI.CombatUIView>().AddCombatInfo
                    (
                        string.Format
                        (
                            ContextConverter.Instance.GetContext(500017),
                            m_targets[m_currentTargetIndex].name,
                            _resultIntValue * -1,
                            _statusInfoName
                        ), GoNextTarget
                    );
            }
        }

        private void OnHPValueInfoShown(int value)
        {
            m_targets[m_currentTargetIndex].HP += value;
            if(value > 0) GetSelf().Hatred += value;
        }
    }
}
