using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_AddDamage : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            bool _isPersent;
            string _valueString = vars[0];
            if (vars[0].Contains("%"))
            {
                _isPersent = true;
                _valueString = _valueString.Replace("%", "");
            }
            else
            {
                _isPersent = false;
                float _pureValue = CombatUtility.Calculate(new CombatUtility.CalculateData
                {
                    caster = GetSelf(),
                    target = null,
                    formula = _valueString,
                    useRawValue = false
                });
                _valueString = Convert.ToInt32(_pureValue).ToString();
            }
            float _value = float.Parse(_valueString);
            if(_isPersent)
            {
                _value *= 0.01f;
            }
            if(CombatUtility.CurrentComabtManager.CurrentActionInfo.actor == processData.caster)
            {
                List<CombatUnit> _targets = new List<CombatUnit>(processData.caster.targetToDmg.Keys);
                for(int i = 0; i < _targets.Count; i++)
                {
                    int _intDmg;
                    if (_isPersent)
                    {
                        float _dmg = (float)processData.caster.targetToDmg[_targets[i]];
                        _intDmg = Convert.ToInt32(_dmg * _value);
                        processData.caster.targetToDmg[_targets[i]] += _intDmg;
                    }
                    else
                    {
                        _intDmg = Convert.ToInt32(_value);
                        processData.caster.targetToDmg[_targets[i]] += _intDmg;
                    }

                    if (processData.caster.targetToDmg[_targets[i]] < 1)
                        processData.caster.targetToDmg[_targets[i]] = 1;
                }
            }
            else
            {
                int _intDmg;
                if (_isPersent)
                {
                    float _dmg = (float)CombatUtility.CurrentComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster];
                    _intDmg = Convert.ToInt32(_dmg * _value);
                    CombatUtility.CurrentComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster] += _intDmg;
                }
                else
                {
                    _intDmg = Convert.ToInt32(_value);
                    CombatUtility.CurrentComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster] += _intDmg;
                }

                if (CombatUtility.CurrentComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster] < 1)
                    CombatUtility.CurrentComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster] = 1;

            }

            onCompleted?.Invoke();
        }
    }
}

