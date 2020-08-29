﻿using KahaGameCore.Static;
using ProjectBS.Data;
using System;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_CastSkill : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            SkillData _skill = GameDataManager.GetGameData<SkillData>(int.Parse(vars[0]));

            new EffectProcesser(_skill.Command).Start(new EffectProcesser.ProcessData
            {
                caster = processData.caster,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnActived,
                allEffectProcesser = processData.allEffectProcesser,
                referenceBuff = null,
                refenceSkill = _skill,
                onEnded = onCompleted
            });
        }
    }
}