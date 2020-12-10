using System;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_SetSkipCheckSP : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            CombatUnit.Skiper _skiper
                = GetSelf().checkSPSkipers.Find(x => x.parentBuffID == processData.referenceBuff.soruceID);

            if (_skiper == null)
            {
                GetSelf().checkSPSkipers.Add(new CombatUnit.Skiper
                {
                    parentBuffID = processData.referenceBuff.soruceID
                });
            }
            onCompleted?.Invoke();
        }
    }
}
