using ProjectBS.Data;
using System;

namespace ProjectBS.Combat
{
    public static class SkillActiver
    {
        public static void Active(CombatUnit attacker, SkillData skill, Action onAttackEnded)
        {
            new EffectProcesser(skill.Command).Start(
                new EffectProcesser.ProcessData
                {
                    caster = attacker,
                    target = null,
                    timing = EffectProcesser.TriggerTiming.OnActived,
                    onEnded = onAttackEnded
                });
        }
    }
}
