﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.Combat
{
    public static class CombatUtility
    {
        public class CalculateData
        {
            public CombatUnit caster = null;
            public CombatUnit target = null;
            public string formula = "";
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

            return float.Parse(Arithmetic(data, _buffer[_buffer.Count - 1]));
        }

        public static int GetStatusValue(CombatUnit unit, string statusName)
        {
            switch(statusName)
            {
                case "MaxHP":
                    {
                        return unit.GetMaxHP();
                    }
                case "HP":
                    {
                        return unit.HP;
                    }
                case "Attack":
                    {
                        return unit.GetAttack();
                    }
                case "Defence":
                    {
                        return unit.GetDefence();
                    }
                case "Speed":
                    {
                        return unit.GetSpeed();
                    }
                case "SP":
                    {
                        return unit.SP;
                    }
                default:
                    {
                        return 0;
                    }
            }
        }

        private static int GetValueByParaString(CalculateData data, string paraString)
        {
            string[] _getValueData = paraString.Split('.');
            CombatUnit _getValueTarget = null;

            if (_getValueData[0] == "Self")
            {
                _getValueTarget = data.caster;
            }
            else if (_getValueData[0] == "Target")
            {
                _getValueTarget = data.target;
            }
            else
            {
                return 0;
            }

            return GetStatusValue(_getValueTarget, _getValueData[1]);
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
                        if (_mathString[_recordIndex] == '+'
                            || _mathString[_recordIndex] == '-')
                        {
                            _removeStartIndex = _recordIndex + 1;
                            break;
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
                            || _mathString[_recordIndex] == '-'
                            || _mathString[_recordIndex] == '/'
                            || _mathString[_recordIndex] == '*')
                        {
                            _removeEndIndex = _recordIndex - 1;
                            break;
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
                    string _varA = "";
                    string _varB = "";
                    int _removeStartIndex = 0;
                    int _removeEndIndex = 0;

                    for (int _recordIndex = _mathStringIndex - 1; _recordIndex >= 0; _recordIndex--)
                    {
                        if (_mathString[_recordIndex] == '+'
                            || _mathString[_recordIndex] == '-')
                        {
                            _removeStartIndex = _recordIndex + 1;
                            break;
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
                            || _mathString[_recordIndex] == '-')
                        {
                            _removeEndIndex = _recordIndex - 1;
                            break;
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