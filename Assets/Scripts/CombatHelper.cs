using System.Collections;
using System.Collections.Generic;
using System;
using ProjectBS.Data;

namespace ProjectBS.Combat
{
    public static class CombatUtility
    {
        public static void ProcessAllEquipmentEffects(List<CombatManager.CombatUnit> m_units, EffectProcesser.TriggerTiming timing, Action onEnded)
        {
            List<EffectProcesser> _allEffectProcesser = new List<EffectProcesser>();

            for (int _unitIndex = 0; _unitIndex < m_units.Count; _unitIndex++)
            {
                CreateEffectProcesser(m_units[_unitIndex].foot, ref _allEffectProcesser);
                CreateEffectProcesser(m_units[_unitIndex].hand, ref _allEffectProcesser);
                CreateEffectProcesser(m_units[_unitIndex].head, ref _allEffectProcesser);
                CreateEffectProcesser(m_units[_unitIndex].body, ref _allEffectProcesser);
            }

            if (_allEffectProcesser.Count > 0)
            {
                StartEffectProcesser(_allEffectProcesser, timing, onEnded);
            }
            else
            {
                onEnded?.Invoke();
            }
        }

        private static void StartEffectProcesser(List<EffectProcesser> list, EffectProcesser.TriggerTiming timing, Action onEnded)
        {
            list[0].Start(timing, delegate
            {
                list.RemoveAt(0);
                if (list.Count > 0)
                {
                    StartEffectProcesser(list, timing, onEnded);
                }
                else
                {
                    onEnded?.Invoke();
                }
            });
        }

        private static void CreateEffectProcesser(OwningEquipmentData equipmentData, ref List<EffectProcesser> container)
        {
            if (equipmentData == null)
            {
                return;
            }

            string[] _effectIDs = equipmentData.EffectIDs.Split('$');
            for (int _effectIDIndex = 0; _effectIDIndex < _effectIDs.Length; _effectIDIndex++)
            {
                SkillEffectData _effectData = GameDataLoader.Instance.GetSkillEffect(int.Parse(_effectIDs[_effectIDIndex]));
                container.Add(new EffectProcesser(_effectData.Command));
            }
        }

        public static void SetTarget(CombatManager.CombatUnit caster, string targetCommand, Action<List<CombatManager.CombatUnit>> onSet)
        {
            List<CombatManager.CombatUnit> _targetsList = new List<CombatManager.CombatUnit>();
            if (targetCommand.Contains("Self"))
            {
                _targetsList.Add(caster);
            }

            if (targetCommand.Contains("SelectSameSide"))
            {

            }

            if (targetCommand.Contains("SelectOpponent"))
            {

            }

            if (targetCommand.Contains("SelectAllSide"))
            {

            }

            if (targetCommand.Contains("AllSameSide"))
            {

            }

            if (targetCommand.Contains("AllOpponent"))
            {

            }

            if (targetCommand.Contains("AllBattleField"))
            {

            }

            if (targetCommand.Contains("RandomAll"))
            {

            }

            if (targetCommand.Contains("RandomSameSide"))
            {

            }

            if (targetCommand.Contains("RandomOpponent"))
            {

            }

            onSet?.Invoke(_targetsList);
        }

        public static int GetValueFromUnit(CombatManager.CombatUnit unit, string statusString)
        {
            switch (statusString)
            {
                case "HP":
                    {
                        return unit.GetHP();
                    }
                case "SP":
                    {
                        return unit.GetSP();
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
                default:
                    {
                        throw new NotImplementedException("Invaild Status String:" + statusString);
                    }
            }
        }

        public static int GetValue(CombatManager.CombatUnit caster, CombatManager.CombatUnit target, string valueString)
        {
            int _value = 0;
            if (int.TryParse(valueString, out _value))
            {
                return _value;
            }
            else
            {
                string[] _valueStringPart = valueString.Split('.');

                switch (_valueStringPart[0])
                {
                    case "Self":
                        {
                            return GetValueFromUnit(caster, _valueStringPart[1]);
                        }
                    case "Target":
                        {
                            return GetValueFromUnit(target, _valueStringPart[1]);
                        }
                    default:
                        {
                            throw new NotImplementedException("Invaild GetValueTarget String:" + _valueStringPart[0]);
                        }
                }
            }
        }
    }
}

