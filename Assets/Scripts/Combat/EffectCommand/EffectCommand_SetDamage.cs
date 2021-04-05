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
            AddSkillOrEffectInfo();

            string _valueString = vars[0];
            float _pureValue = CombatUtility.Calculate(new CombatUtility.CalculateData
            {
                caster = GetSelf(),
                target = null,
                formula = _valueString,
                processData = processData,
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

                    GetPage<UI.CombatUIView>().AddCombatInfo
                        (
                            string.Format
                            (
                                ContextConverter.Instance.GetContext(500024),
                                CombatUtility.ComabtManager.GetUnitByUDID(_targetUDIDs[i]).name,
                                processData.caster.targetToDmg[_targetUDIDs[i]]
                            ), null
                        );
                }
            }
            else
            {
                int _intDmg = Convert.ToInt32(_value);
                CombatUtility.ComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster.UDID] = _intDmg;

                if (CombatUtility.ComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster.UDID] < 0)
                    CombatUtility.ComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster.UDID] = 0;

                GetPage<UI.CombatUIView>().AddCombatInfo
                    (
                        string.Format
                        (
                            ContextConverter.Instance.GetContext(500024),
                            CombatUtility.ComabtManager.GetUnitByUDID(processData.caster.UDID).name,
                            CombatUtility.ComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster.UDID]
                        ), null
                    );
            }

            onCompleted?.Invoke();

        }
    }
}
