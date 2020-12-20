using System;
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
        private int[] m_currentSkillIDs = null;
        private int m_currentSkillIndex = -1;
        private int m_currentBuffIndex = -1;
        private EffectProcesser m_currentBuffProcesser = null;
        private EffectProcesser.ProcessData m_currentBuffProcessData = null;
        private int m_currentBuffStack = 0;
        private int m_currentBuffRunningStack = -1;

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

            if (m_data.timing.ToString().Contains("_Any"))
            {
                if (m_data.caster != null && m_units[m_currentUnitIndex] == m_data.caster)
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
                m_currentSkillIDs = m_units[m_currentUnitIndex].skills;
                GoNextOwingSkill();
                return;
            }

            switch (m_currentEquipment)
            {
                case 0:
                    {
                        if(!TryGetEquipmentEffect(m_units[m_currentUnitIndex].head))
                        {
                            GoNextEquipment();
                            return;
                        }
                        break;
                    }
                case 1:
                    {
                        if (!TryGetEquipmentEffect(m_units[m_currentUnitIndex].body))
                        {
                            GoNextEquipment();
                            return;
                        }
                        break;
                    }
                case 2:
                    {
                        if (!TryGetEquipmentEffect(m_units[m_currentUnitIndex].hand))
                        {
                            GoNextEquipment();
                            return;
                        }
                        break;
                    }
                case 3:
                    {
                        if (!TryGetEquipmentEffect(m_units[m_currentUnitIndex].foot))
                        {
                            GoNextEquipment();
                            return;
                        }
                        break;
                    }
            }

            m_currentEquipmentEffectIDIndex = -1;
            GoNextEquipmentEffect();
        }

        private bool TryGetEquipmentEffect(string udid)
        {
            if (string.IsNullOrEmpty(udid)
                || string.IsNullOrEmpty(PlayerManager.Instance.GetEquipmentByUDID(udid).EffectIDs))
            {
                return false;
            }

            m_currentEquipmentEffectIDs = PlayerManager.Instance.GetEquipmentByUDID(udid).EffectIDs.Split('$');
            return true;
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

            EffectProcessManager.GetBuffProcesser(m_currentEquipmentEffectIDs[m_currentEquipmentEffectIDIndex].ToInt())
            .Start(new EffectProcesser.ProcessData
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

            if(m_currentSkillIDs[m_currentSkillIndex] == 0)
            {
                GoNextOwingSkill();
                return;
            }

            EffectProcessManager.GetSkillProcesser(m_currentSkillIDs[m_currentSkillIndex]).Start(new EffectProcesser.ProcessData
            {
                caster = m_data.caster == null ? m_units[m_currentUnitIndex] : m_data.caster,
                target = m_data.target == null ? m_units[m_currentUnitIndex] : m_data.target,
                timing = m_data.timing,
                allEffectProcesser = this,
                referenceBuff = null,
                refenceSkill = GameDataManager.GetGameData<SkillData>(m_currentSkillIDs[m_currentSkillIndex]),
                onEnded = GoNextOwingSkill
            });
        }

        private void GoNextBuff()
        {
            m_currentBuffIndex++;
            if(m_currentBuffIndex >= m_units[m_currentUnitIndex].OwnBuffCount)
            {
                GoNextUnit();
                return;
            }

            CombatUnit.Buff _currentBuff = m_units[m_currentUnitIndex].GetBuffByIndex(m_currentBuffIndex);

            m_currentBuffProcesser = EffectProcessManager.GetBuffProcesser(_currentBuff.soruceID);
            m_currentBuffProcessData = new EffectProcesser.ProcessData
            {
                caster = m_units[m_currentUnitIndex],
                target = null,
                timing = m_data.timing,
                allEffectProcesser = this,
                referenceBuff = _currentBuff,
                refenceSkill = null,
                onEnded = GoNextBuffStack
            };
            m_currentBuffStack = _currentBuff.stackCount;
            m_currentBuffRunningStack = 0;
            GoNextBuffStack();
        }

        private void GoNextBuffStack()
        {
            m_currentBuffRunningStack++;
            if(m_currentBuffRunningStack > m_currentBuffStack)
            {
                GoNextBuff();
                return;
            }

            m_currentBuffProcesser.Start(m_currentBuffProcessData);
        }
    }
}
