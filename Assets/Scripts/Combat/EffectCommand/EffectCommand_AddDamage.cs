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

            if(CombatManager.Instance.CurrentActionInfo.actor == processData.caster)
            {
                List<CombatUnit> _targets = new List<CombatUnit>(processData.caster.targetToDmg.Keys);
                for(int i = 0; i < _targets.Count; i++)
                {
                    if(_isPersent)
                    {
                        float _dmg = (float)processData.caster.targetToDmg[_targets[i]];
                        processData.caster.targetToDmg[_targets[i]] += Convert.ToInt32(_dmg * _value);
                    }
                    else
                    {
                        processData.caster.targetToDmg[_targets[i]] += Convert.ToInt32(_value);
                    }

                    if (processData.caster.targetToDmg[_targets[i]] < 1)
                        processData.caster.targetToDmg[_targets[i]] = 1;

                    UnityEngine.Debug.Log("add to dealing dmg->" + processData.caster.targetToDmg[_targets[i]]);
                }
            }
            else
            {
                if (_isPersent)
                {
                    float _dmg = (float)CombatManager.Instance.CurrentActionInfo.actor.targetToDmg[processData.caster];
                    CombatManager.Instance.CurrentActionInfo.actor.targetToDmg[processData.caster] += Convert.ToInt32(_dmg * _value);
                }
                else
                {
                    CombatManager.Instance.CurrentActionInfo.actor.targetToDmg[processData.caster] += Convert.ToInt32(_value);
                }
                if (CombatManager.Instance.CurrentActionInfo.actor.targetToDmg[processData.caster] < 1)
                    CombatManager.Instance.CurrentActionInfo.actor.targetToDmg[processData.caster] = 1;

                UnityEngine.Debug.Log("add to taken dmg->" + CombatManager.Instance.CurrentActionInfo.actor.targetToDmg[processData.caster]);
            }

            onCompleted?.Invoke();
        }
    }
}

