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

        private AllCombatUnitAllEffectProcesser m_processer = null;
        private Action m_onEnded = null;

        private bool m_forceStop = false;

        public CombatUnitAction(CombatUnit actor, AllCombatUnitAllEffectProcesser processer)
        {
            Actor = actor;
            m_processer = processer;
        }

        public void Start(Action onEnded)
        {
            m_onEnded = onEnded;

            GetPage<UI.CombatUIView>().OnActionAnimationEnded += OnActionAnimationEnded;
            GetPage<UI.CombatUIView>().ShowActorActionStart(Actor);
        }

        public void MarkForceStopOnStart()
        {
            m_forceStop = true;
        }

        private void OnActionAnimationEnded()
        {
            GetPage<UI.CombatUIView>().OnActionAnimationEnded -= OnActionAnimationEnded;
            m_processer.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = null,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnActionStarted_Any,
                onEnded = OnActionStarted_Any_Ended
            });
        }

        private void OnActionStarted_Any_Ended()
        {
            if(m_forceStop)
            {
                m_onEnded?.Invoke();
                return;
            }

            m_processer.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = Actor,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnActionStarted_Self,
                onEnded = Act
            });
        }

        private void Act()
        {
            if (m_forceStop)
            {
                CombatManager.Instance.ShowForceEndAction();
                m_onEnded?.Invoke();
                return;
            }

            if (!string.IsNullOrEmpty(Actor.ai))
            {
                UnityEngine.Debug.LogWarning(Actor.ai);
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
                string[] _skillIDs = Actor.skills.Split(',');
                for(int i = 0; i < _skillIDs.Length; i++)
                {
                    if(_skillIDs[i] == "0")
                    {
                        continue;
                    }
                    _skills.Add(GameDataManager.GetGameData<SkillData>(_skillIDs[i].ToInt()));
                }
                GetPage<UI.CombatUIView>().RefreshCurrentSkillMenu(_skills);
            }
        }

        private void OnSkillSelected(SkillData skill)
        {
            GetPage<UI.CombatUIView>().OnSkillSelected -= OnSkillSelected;
            Actor.lastSkillID = skill.ID;

            new EffectProcesser(skill.Command).Start(new EffectProcesser.ProcessData
            {
                caster = Actor,
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
                caster = null,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnStartToEndAction_Any,
                onEnded = OnStartToEndActionAnyEffectEnded
            });
        }

        private void OnStartToEndActionAnyEffectEnded()
        {
            m_processer.Start(new AllCombatUnitAllEffectProcesser.ProcesserData
            {
                caster = Actor,
                target = null,
                timing = EffectProcesser.TriggerTiming.OnStartToEndAction_Self,
                onEnded = OnStartToEndActionSelfEnded
            });
        }

        private void OnStartToEndActionSelfEnded()
        {
            UnityEngine.Debug.LogWarning("Action End:" + Actor.name);
            m_onEnded?.Invoke();
        }
    }
}

