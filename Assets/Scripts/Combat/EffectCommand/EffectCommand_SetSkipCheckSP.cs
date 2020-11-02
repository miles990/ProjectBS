using System;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_SetSkipCheckSP : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            CombatUnit.Skiper _skiper
                = GetSelf().checkSPSkipers.Find(x => x.parentBuff == processData.referenceBuff);

            if (_skiper == null)
            {
                GetSelf().checkSPSkipers.Add(new CombatUnit.Skiper
                {
                    parentBuff = processData.referenceBuff
                });
            }
            onCompleted?.Invoke();
        }
    }
}
