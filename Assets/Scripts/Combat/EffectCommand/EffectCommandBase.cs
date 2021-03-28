using System;

namespace ProjectBS.Combat.EffectCommand
{
    public abstract class EffectCommandBase : KahaGameCore.Interface.Manager
    {
        public EffectProcesser.ProcessData processData = null;
        public abstract void Process(string[] vars, Action onCompleted);

        protected virtual void AddSkillOrEffectInfo()
        {
            if(processData.refenceSkill != null)
            {
                GetPage<UI.CombatUIView>().AddCombatInfo("processData.refenceSkill", null);
            }
            if (processData.referenceBuff != null)
            {
                GetPage<UI.CombatUIView>().AddCombatInfo("processData.referenceBuff", null);
            }
        }

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
