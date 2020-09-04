﻿using System.Collections.Generic;

namespace ProjectBS.Combat
{
    public static class CombatUtility
    {
        public class CalculateData
        {
            public CombatUnit caster = null;
            public CombatUnit target = null;
            public string formula = "";
            public bool useRawValue = false;
        }
        public static float Calculate(CalculateData data)
        {
            List<string> _buffer = new List<string> { "" };

            for (int i = 0; i < data.formula.Length; i++)
            {
                if (data.formula[i] == '(')
                {
                    _buffer.Add("");
                    continue;
                }

                if(i == data.formula.Length - 1 && data.formula[i] != ')')
                {
                    _buffer[_buffer.Count - 1] += data.formula[i];
                }

                if (data.formula[i] == ')' || i == data.formula.Length - 1)
                {
                    string _blockResult = (Arithmetic(data, _buffer[_buffer.Count - 1]));

                    if (_buffer.Count > 1)
                    {
                        _buffer[_buffer.Count - 2] += int.Parse(_blockResult);
                        _buffer.RemoveAt(_buffer.Count - 1);
                        continue;
                    }
                    else if (i == data.formula.Length - 1)
                    {
                        break;
                    }
                }

                _buffer[_buffer.Count - 1] += data.formula[i];
            }

            string _resultString = Arithmetic(data, _buffer[_buffer.Count - 1]);

            if (!float.TryParse(_resultString, out float _result))
            {
                _result = (float)GetValueByParaString(data, _resultString);
            }

            return _result;
        }

        public static int GetCombatFieldStatus(string statusName)
        {
            switch(statusName)
            {
                case Keyword.BossUnitCount:
                    {
                        return CombatManager.Instance.GetCampCount(CombatUnit.Camp.Boss);
                    }
                case Keyword.PlayerUnitCount:
                    {
                        return CombatManager.Instance.GetCampCount(CombatUnit.Camp.Player);
                    }
                case Keyword.TurnCount:
                    {
                        return CombatManager.Instance.TurnCount;
                    }
                default:
                    {
                        throw new System.Exception("[CombatUtility][GetCombatFieldStatus] Invaild statusName=" + statusName);
                    }
            }
        }

        public static int GetStatusValue(CombatUnit unit, string statusName, bool useRawValue)
        {
            switch(statusName.Trim())
            {
                case Keyword.MaxHP:
                    {
                        if (useRawValue)
                            return unit.rawMaxHP;
                        else
                            return unit.GetMaxHP();
                    }
                case Keyword.HP:
                    {
                        return unit.HP;
                    }
                case Keyword.Attack:
                    {
                        if (useRawValue)
                            return unit.rawAttack;
                        else 
                            return unit.GetAttack();
                    }
                case Keyword.Defence:
                    {
                        if (useRawValue)
                            return unit.rawDefence;
                        else
                            return unit.GetDefence();
                    }
                case Keyword.Speed:
                    {
                        if (useRawValue)
                            return unit.rawSpeed;
                        else
                            return unit.GetSpeed();
                    }
                case Keyword.SP:
                    {
                        return unit.SP;
                    }
                case Keyword.Hatred:
                    {
                        return unit.hatred;
                    }
                case Keyword.Head:
                    {
                        return unit.head == null ? 0 : unit.head.EquipmentSourceID;
                    }
                case Keyword.Body:
                    {
                        return unit.body == null ? 0 : unit.body.EquipmentSourceID;
                    }
                case Keyword.Hand:
                    {
                        return unit.hand == null ? 0 : unit.hand.EquipmentSourceID;
                    }
                case Keyword.Foot:
                    {
                        return unit.foot == null ? 0 : unit.foot.EquipmentSourceID;
                    }
                case Keyword.TotalDamage:
                    {
                        int _value = 0;
                        List<int> _allDmg = new List<int>(unit.targetToDmg.Values);
                        for(int i = 0; i < _allDmg.Count; i++)
                        {
                            _value += _allDmg[i];
                        }
                        return _value;
                    }
                case Keyword.LastSkill:
                    {
                        return unit.lastSkillID;
                    }
                case Keyword.LastTakenDamage:
                case Keyword.LastDealedDamage:
                default:
                    {
                        throw new System.Exception("[CombatUtility][GetStatusValue] Invaild statusName=" + statusName);
                    }
            }
        }

        private static int GetValueByParaString(CalculateData data, string paraString)
        {
            bool _minus = false;
            if(paraString.StartsWith("-"))
            {
                paraString = paraString.Remove(0, 1);
                _minus = true;
            }

            string[] _getValueData = paraString.Split('.');

            CombatUnit _getValueTarget;
            if (_getValueData[0].Trim() == Keyword.Self
                || _getValueData[0].Trim() == Keyword.Caster)
            {
                _getValueTarget = data.caster;
            }
            else if (_getValueData[0].Trim() == Keyword.Target)
            {
                _getValueTarget = data.target;
            }
            else
            {
                return GetCombatFieldStatus(_getValueData[1]);
            }

            if(_minus)
                return -GetStatusValue(_getValueTarget, _getValueData[1], data.useRawValue);
            else
                return GetStatusValue(_getValueTarget, _getValueData[1], data.useRawValue);
        }

        private static string Arithmetic(CalculateData data, string arithmeticString)
        {
            List<char> _mathString = new List<char>(arithmeticString);
            for (int _mathStringIndex = 0; _mathStringIndex < _mathString.Count; _mathStringIndex++)
            {
                if (_mathString[_mathStringIndex] == '*' || _mathString[_mathStringIndex] == '/')
                {
                    string _varA = "";
                    string _varB = "";
                    int _removeStartIndex = 0;
                    int _removeEndIndex = 0;

                    for (int _recordIndex = _mathStringIndex - 1; _recordIndex >= 0; _recordIndex--)
                    {
                        if (_mathString[_recordIndex] == '+')
                        {
                            _removeStartIndex = _recordIndex + 1;
                            break;
                        }

                        if (_mathString[_recordIndex] == '-')
                        {
                            if (_recordIndex == 0)
                            {
                                _varA = _varA.Insert(0, _mathString[_recordIndex].ToString());
                                continue;
                            }
                            else
                            {
                                if (_mathString[_recordIndex - 1] == '('
                                    || _mathString[_recordIndex - 1] == ')'
                                    || _mathString[_recordIndex - 1] == '+'
                                    || _mathString[_recordIndex - 1] == '*'
                                    || _mathString[_recordIndex - 1] == '/')
                                {
                                    _removeStartIndex = _recordIndex + 1;
                                    break;
                                }
                                else
                                {
                                    _varA = _varA.Insert(0, _mathString[_recordIndex].ToString());
                                    continue;
                                }
                            }
                        }

                        _varA = _varA.Insert(0, _mathString[_recordIndex].ToString());

                        if (_recordIndex == 0)
                        {
                            _removeStartIndex = 0;
                        }
                    }

                    for (int _recordIndex = _mathStringIndex + 1; _recordIndex < _mathString.Count; _recordIndex++)
                    {
                        if (_mathString[_recordIndex] == '+'
                            || _mathString[_recordIndex] == '/'
                            || _mathString[_recordIndex] == '*')
                        {
                            _removeEndIndex = _recordIndex - 1;
                            break;
                        }

                        if (_mathString[_recordIndex] == '-')
                        {
                            if (_mathString[_recordIndex - 1] == '('
                                || _mathString[_recordIndex - 1] == ')'
                                || _mathString[_recordIndex - 1] == '+'
                                || _mathString[_recordIndex - 1] == '*'
                                || _mathString[_recordIndex - 1] == '/')
                            {
                                _removeStartIndex = _recordIndex + 1;
                                break;
                            }
                            else
                            {
                                _varB += _mathString[_recordIndex];
                                continue;
                            }
                        }

                        _varB += _mathString[_recordIndex];

                        if (_recordIndex == _mathString.Count - 1)
                        {
                            _removeEndIndex = _mathString.Count - 1;
                        }
                    }

                    if (!float.TryParse(_varA, out float _varAFloat))
                    {
                        _varAFloat = GetValueByParaString(data, _varA);
                    }

                    if (!float.TryParse(_varB, out float _varBFloat))
                    {
                        _varBFloat = GetValueByParaString(data, _varB);
                    }

                    if (_mathString[_mathStringIndex] == '*')
                    {
                        _mathString.RemoveRange(_removeStartIndex, _removeEndIndex - _removeStartIndex + 1);
                        _mathString.InsertRange(_removeStartIndex, (_varAFloat * _varBFloat).ToString());
                    }
                    else
                    {
                        _mathString.RemoveRange(_removeStartIndex, _removeEndIndex - _removeStartIndex + 1);
                        _mathString.InsertRange(_removeStartIndex, (_varAFloat / _varBFloat).ToString());
                    }

                    _mathStringIndex = 0;
                }
            }

            for (int _mathStringIndex = 0; _mathStringIndex < _mathString.Count; _mathStringIndex++)
            {
                if (_mathString[_mathStringIndex] == '+' || _mathString[_mathStringIndex] == '-')
                {
                    if(_mathStringIndex == 0)
                    {
                        continue;
                    }

                    string _varA = "";
                    string _varB = "";
                    int _removeStartIndex = 0;
                    int _removeEndIndex = 0;

                    for (int _recordIndex = _mathStringIndex - 1; _recordIndex >= 0; _recordIndex--)
                    {
                        if (_mathString[_recordIndex] == '+')
                        {
                            _removeStartIndex = _recordIndex + 1;
                            break;
                        }

                        if(_mathString[_recordIndex] == '-')
                        {
                            if (_recordIndex == 0)
                            {
                                _varA = _varA.Insert(0, _mathString[_recordIndex].ToString());
                                continue;
                            }
                            else
                            {
                                if (_mathString[_recordIndex - 1] == '('
                                    || _mathString[_recordIndex - 1] == ')'
                                    || _mathString[_recordIndex - 1] == '+'
                                    || _mathString[_recordIndex - 1] == '*'
                                    || _mathString[_recordIndex - 1] == '/')
                                {
                                    _removeStartIndex = _recordIndex + 1;
                                    break;
                                }
                                else
                                {
                                    _varA = _varA.Insert(0, _mathString[_recordIndex].ToString());
                                    continue;
                                }
                            }
                        }

                        _varA = _varA.Insert(0, _mathString[_recordIndex].ToString());

                        if (_recordIndex == 0)
                        {
                            _removeStartIndex = 0;
                        }
                    }

                    for (int _recordIndex = _mathStringIndex + 1; _recordIndex < _mathString.Count; _recordIndex++)
                    {
                        if (_mathString[_recordIndex] == '+')
                        {
                            _removeEndIndex = _recordIndex - 1;
                            break;
                        }

                        if (_mathString[_recordIndex] == '-')
                        {
                            if (_mathString[_recordIndex - 1] == '('
                                || _mathString[_recordIndex - 1] == ')'
                                || _mathString[_recordIndex - 1] == '+'
                                || _mathString[_recordIndex - 1] == '*'
                                || _mathString[_recordIndex - 1] == '/')
                            {
                                _removeStartIndex = _recordIndex + 1;
                                break;
                            }
                            else
                            {
                                _varB += _mathString[_recordIndex];
                                continue;
                            }
                        }

                        _varB += _mathString[_recordIndex];

                        if (_recordIndex == _mathString.Count - 1)
                        {
                            _removeEndIndex = _mathString.Count - 1;
                        }
                    }

                    if (!float.TryParse(_varA, out float _varAFloat))
                    {
                        _varAFloat = GetValueByParaString(data, _varA);
                    }

                    if (!float.TryParse(_varB, out float _varBFloat))
                    {
                        _varBFloat = GetValueByParaString(data, _varB);
                    }

                    if (_mathString[_mathStringIndex] == '+')
                    {
                        _mathString.RemoveRange(_removeStartIndex, _removeEndIndex - _removeStartIndex + 1);
                        _mathString.InsertRange(_removeStartIndex, (_varAFloat + _varBFloat).ToString());
                    }
                    else
                    {
                        _mathString.RemoveRange(_removeStartIndex, _removeEndIndex - _removeStartIndex + 1);
                        _mathString.InsertRange(_removeStartIndex, (_varAFloat - _varBFloat).ToString());
                    }

                    _mathStringIndex = 0;
                }
            }

            return new string(_mathString.ToArray());
        }
    }
}
