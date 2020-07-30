using System.Collections.Generic;
using System;
using KahaGameCore.Interface;

namespace ProjectBS.Combat
{
    public class CombatUnitAction : Manager
    {
        private CombatUnit m_actor = null;
        private CombatUnitEffectProcesser m_processer = null;
        private Action m_onEnded = null;

        public CombatUnitAction(CombatUnit actor, CombatUnitEffectProcesser processer)
        {
            m_actor = actor;
            m_processer = processer;
        }

        public void Start(Action onEnded)
        {
            m_onEnded = onEnded;

            m_processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = null,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnActionStarted_Any,
                onEnded = OnActionStarted_Any_Ended
            });
        }

        private void OnActionStarted_Any_Ended()
        {
            m_processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = m_actor,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnActionStarted_Self,
                onEnded = Act
            });
        }

        private void Act()
        {
            if(!string.IsNullOrEmpty(m_actor.ai))
            {
                UnityEngine.Debug.Log(m_actor.ai);
                OnSkillEnded();
            }
            else
            {
                GetPage<UI.CombatUIView>().OnSkillSelected += OnSkillSelected;
                List<Data.SkillData> _skills = new List<Data.SkillData>();
                string[] _skillIDs = m_actor.skills.Split(',');
                for(int i = 0; i < _skillIDs.Length; i++)
                {
                    _skills.Add(GameDataLoader.Instance.GetSkill(_skillIDs[i]));
                }
                GetPage<UI.CombatUIView>().RefreshCurrentSkillMenu(_skills);
            }
        }

        private void OnSkillSelected(Data.SkillData skill)
        {
            GetPage<UI.CombatUIView>().OnSkillSelected -= OnSkillSelected;
            new EffectProcesser(skill.Command).Start(new EffectProcesser.ProcessData
            {
                caster = m_actor,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnActived,
                processer = m_processer,
                onEnded = OnSkillEnded
            });
        }

        private void OnSkillEnded()
        {
            m_processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = null,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnStartToEndAction_Any,
                onEnded = OnStartToEndActionAnyEffectEnded
            });
        }

        private void OnStartToEndActionAnyEffectEnded()
        {
            m_processer.Start(new CombatUnitEffectProcesser.ProcesserData
            {
                caster = m_actor,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnStartToEndAction_Self,
                onEnded = m_onEnded
            });
        }
    }
}

