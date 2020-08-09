using System;
using KahaGameCore.Static;
using ProjectBS.Data;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_GainBuff : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            CombatUnit _target = null;
            switch(vars[0].Trim())
            {
                case Keyword.Self:
                case Keyword.Caster:
                    {
                        _target = processData.caster;
                        break;
                    }
                case Keyword.Target:
                    {
                        _target = processData.target;
                        break;
                    }
            }

            int _effectID = int.Parse(vars[1]);
            int _remainingTime = int.Parse(vars[2]);
            CombatUnit.Buff _buff = _target.buffs.Find(x => x.effectID == _effectID);
            SkillEffectData _skillEffectData = GameDataManager.GetGameData<SkillEffectData>(_effectID);
            
            if (_buff != null)
            {
                if (_skillEffectData.MaxStackCount > _buff.stackCount)
                {
                    _buff.stackCount++;
                }
                _buff.remainingTime = _remainingTime;
            }
            else
            {
                _buff = new CombatUnit.Buff
                {
                    effectID = _effectID,
                    from = processData.caster,
                    remainingTime = _remainingTime,
                    stackCount = 1
                };

                _target.buffs.Add(_buff);
            }
            
            new EffectProcesser(_skillEffectData.Command).Start(
                new EffectProcesser.ProcessData
                {
                    caster = processData.caster,
                    target = processData.target,
                    allEffectProcesser = processData.allEffectProcesser,
                    timing = EffectProcesser.TriggerTiming.OnActived,
                    referenceBuff = _buff,
                    refenceSkill = null,
                    onEnded = onCompleted
                });
        }
    }
}
