using System;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_AddStatus : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            CombatUnit _target;

            switch (vars[0].Trim())
            {
                case Keyword.Self:
                    {
                        if(processData.referenceBuff != null)
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
                default:
                    {
                        throw new Exception("[EffectCommand_AddStatus][Process] Invaild target=" + vars[0]);
                    }
            }

            switch(vars[1].Trim())
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
                        UnityEngine.Debug.LogWarning("Add status formula=" + vars[2]);
                        float _value = CombatUtility.Calculate(
                            new CombatUtility.CalculateData
                            {
                                caster = processData.caster,
                                target = processData.target,
                                formula = vars[2],
                                useRawValue = true
                            });
                        int _add = Convert.ToInt32(_value);
                        UnityEngine.Debug.LogWarning("Add status _add=" + _add);
                        UnityEngine.Debug.LogWarning("Add status status=" + vars[1]);

                        switch (vars[1])
                        {
                            case Keyword.Hatred:
                                {
                                    _target.hatred += _add;
                                    break;
                                }
                            case Keyword.HP:
                                {
                                    if(_add < 0)
                                    {
                                        CombatManager.Instance.ShowDamage(new UI.CombatUIView.DisplayDamageData
                                        {
                                            attackerName = _target.name,
                                            damageValue = -_add,
                                            defenderName = _target.name,
                                            skillName = "效果傷害"
                                        });
                                    }
                                    _target.HP += _add;
                                    break;
                                }
                            case Keyword.SP:
                                {
                                    _target.SP += _add;
                                    break;
                                }
                            default:
                                {
                                    throw new Exception("[EffectCommand_AddStatus][Process] Invaild status=" + vars[1]);
                                }
                        }

                        break;
                    }
            }

            onCompleted?.Invoke();
        }
    }
}
