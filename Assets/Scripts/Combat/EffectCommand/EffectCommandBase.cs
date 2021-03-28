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
                GetPage<UI.CombatUIView>().AddCombatInfo
                    (
                      string.Format
                      (
                          ContextConverter.Instance.GetContext(500001), 
                          ContextConverter.Instance.GetContext(processData.refenceSkill.skill.NameContextID)
                      ), null
                    );
            }
            if (processData.referenceBuff != null)
            {
                GetPage<UI.CombatUIView>().AddCombatInfo
                    (
                      string.Format
                      (
                          ContextConverter.Instance.GetContext(500002),
                          ContextConverter.Instance.GetContext(processData.referenceBuff.GetBuffSourceData().NameContextID)
                      ), null
                    );
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
