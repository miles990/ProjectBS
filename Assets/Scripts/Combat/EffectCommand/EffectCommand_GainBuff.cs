using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_GainBuff : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            Debug.Log("EffectCommand_GainBuff Caster:" + caster.name);
            Debug.Log("EffectCommand_GainBuff Target:" + caster.name);
        }
    }
}
