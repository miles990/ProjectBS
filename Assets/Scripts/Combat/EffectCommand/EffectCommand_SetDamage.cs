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
            if (CombatUtility.ComabtManager.CurrentActionInfo.actor == processData.caster)
            {
                List<string> _targetUDIDs = new List<string>(processData.caster.targetToDmg.Keys);
                for (int i = 0; i < _targetUDIDs.Count; i++)
                {
                    int _intDmg = Convert.ToInt32(_value);
                    processData.caster.targetToDmg[_targetUDIDs[i]] = _intDmg;

                    if (processData.caster.targetToDmg[_targetUDIDs[i]] < 0)
                        processData.caster.targetToDmg[_targetUDIDs[i]] = 0;
                }
            }
            else
            {
                int _intDmg = Convert.ToInt32(_value);
                CombatUtility.ComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster.UDID] = _intDmg;

                if (CombatUtility.ComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster.UDID] < 0)
                    CombatUtility.ComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster.UDID] = 0;
            }

            onCompleted?.Invoke();

        }
    }
}
