using ProjectBS.Data;
using System.Collections.Generic;
using KahaGameCore.Static;
using ProjectBS.Combat;

using Random = UnityEngine.Random;

namespace ProjectBS
{
    public class CharacterManager
    {
        public static CharacterManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new CharacterManager();
                }
                return m_instance;
            }
        }
        private static CharacterManager m_instance = null;

        private CharacterManager() 
        {
            AbilityData[] _allDatas = GameDataManager.GetAllGameData<AbilityData>();
            m_hpAbiPool = new List<AbilityData>();
            m_attackAbiPool = new List<AbilityData>();
            m_defenceAbiPool = new List<AbilityData>();
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
                    case Keyword.Defence:
                        {
                            m_defenceAbiPool.Add(_allDatas[i]);
                            m_defenceTotalWeight += _allDatas[i].Weight;
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
        }

        private List<AbilityData> m_hpAbiPool = null;
        private int m_hpTotalWeight = 1;
        private List<AbilityData> m_attackAbiPool = null;
        private int m_attackTotalWeight = 1;
        private List<AbilityData> m_defenceAbiPool = null;
        private int m_defenceTotalWeight = 1;
        private List<AbilityData> m_speedAbiPool = null;
        private int m_speedTotalWeight = 1;

        public OwningCharacterData CreateNewCharacter()
        {
            AbilityData _hp = RollFromList(m_hpTotalWeight, m_hpAbiPool);
            AbilityData _attack = RollFromList(m_attackTotalWeight, m_attackAbiPool);
            AbilityData _defence = RollFromList(m_defenceTotalWeight, m_defenceAbiPool);
            AbilityData _speed = RollFromList(m_speedTotalWeight, m_speedAbiPool);

            OwningCharacterData _newChar = new OwningCharacterData
            {
                Attack = Random.Range(_attack.MinValue, _attack.MaxValue),
                AttackAbilityID = _attack.ID,
                CharacterNameID = 0,
                CharacterSpriteID = 0,
                Defence = Random.Range(_defence.MinValue, _defence.MaxValue),
                DefenceAbilityID = _defence.ID,
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
                SKillSlot_2 = 0,
                SKillSlot_3 = 0,
                SP = 100,
                Speed = Random.Range(_speed.MinValue, _speed.MaxValue),
                SpeedAbilityID = _speed.ID,
                UDID = System.Guid.NewGuid().ToString()
            };

            return _newChar;
        }

        private AbilityData RollFromList(int totalWeight, List<AbilityData> abilities)
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
    }
}
