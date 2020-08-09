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

            _target.statusAdders.Add(new CombatUnit.StatusAdder
            {
                parentBuff = processData.referenceBuff,
                statusType = vars[1],
                valueString = vars[2]
            });

            onCompleted?.Invoke();
        }
    }
}
