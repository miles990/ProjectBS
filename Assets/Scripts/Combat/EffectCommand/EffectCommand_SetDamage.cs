using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_SetDamage : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            string _valueString = vars[0];
            float _pureValue = CombatUtility.Calculate(new CombatUtility.CalculateData
            {
                caster = GetSelf(),
                target = null,
                formula = _valueString,
                referenceBuff = processData.referenceBuff,
                useRawValue = false
            });
            _valueString = Convert.ToInt32(_pureValue).ToString();

            float _value = float.Parse(_valueString);
            if (CombatUtility.CurrentComabtManager.CurrentActionInfo.actor == processData.caster)
            {
                List<CombatUnit> _targets = new List<CombatUnit>(processData.caster.targetToDmg.Keys);
                for (int i = 0; i < _targets.Count; i++)
                {
                    int _intDmg = Convert.ToInt32(_value);
                    processData.caster.targetToDmg[_targets[i]] = _intDmg;

                    if (processData.caster.targetToDmg[_targets[i]] < 0)
                        processData.caster.targetToDmg[_targets[i]] = 0;
                }
            }
            else
            {
                int _intDmg = Convert.ToInt32(_value);
                CombatUtility.CurrentComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster] = _intDmg;

                if (CombatUtility.CurrentComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster] < 0)
                    CombatUtility.CurrentComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster] = 0;
            }

            onCompleted?.Invoke();

        }
    }
}
