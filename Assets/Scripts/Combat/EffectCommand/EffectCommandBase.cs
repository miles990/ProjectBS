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
                if(processData.refenceSkill == null)
                {
                    return processData.caster;
                }
                else
                {
                    return processData.refenceSkill.owner;
                }
            }
            else
            {
                return CombatUtility.ComabtManager.GetUnitByUDID(processData.referenceBuff.ownerUnitUDID);
            }
        }
    }
}
