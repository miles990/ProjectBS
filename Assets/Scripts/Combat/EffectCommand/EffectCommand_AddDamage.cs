using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_AddDamage : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            bool _isPersent = false;
            string _valueString = vars[0];
            if (vars[0].Contains("%"))
            {
                _isPersent = true;
                _valueString = _valueString.Replace("%", "");
            }
            float _value = float.Parse(_valueString);
            if(_isPersent)
            {
                _value *= 0.01f;
            }
            UnityEngine.Debug.LogWarning("AddDamage _value=" + _value);
            if(CombatManager.Instance.CurrentActionInfo.actor == processData.caster)
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

                    UnityEngine.Debug.LogWarning("AddDamage add dealed dmg=" + _intDmg);

                    if (processData.caster.targetToDmg[_targets[i]] < 1)
                        processData.caster.targetToDmg[_targets[i]] = 1;
                }
            }
            else
            {
                int _intDmg;
                if (_isPersent)
                {
                    float _dmg = (float)CombatManager.Instance.CurrentActionInfo.actor.targetToDmg[processData.caster];
                    _intDmg = Convert.ToInt32(_dmg * _value);
                    CombatManager.Instance.CurrentActionInfo.actor.targetToDmg[processData.caster] += _intDmg;
                }
                else
                {
                    _intDmg = Convert.ToInt32(_value);
                    CombatManager.Instance.CurrentActionInfo.actor.targetToDmg[processData.caster] += _intDmg;
                }

                UnityEngine.Debug.LogWarning("AddDamage add taken dmg=" + _intDmg);

                if (CombatManager.Instance.CurrentActionInfo.actor.targetToDmg[processData.caster] < 1)
                    CombatManager.Instance.CurrentActionInfo.actor.targetToDmg[processData.caster] = 1;

            }

            onCompleted?.Invoke();
        }
    }
}

