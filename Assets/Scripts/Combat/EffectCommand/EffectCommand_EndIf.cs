using System;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_EndIf : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            processData.skipIfCount--;
            if (processData.skipIfCount < 0)
                processData.skipIfCount = 0;
            UnityEngine.Debug.LogWarning("EndIf final processData.skipIfCount=" + processData.skipIfCount);
            onCompleted?.Invoke();
        }
    }
}

