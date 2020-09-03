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

            UnityEngine.Debug.Log("_varA=" + _varA + ", _varB=" + _varB);

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
                string[] _getValuePart = paraString.Split('.');
                switch (_getValuePart[0])
                {
                    case Keyword.Self:
                    case Keyword.Caster:
                        {
                            value = CombatUtility.GetStatusValue(processData.caster, _getValuePart[1], false);
                            break;
                        }
                    case Keyword.Target:
                        {
                            value = CombatUtility.GetStatusValue(processData.target, _getValuePart[1], false);
                            break;
                        }
                    case Keyword.CombatField:
                        {
                            value = CombatUtility.GetCombatFieldStatus(_getValuePart[1]);
                            break;
                        }
                    default:
                        {
                            throw new Exception("[EffectCommand_BeginIf][Process] Invaild Command=" + paraString);
                        }
                }
            }
        }
    }
}

