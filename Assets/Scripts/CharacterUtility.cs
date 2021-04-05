using ProjectBS.Data;
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
        private static AppearanceData[] m_appearanceIDPool = null;

        public static OwningCharacterData CreateNewCharacter()
        {
            InitAbilityData();

            AbilityData _hp = RollFromList(m_hpTotalWeight, m_hpAbiPool);
            AbilityData _attack = RollFromList(m_attackTotalWeight, m_attackAbiPool);
            AbilityData _defense = RollFromList(m_defenseTotalWeight, m_defenseAbiPool);
            AbilityData _speed = RollFromList(m_speedTotalWeight, m_speedAbiPool);

            AppearanceData _skin = GetRandomSkin();
            string[] _skillIDs = _skin.DefaultSkillSet.Split(';');
            if(_skillIDs.Length > 8)
            {
                throw new System.Exception("[CharacterUtility][CreateNewCharacter] _skillIDs MUST be less then 8 elements. AppearanceData ID=" + _skin.ID);
            }

            OwningCharacterData _newChar = new OwningCharacterData
            {
                Name = ContextConverter.Instance.GetContext(_skin.NameContextID),
                Attack = Random.Range(_attack.MinValue, _attack.MaxValue),
                AttackAbilityID = _attack.ID,
                AppearanceDataID = _skin.ID,
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
                SP = 100,
                Speed = Random.Range(_speed.MinValue, _speed.MaxValue),
                SpeedAbilityID = _speed.ID,
                UDID = System.Guid.NewGuid().ToString()
            };

            for(int i = 0; i < _skillIDs.Length; i++)
            {
                _newChar.Skills[i] = int.Parse(_skillIDs[i]);
            }

            return _newChar;
        }

        public static AppearanceData GetRandomSkin()
        {
            InitAbilityData();

            AppearanceData _random = m_appearanceIDPool[Random.Range(0, m_appearanceIDPool.Length - 1)];

            if (_random.IsDrop == 1)
                return _random;
            else
                return GetRandomSkin();
        }

        private static void InitAbilityData()
        {
            if(m_inited)
            {
                return;
            }

            m_appearanceIDPool = GameDataManager.GetAllGameData<AppearanceData>();
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
            if(PlayerManager.Instance.Player.OwnExp <= 0)
                return;

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

        public static bool Depart(string UDID)
        {
            if (PlayerManager.Instance.Player.Characters.Count <= 4)
            {
                GameManager.Instance.MessageManager.ShowCommonMessage("1000016", "Warning", null);
                return false;
            }

            if (PlayerManager.Instance.GetPartyIndex(UDID) != -1)
            {
                GameManager.Instance.MessageManager.ShowCommonMessage("1000017", "Warning", null);
                return false;
            }

            OwningCharacterData _target = PlayerManager.Instance.GetCharacterByUDID(UDID);
            PlayerManager.Instance.Player.Characters.Remove(_target);
            PlayerManager.Instance.Player.OwnExp += GameDataManager.GetGameData<ExpData>(_target.Level).Owning / 2;
            return true;
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
