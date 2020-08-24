using System;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_BeginIf_Effect : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            if (processData.skipIfCount > 0)
            {
                processData.skipIfCount++;
                onCompleted?.Invoke();
                return;
            }
        }
    }
}
