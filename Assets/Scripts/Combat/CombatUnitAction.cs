using System.Collections.Generic;
using System;
using KahaGameCore.Interface;
using ProjectBS.Data;
using KahaGameCore.Static;

namespace ProjectBS.Combat
{
    public class CombatUnitAction : Manager
    {
        public CombatUnit Actor { get; } = null;
        public int MinAttackRoll = 0;
        public int MinDefenseRoll = 0;

        private AllCombatUnitAllEffectProcesser m_processer = null;
        private Action m_onEnded = null;

        public CombatUnitAction(CombatUnit actor, AllCombatUnitAllEffectProcesser processer)
        {
            Actor = actor;
            m_processer = processer;
        }

        public void Start(Action onEnded)
        {
            m_onEnded = onEnded;
            GetPage<UI.CombatUIView>().ShowActorActionStart(Actor, OnActionAnimationEnded);
        }

        public void ForceEnd()
        {
            GetPage<UI.CombatUIView>().OnSkillSelected -= OnSkillSelected;
            OnSkillEnded();
        }

        private void OnActionAnimationEnded()
        {
            m_processer.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = Actor,
                target = Actor,
                timing = EffectProcesser.TriggerTiming.OnActionStarted_Any,
                onEnded = OnActionStarted_Any_Ended
            });
        }

        private void OnActionStarted_Any_Ended()
        {
            m_processer.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = Actor,
                target = Actor,
                timing = EffectProcesser.TriggerTiming.OnActionStarted_Self,
                onEnded = Act
            });
        }

        private void Act()
        {
            if (Actor.IsSkipAtion || Actor.HP <= 0)
            {
                if (Actor.HP > 0)
                {
                    GetPage<UI.CombatUIView>().ShowForceEndAction(Actor);
                    TimerManager.Schedule(UI.CombatUIView.DISPLAY_INFO_TIME, OnSkillEnded);
                }
                else
                    OnSkillEnded();
                return;
            }

            if (!string.IsNullOrEmpty(Actor.ai))
            {
                new EffectProcesser(Actor.ai).Start(
                    new EffectProcesser.ProcessData
                    {
                        caster = Actor,
                        target = null,
                        refenceSkill = null,
                        referenceBuff = null,
                        skipIfCount = 0,
                        allEffectProcesser = m_processer,
                        timing = EffectProcesser.TriggerTiming.AI,
                        onEnded = OnSkillEnded
                    });
            }
            else
            {
                GetPage<UI.CombatUIView>().OnSkillSelected += OnSkillSelected;
                List<SkillData> _skills = new List<SkillData>();
                for(int i = 0; i < Actor.skills.Length; i++)
                {
                    if(Actor.skills[i] == 0)
                    {
                        continue;
                    }
                    _skills.Add(GameDataManager.GetGameData<SkillData>(Actor.skills[i]));
                }
                GetPage<UI.CombatUIView>().RefreshCurrentSkillMenu(_skills);
            }
        }

        private void OnSkillSelected(SkillData skill)
        {
            GetPage<UI.CombatUIView>().OnSkillSelected -= OnSkillSelected;

            if(Actor.SP < skill.SP && !Actor.IsSkipCheckSP)
            {
                Act();
                return;
            }

            Actor.lastSkillID = skill.ID;
            
            if(!Actor.IsSkipCheckSP)
            {
                Actor.SP -= skill.SP;
            }

            EffectProcessManager.GetSkillProcesser(skill.ID).Start(new EffectProcesser.ProcessData
            {
                caster = null,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnActived,
                allEffectProcesser = m_processer,
                referenceBuff = null,
                refenceSkill = skill,
                onEnded = OnSkillEnded
            });
        }

        private void OnSkillEnded()
        {
            m_processer.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = Actor,
                target = Actor,
                timing = EffectProcesser.TriggerTiming.OnStartToEndAction_Any,
                onEnded = OnStartToEndActionAnyEffectEnded
            });
        }

        private void OnStartToEndActionAnyEffectEnded()
        {
            m_processer.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = Actor,
                target = Actor,
                timing = EffectProcesser.TriggerTiming.OnStartToEndAction_Self,
                onEnded = OnStartToEndActionSelfEnded
            });
        }

        private void OnStartToEndActionSelfEnded()
        {
            GetPage<UI.CombatUIView>().ShowActorActionEnd(Actor, m_onEnded);
        }
    }
}

