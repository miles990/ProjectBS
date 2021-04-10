using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_Chain : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            CombatUnit _self = GetSelf();
            List<CombatUnit> _pool = CombatUtility.ComabtManager.GetSameCampUnits(_self.camp);

            for(int i = 0; i < _pool.Count; i++)
            {
                if(_pool[i] == _self
                    || _pool[i].IsSkipAtion
                    || !HasSkill(_pool[i], int.Parse(vars[0])))
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

            AddSkillOrEffectInfo();
            GetPage<UI.CombatUIView>().AddCombatInfo
                (
                    string.Format
                    (
                        ContextConverter.Instance.GetContext(500019),
                        ContextConverter.Instance.GetContext(_skill.NameContextID)
                    ), null
                );

            _pool[_roll].lastSkillID = _skill.ID;
            new EffectProcesser(_skill.Command).
                Start(new EffectProcesser.ProcessData
                {
                    allEffectProcesser = CombatUtility.ComabtManager.AllProcesser,
                    caster = _pool[_roll],
                    refenceSkill = new EffectProcesser.ProcessData.ReferenceSkillInfo
                    {
                        skill = _skill,
                        owner = _pool[_roll]
                    },
                    referenceBuff = processData.referenceBuff,
                    skipIfCount = 0,
                    target = null,
                    timing = EffectProcesser.TriggerTiming.OnActived,
                    onEnded = onCompleted
                });
        }

        private bool HasSkill(CombatUnit target, int skillID)
        {
            for(int i = 0; i < target.skills.Length; i++)
            {
                if(target.skills[i] == skillID)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
