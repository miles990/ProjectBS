using ProjectBS.Data;
using System;
using UnityEngine;

namespace ProjectBS.Combat
{
    public static class SkillActiver
    {
        public static void Active(CombatUnit attacker, SkillData skill, Action onAttackEnded)
        {
            Debug.Log(skill.Command.RemoveBlankCharacters());
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
