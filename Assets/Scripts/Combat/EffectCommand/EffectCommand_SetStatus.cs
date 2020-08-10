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

            switch (vars[1])
            {
                case Keyword.Attack:
                    {
                        processData.target.rawAttack = _set;
                        break;
                    }
                case Keyword.Defence:
                    {
                        processData.target.rawDefence = _set;
                        break;
                    }
                case Keyword.MaxHP:
                    {
                        processData.target.rawMaxHP = _set;
                        break;
                    }
                case Keyword.Speed:
                    {
                        processData.target.rawSpeed = _set;
                        break;
                    }
                case Keyword.Hatred:
                    {
                        processData.target.hatred = _set;
                        break;
                    }
                case Keyword.HP:
                    {
                        processData.target.HP = _set;
                        break;
                    }
                case Keyword.SP:
                    {
                        processData.target.SP = _set;
                        break;
                    }
            }

            onCompleted?.Invoke();
        }
    }
}
