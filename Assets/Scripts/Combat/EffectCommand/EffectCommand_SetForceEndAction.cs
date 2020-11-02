using System;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_SetForceEndAction : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            CombatUnit.Skiper _actionSkiper
                = GetSelf().actionSkipers.Find(x => x.parentBuff == processData.referenceBuff);

            if(_actionSkiper == null)
            {
                GetSelf().actionSkipers.Add(new CombatUnit.Skiper
                {
                    parentBuff = processData.referenceBuff
                });
            }

            onCompleted?.Invoke();
        }
    }
}

