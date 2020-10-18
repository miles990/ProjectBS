using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectBS.Data;
using KahaGameCore.Static;

namespace ProjectBS
{
    public static class EquipmentUtility
    {
        public static OwningEquipmentData CreateNewEquipment(int sourceID)
        {
            RawEquipmentData _source = GameDataManager.GetGameData<RawEquipmentData>(sourceID);

            OwningEquipmentData _newEquipment = new OwningEquipmentData
            {
                EffectIDs = _source.SkillEffectIDs,
                EquipmentSourceID = _source.ID,
                Exp = 0,
                Level = 1,
                UDID = System.Guid.NewGuid().ToString()
            };

            SetAility(_newEquipment);

            return _newEquipment;
        }

        public static void SetLevel(OwningEquipmentData owningEquipmentData, int targetLevel)
        {
            while (owningEquipmentData.Level < targetLevel)
            {
                LevelUp(owningEquipmentData);
            }
        }

        public static void TryAddOneLevel(OwningEquipmentData equipment)
        {
            ExpData _expData = GameDataManager.GetGameData<ExpData>(equipment.Level);
            int _needExp = _expData.Require - equipment.Exp;
            if (PlayerManager.Instance.Player.OwnExp >= _needExp)
            {
                PlayerManager.Instance.Player.OwnExp -= _needExp;
                AddExp(equipment, _needExp);
            }
            else
            {
                equipment.Exp += PlayerManager.Instance.Player.OwnExp;
                PlayerManager.Instance.Player.OwnExp = 0;
            }
        }

        public static void AddExp(OwningEquipmentData owningEquipmentData, int addExp)
        {
            owningEquipmentData.Exp += addExp;
            while (GameDataManager.GetGameData<ExpData>(owningEquipmentData.Level).Require != -1 &&
                  owningEquipmentData.Exp - GameDataManager.GetGameData<ExpData>(owningEquipmentData.Level).Require >= 0)
            {
                owningEquipmentData.Exp -= GameDataManager.GetGameData<ExpData>(owningEquipmentData.Level).Require;
                LevelUp(owningEquipmentData);
            }
        }

        private static void LevelUp(OwningEquipmentData owningEquipmentData)
        {
            owningEquipmentData.Level++;
            SetAility(owningEquipmentData);
        }

        private static void SetAility(OwningEquipmentData owningEquipmentData)
        {
            RawEquipmentData _source = GameDataManager.GetGameData<RawEquipmentData>(owningEquipmentData.EquipmentSourceID);

            List<string> _abilityPool = new List<string>(_source.AbilityPool.RemoveBlankCharacters().Split(';'));
            _abilityPool.Remove("");
            string _random = _abilityPool[Random.Range(0, _abilityPool.Count)];
            string[] _ranAbiParts = _random.Split('&');
            for (int i = 0; i < _ranAbiParts.Length; i++)
            {
                if (string.IsNullOrEmpty(_ranAbiParts[i]))
                    continue;

                string[] _abi = _ranAbiParts[i].Split(':');
                string[] _abiRandomValue = _abi[1].Split('~');
                int _value = Random.Range(int.Parse(_abiRandomValue[0]), int.Parse(_abiRandomValue[1]) + 1);

                switch (_abi[0].Trim())
                {
                    case Keyword.Attack:
                        {
                            owningEquipmentData.Attack += _value;
                            break;
                        }
                    case Keyword.Defense:
                        {
                            owningEquipmentData.Defense += _value;
                            break;
                        }
                    case Keyword.Speed:
                        {
                            owningEquipmentData.Speed += _value;
                            break;
                        }
                    case Keyword.HP:
                        {
                            owningEquipmentData.HP += _value;
                            break;
                        }
                    case Keyword.SP:
                        {
                            owningEquipmentData.SP += _value;
                            break;
                        }
                    default:
                        throw new System.Exception("[EquipmentUtility][SetAility] Invaild abi key=" + _abi[0].Trim());
                }
            }
        }
    }
}
