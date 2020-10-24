using System;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_SetSkipCheckSP : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            GetSelf().skipCheckSP = true;
            onCompleted?.Invoke();
        }
    }
}
