using KahaGameCore.Static;
using ProjectBS.Data;
using System;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_CastSkill : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            if (GetSelf().IsSkipAtion)
                onCompleted?.Invoke();

            SkillData _skill = GameDataManager.GetGameData<SkillData>(int.Parse(vars[0]));
            if(_skill == null)
            {
                throw new Exception("[EffectCommand_CastSkill][Process] Invaild Skill ID=" + int.Parse(vars[0]));
            }
            GetSelf().lastSkillID = _skill.ID;

            new EffectProcesser(_skill.Command).Start(new EffectProcesser.ProcessData
            {
                caster = GetSelf(),
                target = null,
                timing = EffectProcesser.TriggerTiming.OnActived,
                allEffectProcesser = CombatUtility.ComabtManager.GetNewAllProcesser(),
                referenceBuff = null,
                refenceSkill = new EffectProcesser.ProcessData.ReferenceSkillInfo
                {
                    skill = _skill,
                    owner = GetSelf()
                },
                onEnded = onCompleted
            });
        }
    }
}
