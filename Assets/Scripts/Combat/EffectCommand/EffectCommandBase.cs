using System;

namespace ProjectBS.Combat.EffectCommand
{
    public abstract class EffectCommandBase
    {
        public EffectProcesser.ProcessData processData = null;
        public abstract void Process(string[] vars, Action onCompleted);
    }
}
