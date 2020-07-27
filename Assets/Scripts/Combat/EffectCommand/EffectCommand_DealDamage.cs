using System;
using ProjectBS.Combat;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_DealDamage : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            CombatTargetSelecter.Instance.StartSelect(
                new CombatTargetSelecter.SelectTargetData
                {
                    attacker = caster,
                    commandString = vars[0],
                    onSelected = OnTargetSelected
                });
        }

        private void OnTargetSelected(List<CombatUnit> targets)
        {
            for(int i = 0; i < targets.Count; i++)
            {
                UnityEngine.Debug.Log("Selected:" + targets[i].name);
            }
        }
    }
}

