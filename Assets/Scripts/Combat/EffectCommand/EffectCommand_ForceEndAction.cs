using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_ForceEndAction : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            CombatManager.Instance.SetForceStopCurrentAction();
            onCompleted?.Invoke();
        }
    }
}

