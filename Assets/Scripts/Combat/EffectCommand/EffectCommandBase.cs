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
                return CombatUtility.ComabtManager.GetUnitByUDID(processData.referenceBuff.ownerUnitUDID);
            }
        }

        protected string GetSelectID()
        {
            if(processData.refenceSkill != null)
            {
                return "Skill" + processData.refenceSkill.ID;
            }

            if (processData.referenceBuff != null)
            {
                return "Buff" + processData.referenceBuff.GetBuffSourceData().ID;
            }

            if(processData.caster != null)
            {
                return "Actor" + processData.caster.GetHashCode();
            }

            throw new Exception("[EffectCommandBase][GetSelectID] Must reference to skill or buff or having caster when select");
        }
    }
}
