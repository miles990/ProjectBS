using KahaGameCore.Static;
using ProjectBS.Data;
using System;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_CastSkill : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            if (GetSelf().IsSkipAtion)
                onCompleted?.Invoke();

            AddSkillOrEffectInfo();

            SkillData _skill = GameDataManager.GetGameData<SkillData>(int.Parse(vars[0]));
            if(_skill == null)
            {
                throw new Exception("[EffectCommand_CastSkill][Process] Invaild Skill ID=" + int.Parse(vars[0]));
            }

            GetPage<UI.CombatUIView>().AddCombatInfo
                (
                    string.Format
                    (
                        ContextConverter.Instance.GetContext(500018),
                        GetSelf().name,
                        ContextConverter.Instance.GetContext(_skill.NameContextID)
                    ), null
                );

            GetPage<UI.CombatUIView>().ShowCastSkill(GetSelf(), ContextConverter.Instance.GetContext(_skill.NameContextID),
            delegate
            {
                CombatUtility.ComabtManager.AddCastSkill(GetSelf().UDID, _skill.ID);
                onCompleted?.Invoke();
            });
        }
    }
}
