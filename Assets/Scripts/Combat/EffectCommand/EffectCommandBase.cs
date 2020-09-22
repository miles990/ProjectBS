using System;

namespace ProjectBS.Combat.EffectCommand
{
    public abstract class EffectCommandBase : KahaGameCore.Interface.Manager
    {
        public EffectProcesser.ProcessData processData = null;
        public abstract void Process(string[] vars, Action onCompleted);

        protected CombatUnit GetSelf()
        {
            if(processData.referenceBuff == null)
            {
                return processData.caster;
            }
            else
            {
                return processData.referenceBuff.owner;
            }
        }
    }
}
