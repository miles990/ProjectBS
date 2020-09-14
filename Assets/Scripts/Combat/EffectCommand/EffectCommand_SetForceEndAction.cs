using System;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_SetForceEndAction : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            GetSelf().skipAction = true;
            onCompleted?.Invoke();
        }
    }
}

