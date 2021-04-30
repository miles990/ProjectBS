using System;
using System.Collections.Generic;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_AddDamage : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            AddSkillOrEffectInfo();

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
                    target = processData.target,
                    processData = processData,
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
            if(CombatUtility.ComabtManager.CurrentActionInfo.actor == GetSelf())
            {
                List<string> _targets = new List<string>(processData.caster.targetToDmg.Keys);
                int _intDmg;
                if (_isPersent)
                {
                    float _dmg = (float)processData.caster.targetToDmg[processData.target.UDID];
                    _intDmg = Convert.ToInt32(_dmg * _value);
                    processData.caster.targetToDmg[processData.target.UDID] += _intDmg;

                }
                else
                {
                    _intDmg = Convert.ToInt32(_value);
                    processData.caster.targetToDmg[processData.target.UDID] += _intDmg;
                }

                ShowAddAttackDamage(_intDmg, processData.target.UDID);

                if (processData.caster.targetToDmg[processData.target.UDID] < 1)
                    processData.caster.targetToDmg[processData.target.UDID] = 1;
            }
            else
            {
                int _intDmg;
                if (_isPersent)
                {
                    float _dmg = (float)CombatUtility.ComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster.UDID];
                    _intDmg = Convert.ToInt32(_dmg * _value);
                    CombatUtility.ComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster.UDID] += _intDmg;
                }
                else
                {
                    _intDmg = Convert.ToInt32(_value);
                    CombatUtility.ComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster.UDID] += _intDmg;
                }

                ShowAddTargetTakenDamage(_intDmg, processData.caster.UDID);

                if (CombatUtility.ComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster.UDID] < 1)
                    CombatUtility.ComabtManager.CurrentActionInfo.actor.targetToDmg[processData.caster.UDID] = 1;
            }

            onCompleted?.Invoke();
        }

        private void ShowAddTargetTakenDamage(int add, string udid)
        {
            if (add > 0)
            {
                GetPage<UI.CombatUIView>().AddCombatInfo
                    (
                        string.Format
                        (
                            ContextConverter.Instance.GetContext(500010),
                            CombatUtility.ComabtManager.GetUnitByUDID(udid).name,
                            add
                        ), null
                    );
            }
            else
            {
                GetPage<UI.CombatUIView>().AddCombatInfo
                    (
                        string.Format
                        (
                            ContextConverter.Instance.GetContext(500012),
                            CombatUtility.ComabtManager.GetUnitByUDID(udid).name,
                            add * -1
                        ), null
                    );
            }
        }

        private void ShowAddAttackDamage(int add, string udid)
        {
            if (add > 0)
            {
                GetPage<UI.CombatUIView>().AddCombatInfo
                    (
                        string.Format
                        (
                            ContextConverter.Instance.GetContext(500009),
                            CombatUtility.ComabtManager.GetUnitByUDID(udid).name,
                            add
                        ), null
                    );
            }
            else
            {
                GetPage<UI.CombatUIView>().AddCombatInfo
                    (
                        string.Format
                        (
                            ContextConverter.Instance.GetContext(500011),
                            CombatUtility.ComabtManager.GetUnitByUDID(udid).name,
                            add * -1
                        ), null
                    );
            }
        }

    }
}

