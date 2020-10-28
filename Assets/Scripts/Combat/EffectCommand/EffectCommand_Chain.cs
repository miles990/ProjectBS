using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_Chain : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            CombatUnit _self = GetSelf();
            List<CombatUnit> _pool = CombatUtility.CurrentComabtManager.GetSameCampUnits(_self.camp);

            for(int i = 0; i < _pool.Count; i++)
            {
                if(_pool[i] == _self
                    || _pool[i].skipAction
                    || !HasSkill(_pool[i], vars[0]))
                {
                    _pool.RemoveAt(i);
                    i--;
                    continue;
                }
            }

            if(_pool.Count <= 0)
            {
                onCompleted?.Invoke();
                return;
            }

            int _roll = UnityEngine.Random.Range(0, _pool.Count);
            Data.SkillData _skill = GameDataManager.GetGameData<Data.SkillData>(int.Parse(vars[0]));
            new EffectProcesser(_skill.Command).
                Start(new EffectProcesser.ProcessData
                {
                    allEffectProcesser = processData.allEffectProcesser,
                    caster = _pool[_roll],
                    refenceSkill = _skill,
                    referenceBuff = processData.referenceBuff,
                    skipIfCount = 0,
                    target = null,
                    timing = EffectProcesser.TriggerTiming.OnActived,
                    onEnded = onCompleted
                });
        }

        private bool HasSkill(CombatUnit target, string skillID)
        {
            string[] _skills = target.skills.Split(',');
            for(int i = 0; i < _skills.Length; i++)
            {
                if(_skills[i].Trim() == skillID)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
