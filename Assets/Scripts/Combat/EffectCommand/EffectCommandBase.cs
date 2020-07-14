using System;
using ProjectBS.Combat;

namespace ProjectBS.Combat.EffectCommand
{
    public abstract class EffectCommandBase
    {
        protected CombatManager.CombatUnit Caster { get; private set; }

        public EffectCommandBase(CombatManager.CombatUnit caster)
        {
            Caster = caster;
        }
        public abstract void Process(string[] vars, Action onCompleted);
    }
}