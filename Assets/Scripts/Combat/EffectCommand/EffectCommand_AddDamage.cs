using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_AddDamage : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            float _addValue = CombatUtility.Calculate(new CombatUtility.CalculateData
            {
                caster = processData.caster,
                target = processData.target,
                formula = vars[0]
            });

            List<CombatUnit> _allTarget = new List<CombatUnit>(CombatManager.Instance.CurrentActionInfo.actor.targetToDmg.Keys);
            for (int i = 0; i < _allTarget.Count; i++)
            {
                CombatManager.Instance.CurrentActionInfo.actor.targetToDmg[_allTarget[i]] += Convert.ToInt32(_addValue);
            }
        }
    }
}

