using System;
using ProjectBS.Data;
using KahaGameCore.Static;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_RandomCastSkill : EffectCommandBase
    {
        private class RandomSkillSource
        {
            public int skillID;
            public int weight;
        }

        public override void Process(string[] vars, Action onCompleted)
        {
            int _randomSKillID = int.Parse(vars[0]);

            RandomSkillData _randomSKillData = GameDataManager.GetGameData<RandomSkillData>(_randomSKillID);
            List<RandomSkillSource> _randomPool = new List<RandomSkillSource>();
            int _totalWeight = 0;

            if(_randomSKillData.SkillID1 != 0)
            {
                _randomPool.Add(new RandomSkillSource { skillID = _randomSKillData.SkillID1, weight = _randomSKillData.Weight1 });
                _totalWeight += _randomSKillData.Weight1;
            }

            if (_randomSKillData.SkillID2 != 0)
            {
                _randomPool.Add(new RandomSkillSource { skillID = _randomSKillData.SkillID2, weight = _randomSKillData.Weight2 });
                _totalWeight += _randomSKillData.Weight2;
            }

            if (_randomSKillData.SkillID3 != 0)
            {
                _randomPool.Add(new RandomSkillSource { skillID = _randomSKillData.SkillID3, weight = _randomSKillData.Weight3 });
                _totalWeight += _randomSKillData.Weight3;
            }

            if (_randomSKillData.SkillID4 != 0)
            {
                _randomPool.Add(new RandomSkillSource { skillID = _randomSKillData.SkillID4, weight = _randomSKillData.Weight4 });
                _totalWeight += _randomSKillData.Weight4;
            }

            if (_randomSKillData.SkillID5 != 0)
            {
                _randomPool.Add(new RandomSkillSource { skillID = _randomSKillData.SkillID5, weight = _randomSKillData.Weight5 });
                _totalWeight += _randomSKillData.Weight5;
            }

            int _roll = UnityEngine.Random.Range(0, _totalWeight);

            for(int i = 0; i < _randomPool.Count; i++)
            {
                _roll -= _randomPool[i].weight;
                if(_roll <= 0)
                {
                    SkillData _skill = GameDataManager.GetGameData<SkillData>(_randomPool[i].skillID);

                    new EffectProcesser(_skill.Command).Start(new EffectProcesser.ProcessData
                    {
                        caster = GetSelf(),
                        target = null,
                        timing = EffectProcesser.TriggerTiming.OnActived,
                        allEffectProcesser = processData.allEffectProcesser,
                        referenceBuff = null,
                        refenceSkill = _skill,
                        onEnded = onCompleted
                    });
                    break;
                }
            }
        }
    }
}
