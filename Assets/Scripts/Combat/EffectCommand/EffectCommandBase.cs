using System;

namespace ProjectBS.Combat.EffectCommand
{
    public abstract class EffectCommandBase
    {
        public CombatUnit caster = null;
        public CombatUnit target = null;

        public abstract void Process(string[] vars, Action onCompleted);
    }
}
