using KahaGameCore.Static;
using ProjectBS.Data;
using System;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_CastSkill : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            SkillData _skill = GameDataManager.GetGameData<SkillData>(int.Parse(vars[0]));
            if(_skill == null)
            {
                throw new Exception("[EffectCommand_CastSkill][Process] Invaild Skill ID=" + int.Parse(vars[0]));
            }
            processData.caster.lastSkillID = _skill.ID;
            EffectProcessManager.GetSkillProcesser(processData.caster.lastSkillID).Start(new EffectProcesser.ProcessData
            {
                caster = GetSelf(),
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
