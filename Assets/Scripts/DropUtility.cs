using System.Collections.Generic;
using UnityEngine;
using KahaGameCore.Static;

namespace ProjectBS
{
    public static class DropUtility
    {
        public class DropInfo
        {
            public int exp;
            public List<Data.OwningEquipmentData> equipments = new List<Data.OwningEquipmentData>();
            public List<int> skillIDs = new List<int>(); 
        }

        public static DropInfo Drop(Data.BossStageData bossStage)
        {
            DropInfo _dropInfo = new DropInfo
            {
                exp = Random.Range(bossStage.MinExp, bossStage.MaxExp + 1)
            };
            DropInfo _skillAndEquipmentDrop = Drop(bossStage.DropData);
            _dropInfo.equipments = _skillAndEquipmentDrop.equipments;
            _dropInfo.skillIDs = _skillAndEquipmentDrop.skillIDs;

            return _dropInfo;
        }

        public static DropInfo Drop(string info)
        {
            DropInfo _dropInfo = new DropInfo();
            Debug.Log(info);
            int _randomDropCount = Random.Range(10, 21);
            if(!string.IsNullOrEmpty(info))
            {
                string[] _dropPool = info.Split(';');
                for (int i = 0; i < _randomDropCount; i++)
                {
                    if (Random.Range(0f, 100f) <= GameDataManager.GameProperties.DropNormalSkillChance)
                    {
                        _dropInfo.skillIDs.Add(RollSkill());
                    }
                    else
                    {
                        //_dropInfo.equipments.Add(RollEquipment(_dropPool));
                        RollAndAddSpecialSkill(_dropPool);
                    }
                }
            }
            else
            {
                for (int i = 0; i < _randomDropCount; i++)
                {
                    _dropInfo.skillIDs.Add(RollSkill());
                }
            }

            return _dropInfo;
        }

        private static Data.OwningEquipmentData RollEquipment(string[] pool)
        {
            int _total = 0;
            for(int i = 0; i < pool.Length; i++)
            {
                _total += int.Parse(pool[i].Split(':')[1]);
            }
            int _random = Random.Range(0, _total);
            for(int i = 0; i < pool.Length; i++)
            {
                string[] _data = pool[i].Split(':');
                _random -= int.Parse(_data[1]);
                if(_random < 0)
                {
                    return EquipmentUtility.CreateNewEquipment(int.Parse(_data[0]));
                }
            }
            return null;
        }

        private static void RollAndAddSpecialSkill(string[] pool)
        {
            int _total = 0;
            for (int i = 0; i < pool.Length; i++)
            {
                _total += int.Parse(pool[i].Split(':')[1]);
            }
            int _random = Random.Range(0, _total);
            for (int i = 0; i < pool.Length; i++)
            {
                string[] _data = pool[i].Split(':');
                _random -= int.Parse(_data[1]);
                if (_random < 0)
                {
                    PlayerManager.Instance.AddSkill(int.Parse(_data[0]));
                }
            }
        }

        private static int RollSkill()
        {
            Data.SkillData[] _allSkill = GameDataManager.GetAllGameData<Data.SkillData>();
            Data.SkillData _random = _allSkill[Random.Range(0, _allSkill.Length)];

            int _count = 0;
            while(_random.IsDrop == 0)
            {
                _random = _allSkill[Random.Range(0, _allSkill.Length)];
                _count++;
                if(_count >= 100)
                {
                    return 1;
                }
            }

            return _random.ID;
        }
    }
}

