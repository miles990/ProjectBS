using System;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_ForceEndAction : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            CombatManager.Instance.MarkForceStopCurrentActionOnStart();
            onCompleted?.Invoke();
        }
    }
}

