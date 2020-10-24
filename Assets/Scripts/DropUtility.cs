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
            DropInfo _dropInfo = new DropInfo();
            
            Data.BossData _mainBoss = GameDataManager.GetGameData<Data.BossData>(bossStage.MainBossID);
            _dropInfo.exp = Random.Range(bossStage.MinExp, bossStage.MaxExp + 1);
           
            int _randomDropCount = Random.Range(10, 21);
            string[] _dropPool = bossStage.DropData.Split('$');
            for (int i = 0; i < _randomDropCount; i++)
            {
                if (Random.Range(0f, 100f) <= GameDataLoader.GameProperties.DropSkillChance)
                {
                    _dropInfo.skillIDs.Add(RollSkill());
                }
                else
                {
                    _dropInfo.equipments.Add(RollEquipment(_dropPool));
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

