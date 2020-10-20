﻿using ProjectBS.Data;
using System.Collections.Generic;
using KahaGameCore.Static;

using Random = UnityEngine.Random;

namespace ProjectBS
{
    public static class CharacterUtility
    {
        private static bool m_inited = false;

        private static List<AbilityData> m_hpAbiPool = null;
        private static int m_hpTotalWeight = 1;
        private static List<AbilityData> m_attackAbiPool = null;
        private static int m_attackTotalWeight = 1;
        private static List<AbilityData> m_defenseAbiPool = null;
        private static int m_defenseTotalWeight = 1;
        private static List<AbilityData> m_speedAbiPool = null;
        private static int m_speedTotalWeight = 1;
        private static CharacterNamePoolData[] m_nameIDPool = null;

        public static OwningCharacterData CreateNewCharacter()
        {
            InitAbilityData();

            AbilityData _hp = RollFromList(m_hpTotalWeight, m_hpAbiPool);
            AbilityData _attack = RollFromList(m_attackTotalWeight, m_attackAbiPool);
            AbilityData _defense = RollFromList(m_defenseTotalWeight, m_defenseAbiPool);
            AbilityData _speed = RollFromList(m_speedTotalWeight, m_speedAbiPool);

            OwningCharacterData _newChar = new OwningCharacterData
            {
                Attack = Random.Range(_attack.MinValue, _attack.MaxValue),
                AttackAbilityID = _attack.ID,
                CharacterNameID = GetRandomName(),
                CharacterSpriteID = 0,
                Defense = Random.Range(_defense.MinValue, _defense.MaxValue),
                DefenseAbilityID = _defense.ID,
                Equipment_UDID_Body = null,
                Equipment_UDID_Foot = null,
                Equipment_UDID_Hand = null,
                Equipment_UDID_Head = null,
                Exp = 0,
                HP = Random.Range(_hp.MinValue, _hp.MaxValue),
                HPAbilityID = _hp.ID,
                Level = 1,
                SkillSlot_0 = 1,
                SkillSlot_1 = 2,
                SkillSlot_2 = 0,
                SkillSlot_3 = 0,
                SP = 100,
                Speed = Random.Range(_speed.MinValue, _speed.MaxValue),
                SpeedAbilityID = _speed.ID,
                UDID = System.Guid.NewGuid().ToString()
            };

            return _newChar;
        }

        public static int GetRandomName()
        {
            InitAbilityData();

            return m_nameIDPool[Random.Range(0, m_nameIDPool.Length - 1)].NameContextID;
        }

        private static void InitAbilityData()
        {
            if(m_inited)
            {
                return;
            }

            m_nameIDPool = GameDataManager.GetAllGameData<CharacterNamePoolData>();
            AbilityData[] _allDatas = GameDataManager.GetAllGameData<AbilityData>();
            m_hpAbiPool = new List<AbilityData>();
            m_attackAbiPool = new List<AbilityData>();
            m_defenseAbiPool = new List<AbilityData>();
            m_speedAbiPool = new List<AbilityData>();
            for (int i = 0; i < _allDatas.Length; i++)
            {
                if (_allDatas[i].ID == 0)
                    continue;

                switch (_allDatas[i].Type)
                {
                    case Keyword.HP:
                        {
                            m_hpAbiPool.Add(_allDatas[i]);
                            m_hpTotalWeight += _allDatas[i].Weight;
                            break;
                        }
                    case Keyword.Attack:
                        {
                            m_attackAbiPool.Add(_allDatas[i]);
                            m_attackTotalWeight += _allDatas[i].Weight;
                            break;
                        }
                    case Keyword.Defense:
                        {
                            m_defenseAbiPool.Add(_allDatas[i]);
                            m_defenseTotalWeight += _allDatas[i].Weight;
                            break;
                        }
                    case Keyword.Speed:
                        {
                            m_speedAbiPool.Add(_allDatas[i]);
                            m_speedTotalWeight += _allDatas[i].Weight;
                            break;
                        }
                    default:
                        {
                            throw new System.Exception("[CharacterManager][ctor] Invaild AbilityData Type=" + _allDatas[i].Type);
                        }
                }
            }

            m_inited = true;
        }

        private static AbilityData RollFromList(int totalWeight, List<AbilityData> abilities)
        {
            int _roll = Random.Range(0, totalWeight);
            for (int i = 0; i < abilities.Count; i++)
            {
                _roll -= abilities[i].Weight;
                if(_roll <= 0)
                {
                    return abilities[i];
                }
                else
                {
                    continue;
                }
            }

            return null;
        }

        public static void SetLevel(OwningCharacterData character, int targetLevel)
        {
            while(character.Level < targetLevel)
            {
                ForceLevelUp(character);
                character.Exp = GameDataManager.GetGameData<ExpData>(character.Level).Owning;
            }
        }

        public static void TryAddOneLevel(OwningCharacterData character)
        {
            ExpData _expData = GameDataManager.GetGameData<ExpData>(character.Level);
            int _needExp = _expData.Require - character.Exp;
            if (PlayerManager.Instance.Player.OwnExp >= _needExp)
            {
                PlayerManager.Instance.Player.OwnExp -= _needExp;
                AddExp(character, _needExp);
            }
            else
            {
                character.Exp += PlayerManager.Instance.Player.OwnExp;
                PlayerManager.Instance.Player.OwnExp = 0;
            }
        }

        public static void Depart(string UDID)
        {
            if (PlayerManager.Instance.Player.Characters.Count <= 4)
            {
                GameManager.Instance.MessageManager.ShowCommonMessage("角色不可少於4人", "Warning", null);
                return;
            }

            OwningCharacterData _target = PlayerManager.Instance.GetCharacterByUDID(UDID);
            PlayerManager.Instance.Player.Characters.Remove(_target);
            PlayerManager.Instance.Player.OwnExp += GameDataManager.GetGameData<ExpData>(_target.Level).Owning / 2;
        }

        public static void AddExp(OwningCharacterData character, int addExp)
        {
            character.Exp += addExp;
            while(GameDataManager.GetGameData<ExpData>(character.Level).Require != -1 &&
                  character.Exp - GameDataManager.GetGameData<ExpData>(character.Level).Require >= 0)
            {
                character.Exp -= GameDataManager.GetGameData<ExpData>(character.Level).Require;
                ForceLevelUp(character);
            }
        }

        private static void ForceLevelUp(OwningCharacterData character)
        {
            InitAbilityData();

            character.Level++;

            AbilityData _hp = m_hpAbiPool.Find(x => x.ID == character.HPAbilityID);
            AbilityData _attack = m_attackAbiPool.Find(x => x.ID == character.AttackAbilityID);
            AbilityData _defense = m_defenseAbiPool.Find(x => x.ID == character.DefenseAbilityID);
            AbilityData _speed = m_speedAbiPool.Find(x => x.ID == character.SpeedAbilityID);

            character.HP += Random.Range(_hp.MinValue, _hp.MaxValue);
            character.Attack += Random.Range(_attack.MinValue, _attack.MaxValue);
            character.Defense += Random.Range(_defense.MinValue, _defense.MaxValue);
            character.Speed += Random.Range(_speed.MinValue, _speed.MaxValue);
        }
    }
}
