﻿using System;
using System.Collections.Generic;
using KahaGameCore.Static;
using ProjectBS.Data;

namespace ProjectBS.Combat
{
    public class AllCombatUnitAllEffectProcesser
    {
        private List<CombatUnit> m_units = null;

        public class ProcesserData
        {
            public CombatUnit caster = null;
            public CombatUnit target = null;
            public EffectProcesser.TriggerTiming timing = EffectProcesser.TriggerTiming.OnActived;
            public Action onEnded = null;
        }

        private ProcesserData m_data = null;

        private int m_currentUnitIndex = -1;
        private int m_currentEquipment = -1;
        private string[] m_currentEquipmentEffectIDs = null;
        private int m_currentEquipmentEffectIDIndex = -1;
        private string[] m_currentSkillIDs = null;
        private int m_currentSkillIndex = -1;
        private int m_currentBuffIndex = -1;

        public AllCombatUnitAllEffectProcesser(List<CombatUnit> units)
        {
            m_units = units;
        }

        public void Start(ProcesserData data)
        {
            m_data = data;
            m_currentUnitIndex = -1;
            GoNextUnit();
        }

        private void GoNextUnit()
        {
            m_currentUnitIndex++;
            if(m_currentUnitIndex >= m_units.Count)
            {
                m_data.onEnded?.Invoke();
                return;
            }

            if(m_data.timing.ToString().Contains("_Self"))
            {
                if(m_data.caster == null)
                {
                    m_data.onEnded?.Invoke();
                    return;
                }

                if (m_units[m_currentUnitIndex] != m_data.caster)
                {
                    GoNextUnit();
                    return;
                }
            }

            m_currentEquipment = -1;
            GoNextEquipment();
        }

        private void GoNextEquipment()
        {
            m_currentEquipment++;
            if (m_currentEquipment >= 4)
            {
                m_currentSkillIndex = -1;
                m_currentSkillIDs = m_units[m_currentUnitIndex].skills.Split(',');
                GoNextOwingSkill();
                return;
            }

            switch (m_currentEquipment)
            {
                case 0:
                    {
                        if (m_units[m_currentUnitIndex].head == null)
                        {
                            GoNextEquipment();
                            return;
                        }

                        m_currentEquipmentEffectIDs = m_units[m_currentUnitIndex].head.EffectIDs.Split('$');
                        break;
                    }
                case 1:
                    {
                        if (m_units[m_currentUnitIndex].body == null)
                        {
                            GoNextEquipment();
                            return;
                        }

                        m_currentEquipmentEffectIDs = m_units[m_currentUnitIndex].body.EffectIDs.Split('$');
                        break;
                    }
                case 2:
                    {
                        if (m_units[m_currentUnitIndex].hand == null)
                        {
                            GoNextEquipment();
                            return;
                        }

                        m_currentEquipmentEffectIDs = m_units[m_currentUnitIndex].hand.EffectIDs.Split('$');
                        break;
                    }
                case 3:
                    {
                        if (m_units[m_currentUnitIndex].foot == null)
                        {
                            GoNextEquipment();
                            return;
                        }

                        m_currentEquipmentEffectIDs = m_units[m_currentUnitIndex].foot.EffectIDs.Split('$');
                        break;
                    }
            }

            m_currentEquipmentEffectIDIndex = -1;
            GoNextEquipmentEffect();
        }

        private void GoNextEquipmentEffect()
        {
            m_currentEquipmentEffectIDIndex++;
            if(m_currentEquipmentEffectIDIndex >= m_currentEquipmentEffectIDs.Length)
            {
                GoNextEquipment();
                return;
            }

            if (m_currentEquipmentEffectIDs[m_currentEquipmentEffectIDIndex] == "0")
            {
                GoNextEquipmentEffect();
                return;
            }

            string _command = GameDataManager.GetGameData<SkillEffectData>(m_currentEquipmentEffectIDs[m_currentEquipmentEffectIDIndex].ToInt()).Command;

            new EffectProcesser(_command).Start(new EffectProcesser.ProcessData
            {
                caster = m_data.caster == null ? m_units[m_currentUnitIndex] : m_data.caster,
                target = m_data.target == null ? m_units[m_currentUnitIndex] : m_data.target,
                timing = m_data.timing,
                allEffectProcesser = this,
                referenceBuff = null,
                refenceSkill = null,
                onEnded = GoNextEquipmentEffect
            });
        }

        private void GoNextOwingSkill()
        {
            m_currentSkillIndex++;
            if(m_currentSkillIndex >= m_currentSkillIDs.Length)
            {
                m_currentBuffIndex = -1;
                GoNextBuff();
                return;
            }

            if(string.IsNullOrEmpty(m_currentSkillIDs[m_currentSkillIndex]))
            {
                GoNextOwingSkill();
                return;
            }

            if(m_currentSkillIDs[m_currentSkillIndex] == "0")
            {
                GoNextOwingSkill();
                return;
            }

            SkillData _skill = GameDataManager.GetGameData<SkillData>(m_currentSkillIDs[m_currentSkillIndex].ToInt());
            if(_skill == null)
            {
                throw new Exception("[AllCombatUnitAllEffectProcesser][GoNextOwingSkill] Can't find skill id=" + m_currentSkillIDs[m_currentSkillIndex]);
            }
            
            string _command = _skill.Command;

            new EffectProcesser(_command).Start(new EffectProcesser.ProcessData
            {
                caster = m_data.caster == null ? m_units[m_currentUnitIndex] : m_data.caster,
                target = m_data.target == null ? m_units[m_currentUnitIndex] : m_data.target,
                timing = m_data.timing,
                allEffectProcesser = this,
                referenceBuff = null,
                refenceSkill = _skill,
                onEnded = GoNextOwingSkill
            });
        }

        private void GoNextBuff()
        {
            m_currentBuffIndex++;
            if(m_currentBuffIndex >= m_units[m_currentUnitIndex].buffs.Count)
            {
                GoNextUnit();
                return;
            }

            CombatUnit.Buff _currentBuff = m_units[m_currentUnitIndex].buffs[m_currentBuffIndex];
            string _command = GameDataManager.GetGameData<SkillEffectData>(_currentBuff.effectID).Command;

            new EffectProcesser(_command).Start(new EffectProcesser.ProcessData
            {
                caster = _currentBuff.from,
                target = m_units[m_currentUnitIndex],
                timing = m_data.timing,
                allEffectProcesser = this,
                referenceBuff = _currentBuff,
                refenceSkill = null,
                onEnded = GoNextBuff
            });
        }
    }
}
