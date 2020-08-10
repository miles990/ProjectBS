using System;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_AddStatus : EffectCommandBase
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

            switch(vars[1])
            {
                case Keyword.Attack:
                case Keyword.Defence:
                case Keyword.MaxHP:
                case Keyword.Speed:
                    {
                        _target.statusAdders.Add(new CombatUnit.StatusAdder
                        {
                            parentBuff = processData.referenceBuff,
                            statusType = vars[1],
                            valueString = vars[2]
                        });
                        break;
                    }
                default:
                    {
                        float _value = CombatUtility.Calculate(
                            new CombatUtility.CalculateData
                            {
                                caster = processData.caster,
                                target = processData.target,
                                formula = vars[2],
                                useRawValue = true
                            });
                        int _add = Convert.ToInt32(_value);

                        switch (vars[1])
                        {
                            case Keyword.Hatred:
                                {
                                    processData.target.hatred += _add;
                                    break;
                                }
                            case Keyword.HP:
                                {
                                    processData.target.HP += _add;
                                    break;
                                }
                            case Keyword.SP:
                                {
                                    processData.target.SP += _add;
                                    break;
                                }
                        }

                        break;
                    }
            }

            onCompleted?.Invoke();
        }
    }
}
