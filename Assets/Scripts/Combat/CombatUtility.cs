using System.Collections.Generic;
using KahaGameCore.Static;

namespace ProjectBS.Combat
{
    public static class CombatUtility
    {
        public static CombatManagerBase ComabtManager
        {
            get
            {
                if(m_currentCombatManager == null)
                {
                    throw new System.Exception("[CombatUtility][CurrentComabtManager get] CurrentComabtManager == null");
                }
                return m_currentCombatManager;
            }
        }
        private static CombatManagerBase m_currentCombatManager = null;
        public static int LastAttackRoll = 0;
        public static int LastDefenseRoll = 0;

        public static void SetCombatManager(CombatManagerBase combatManager)
        {
            if(m_currentCombatManager != null)
            {
                throw new System.Exception("[CombatUtility][SetCombatManager] Is trying to overwrite CombatUtility.currentComabtManager");
            }
            m_currentCombatManager = combatManager;
        }

        public static void EndCombatProcess(CombatManagerBase combatManager)
        {
            if(m_currentCombatManager == null)
            {
                return;
            }

            if(m_currentCombatManager != combatManager)
            {
                throw new System.Exception("[CombatUtility][EndCombat] Is trying to end combat with different combar manager");
            }

            m_currentCombatManager = null;
        }

        public class CalculateData
        {
            public CombatUnit caster = null;
            public CombatUnit target = null;
            public CombatUnit.Buff referenceBuff = null;
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
                    string _blockResult = _buffer[_buffer.Count - 1];

                    if (_buffer.Count > 1)
                    {
                        if(_blockResult.Contains(",")) // means this block is a value command
                        {
                            string _commandBuffer = ""; // start get command
                            for(int j = _buffer[_buffer.Count - 2].Length - 1; j >= 0; j--)
                            {
                                if(_buffer[_buffer.Count - 2][j] != '+'
                                    && _buffer[_buffer.Count - 2][j] != '-'
                                    && _buffer[_buffer.Count - 2][j] != '*'
                                    && _buffer[_buffer.Count - 2][j] != '/'
                                    && _buffer[_buffer.Count - 2][j] != '('
                                    && _buffer[_buffer.Count - 2][j] != ')')
                                {
                                    _commandBuffer = _commandBuffer.Insert(0, _buffer[_buffer.Count - 2][j].ToString());
                                }
                                else
                                {
                                    break;
                                }
                            }

                            string _para = _buffer[_buffer.Count - 1];
                            int _commandResult = GetValueByCommand(data, _commandBuffer, _para);

                            _buffer[_buffer.Count - 2] = _buffer[_buffer.Count - 2].Replace(_commandBuffer, "");
                            _buffer.RemoveAt(_buffer.Count - 1);

                            if (_buffer.Count > 0)
                            {
                                _buffer[_buffer.Count - 1] += _commandResult;
                            }
                            else
                            {
                                _buffer.Add(_commandResult.ToString());
                            }
                            continue;
                        }

                        if(int.TryParse(_blockResult, out int _blockValue))
                        {
                            _buffer[_buffer.Count - 2] += _blockValue;
                        }
                        else
                        {
                            _buffer[_buffer.Count - 2] += Arithmetic(data, _blockResult);
                        }

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
                case Keyword.TurnCount:
                    {
                        return ComabtManager.TurnCount;
                    }
                case Keyword.LastAttackRoll:
                    {
                        return LastAttackRoll;
                    }
                case Keyword.LastDefenseRoll:
                    {
                        return LastDefenseRoll;
                    }
                case Keyword.MyUnitCount:
                    {
                        List<CombatUnit> _allUnits = m_currentCombatManager.AllUnit;
                        int _count = 0;
                        for(int i = 0; i < _allUnits.Count; i++)
                        {
                            if(_allUnits[i].camp == 0)
                            {
                                _count++;
                            }
                        }

                        return _count;
                    }
                case Keyword.OpponentUnitCount:
                    {
                        List<CombatUnit> _allUnits = m_currentCombatManager.AllUnit;
                        int _count = 0;
                        for (int i = 0; i < _allUnits.Count; i++)
                        {
                            if (_allUnits[i].camp == 1)
                            {
                                _count++;
                            }
                        }

                        return _count;
                    }
                default:
                    {
                        throw new System.Exception("[CombatUtility][GetCombatFieldStatus] Invaild statusName=" + statusName);
                    }
            }
        }

        public static int GetStatusValue(CombatUnit unit, string statusName, bool useRawValue)
        {
            if (unit == null) throw new System.Exception("[CombatUtility][GetStatusValue] unit == null");

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
                case Keyword.Defense:
                    {
                        if (useRawValue)
                            return unit.rawDefense;
                        else
                            return unit.GetDefense();
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
                        return unit.Hatred;
                    }
                case Keyword.Head:
                    {
                        return string.IsNullOrEmpty(unit.head) ? 0 : PlayerManager.Instance.GetEquipmentByUDID(unit.head).EquipmentSourceID;
                    }
                case Keyword.Body:
                    {
                        return string.IsNullOrEmpty(unit.body) ? 0 : PlayerManager.Instance.GetEquipmentByUDID(unit.body).EquipmentSourceID;
                    }
                case Keyword.Hand:
                    {
                        return string.IsNullOrEmpty(unit.hand) ? 0 : PlayerManager.Instance.GetEquipmentByUDID(unit.hand).EquipmentSourceID;
                    }
                case Keyword.Foot:
                    {
                        return string.IsNullOrEmpty(unit.foot) ? 0 : PlayerManager.Instance.GetEquipmentByUDID(unit.foot).EquipmentSourceID;
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
                    {
                        return unit.lastTakenDamage;
                    }
                case Keyword.LastDealedDamage:
                    {
                        int _total = 0;
                        foreach(KeyValuePair<string, int> kvp in unit.targetToDmg)
                        {
                            _total += kvp.Value;
                        }
                        return _total;
                    }
                case Keyword.Camp:
                    {
                        return unit.camp;
                    }
                case Keyword.ActionIndex:
                    {
                        return unit.actionIndex;
                    }
                default:
                    {
                        throw new System.Exception("[CombatUtility][GetStatusValue] Invaild statusName=" + statusName);
                    }
            }
        }

        public static void RemoveEffect(CombatUnit unit, int buffID)
        {
            for (int i = 0; i < unit.statusAdders.Count; i++)
            {
                if (unit.statusAdders[i].parentBuffID != 0
                    && unit.statusAdders[i].parentBuffID == buffID)
                {
                    unit.statusAdders.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < unit.statusAddLockers.Count; i++)
            {
                if (unit.statusAddLockers[i].parentBuffID != 0
                    && unit.statusAddLockers[i].parentBuffID == buffID)
                {
                    unit.statusAddLockers.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < unit.actionSkipers.Count; i++)
            {
                if (unit.actionSkipers[i].parentBuffID != 0
                    && unit.actionSkipers[i].parentBuffID == buffID)
                {
                    unit.actionSkipers.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < unit.checkSPSkipers.Count; i++)
            {
                if (unit.checkSPSkipers[i].parentBuffID != 0
                    && unit.checkSPSkipers[i].parentBuffID == buffID)
                {
                    unit.checkSPSkipers.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < unit.shields.Count; i++)
            {
                if(unit.shields[i].parentBuffID != 0
                    && unit.shields[i].parentBuffID == buffID)
                {
                    unit.shields.RemoveAt(i);
                    i--;
                }
            }
        }

        public static float GetHatredPersent(CombatUnit unit)
        {
            List<CombatUnit> _allUnits = ComabtManager.AllUnit;
            if (_allUnits.Contains(unit))
            {
                int _totalHaterd = 0;
                for (int i = 0; i < _allUnits.Count; i++)
                {
                    if (_allUnits[i].camp == unit.camp)
                    {
                        _totalHaterd += _allUnits[i].Hatred;
                    }
                }

                return (float)unit.Hatred / (float)_totalHaterd;
            }
            else
            {
                return 0f;
            }
        }

        private static int GetValueByCommand(CalculateData data, string command, string paraString)
        {
            bool _minus = false;
            if (command.StartsWith("-"))
            {
                command = command.Remove(0, 1);
                _minus = true;
            }
            switch (command)
            {
                case Keyword.Random:
                    {
                        string[] _varParts = paraString.Split(',');
                        int _min = System.Convert.ToInt32(float.Parse(Arithmetic(data, _varParts[0])));
                        int _max = System.Convert.ToInt32(float.Parse(Arithmetic(data, _varParts[1])));
                        if (_minus)
                            return -UnityEngine.Random.Range(_min, _max);
                        else
                            return UnityEngine.Random.Range(_min, _max);
                    }
                case Keyword.Buff:
                    {
                        string[] _varParts = paraString.Split(',');
                        CombatUnit _getValueTarget;
                        switch (_varParts[0])
                        {
                            case Keyword.Caster:
                                {
                                    _getValueTarget = data.caster;
                                    break;
                                }
                            case Keyword.Target:
                                {
                                    _getValueTarget = data.target;
                                    break;
                                }
                            case Keyword.Owner:
                                {
                                    _getValueTarget = ComabtManager.GetUnitByUDID(data.referenceBuff.ownerUnitUDID);
                                    break;
                                }
                            case Keyword.From:
                                {
                                    _getValueTarget = ComabtManager.GetUnitByUDID(data.referenceBuff.fromUnitUDID);
                                    break;
                                }
                            default:
                                {
                                    throw new System.Exception("[CombatUtility][GetValueByCommand] Invaild target when getting buff info:" + _varParts[0]);
                                }
                        }

                        int _buffID = int.Parse(_varParts[1]);
                        CombatUnit.Buff _buff = _getValueTarget.GetBuffByBuffEffectID(_buffID);
                        if (_buff == null)
                        {
                            return 0;
                        }
                        else
                        {
                            switch(_varParts[2])
                            {
                                case Keyword.StackCount:
                                    {
                                        return _buff.stackCount;
                                    }
                                case Keyword.Time:
                                    {
                                        return _buff.remainingTime;
                                    }
                                case Keyword.SkillEffectID:
                                    {
                                        return _buff.soruceID;
                                    }
                                default:
                                    {
                                        throw new System.Exception("[CombatUtility][GetValueByCommand] Invaild value type when getting buff info:" + _varParts[2]);
                                    }
                            }
                        }
                    }
                default:
                    {
                        throw new System.Exception("[CombatUtility][GetValueByCommand] Invaild command=" + command);
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
            CombatUnit _getValueTarget = data.caster;

            switch (_getValueData[0].Trim())
            {
                case Keyword.Self:
                case Keyword.Caster:
                    {
                        _getValueTarget = data.caster;
                        break;
                    }
                case Keyword.Target:
                    {
                        _getValueTarget = data.target;
                        break;
                    }
                case Keyword.CombatField:
                    {
                        if (_minus)
                            return -GetCombatFieldStatus(_getValueData[1]);
                        else
                            return GetCombatFieldStatus(_getValueData[1]);
                    }
                case Keyword.Random:
                    {
                        string _value = paraString.RemoveBlankCharacters().Replace(Keyword.Random, "");
                        _value = paraString.Replace("(", "");
                        _value = paraString.Replace(")", "");
                        string[] _valueParts = _value.Split(',');
                        int _min = int.Parse(_valueParts[0]);
                        int _max = int.Parse(_valueParts[1]);

                        return UnityEngine.Random.Range(_min, _max);
                    }
                default:
                    {
                        throw new System.Exception("[CombatUtility][GetValueByParaString] Invaild target=" + _getValueData[0]);
                    }
            }

            if (_minus)
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

        public static CombatUnit CreateUnit(Data.OwningCharacterData character, int camp)
        {
            CombatUnit _newUnit = new CombatUnit
            {
                UDID = character.UDID,
                ai = "",
                rawAttack = character.Attack,
                camp = camp,
                rawDefense = character.Defense,
                rawMaxHP = character.HP,
                name = character.Name,
                icon = character.GetIcon(),
                SP = character.SP,
                rawSpeed = character.Speed,
                Hatred = 1,
                //sprite = GameDataLoader.Instance.GetSprite(character.CharacterSpriteID),
                body = character.Equipment_UDID_Body,
                foot = character.Equipment_UDID_Foot,
                hand = character.Equipment_UDID_Hand,
                head = character.Equipment_UDID_Head,
                statusAdders = new List<CombatUnit.StatusAdder>()
            };

            _newUnit.skills[0] = character.SkillSlot_0;
            _newUnit.skills[1] = character.SkillSlot_1;
            _newUnit.skills[2] = character.SkillSlot_2;
            _newUnit.skills[3] = character.SkillSlot_3;

            return _newUnit;
        }

        public static CombatUnit CreateUnit(Data.BossData boss)
        {
            CombatUnit _boss = new CombatUnit
            {
                UDID = System.Guid.NewGuid().ToString(),
                ai = boss.AI,
                rawAttack = boss.Attack,
                camp = 1,
                rawDefense = boss.Defense,
                rawMaxHP = boss.HP,
                HP = boss.HP,
                name = boss.GetName(),
                icon = boss.GetIcon(),
                SP = boss.SP,
                rawSpeed = boss.Speed,
                Hatred = 1,
                //sprite = GameDataLoader.Instance.GetSprite(boss.CharacterSpriteID),
                body = null,
                foot = null,
                hand = null,
                head = null,
                statusAdders = new List<CombatUnit.StatusAdder>()
            };

            if (!string.IsNullOrEmpty(boss.BuffIDs))
            {
                string[] _buffIDs = boss.BuffIDs.Split(';');
                for (int i = 0; i < _buffIDs.Length; i++)
                {
                    _boss.AddBuff(new CombatUnit.Buff
                    {
                        soruceID = int.Parse(_buffIDs[i]),
                        fromUnitUDID = _boss.UDID,
                        ownerUnitUDID = _boss.UDID,
                        remainingTime = -1,
                        stackCount = 1
                    });
                }
            }

            return _boss;
        }

    }
}
