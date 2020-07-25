using System;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_SetStatus : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            CombatUnit _target = vars[0] == "Self" ? caster : target;


        }
    }
}
