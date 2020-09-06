using System;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_SetStatus : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            CombatUnit _target = null;

            switch (vars[0])
            {
                case Keyword.Self:
                    {
                        if (processData.referenceBuff != null)
                        {
                            _target = processData.referenceBuff.owner;
                        }
                        else
                        {
                            _target = processData.caster;
                        }
                        break;
                    }
                case Keyword.Caster:
                    {
                        _target = processData.caster;
                        break;
                    }
                case Keyword.Target:
                    {
                        _target = processData.target;
                        break;
                    }
            }

            float _value = CombatUtility.Calculate(
                new CombatUtility.CalculateData
                {
                    caster = processData.caster,
                    target = processData.target,
                    formula = vars[2],
                    useRawValue = true
                });
            int _set = Convert.ToInt32(_value);

            switch (vars[1].Trim())
            {
                case Keyword.Attack:
                    {
                        _target.rawAttack = _set;
                        break;
                    }
                case Keyword.Defence:
                    {
                        _target.rawDefence = _set;
                        break;
                    }
                case Keyword.MaxHP:
                    {
                        _target.rawMaxHP = _set;
                        break;
                    }
                case Keyword.Speed:
                    {
                        _target.rawSpeed = _set;
                        break;
                    }
                case Keyword.Hatred:
                    {
                        _target.hatred = _set;
                        UnityEngine.Debug.LogWarningFormat("SetStatus {0} hatred={1}", _target, _target.hatred);
                        break;
                    }
                case Keyword.HP:
                    {
                        _target.HP = _set;
                        break;
                    }
                case Keyword.SP:
                    {
                        _target.SP = _set;
                        break;
                    }
                default:
                    {
                        throw new Exception("[EffectCommand_SetStatus][Process] Invaild status:" + vars[1]);
                    }
            }

            onCompleted?.Invoke();
        }
    }
}
