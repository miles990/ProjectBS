using System;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_Quit : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            UnityEngine.Debug.LogWarning("Quit");
            processData.skipIfCount = 999;
            onCompleted?.Invoke();
        }
    }
}
