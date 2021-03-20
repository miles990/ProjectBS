using System;
using KahaGameCore.Static;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_BeginIf : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            if(processData.skipIfCount > 0)
            {
                processData.skipIfCount++;
                onCompleted?.Invoke();
                return;
            }

            string _condtion = vars[0].RemoveBlankCharacters();
            string _condtionMark;
            if(_condtion.Contains(">="))
            {
                _condtionMark = ">=";
            }
            else if(_condtion.Contains("<="))
            {
                _condtionMark = "<=";
            }
            else if(_condtion.Contains("=="))
            {
                _condtionMark = "==";
            }
            else if(_condtion.Contains("!="))
            {
                _condtionMark = "!=";
            }
            else
            {
                throw new Exception("[EffectCommand_BeginIf][Process] invaild var=" + vars[0]);
            }

            _condtion = _condtion.Replace(_condtionMark, ";");
            string[] _compareParts = _condtion.Split(';');
            GetValue(_compareParts[0], out int _varA);
            GetValue(_compareParts[1], out int _varB);

            bool _pass;

            switch (_condtionMark)
            {
                case ">=":
                    {
                        _pass = _varA >= _varB;
                        break;
                    }
                case "<=":
                    {
                        _pass = _varA <= _varB;
                        break;
                    }
                case "==":
                    {
                        _pass = _varA == _varB;
                        break;
                    }
                case "!=":
                    {
                        _pass = _varA != _varB;
                        break;
                    }
                default:
                    {
                        throw new Exception("[EffectCommand_BeginIf][Process] invaild var=" + vars[0]);
                    }
            }

            if(!_pass)
            {
                processData.skipIfCount++;
            }

            onCompleted?.Invoke();
        }

        private void GetValue(string paraString, out int value)
        {
            if (!int.TryParse(paraString, out value))
            {
                float _value = CombatUtility.Calculate(
                    new CombatUtility.CalculateData
                    {
                        caster = processData.caster,
                        target = processData.target,
                        processData = processData,
                        formula = paraString,
                        useRawValue = false
                    });
                value = Convert.ToInt32(_value);
            }
        }
    }
}

